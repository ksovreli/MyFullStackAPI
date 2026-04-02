using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackpackStoreFS.Services
{
    public class AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration,
        BackpackContext context,
        IEmailService emailService
        ) : IAuthService
    {
        public async Task<UserReadDto?> RegisterAsync(UserCreateDto dto)
        {
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                Role = "User",
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) return null;

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{configuration["AppUrl"]}/verify-email?token={Uri.EscapeDataString(token)}&email={user.Email}";

            var message = $@"
                <div style='background-color: #050505; padding: 40px 20px; font-family: sans-serif; text-align: center;'>
                    <div style='max-width: 400px; margin: 0 auto; background: #0d0d12; border: 1px solid #1a1a1a; border-top: 4px solid #d4af37; padding: 30px; border-radius: 8px;'>
                        <h2 style='color: #ffffff;'>WELCOME_OPERATIVE_{user.UserName?.ToUpper()}</h2>
                        <p style='color: #a29c9c; font-size: 14px;'>Confirm your coordinates to activate your profile.</p>
                        <a href='{confirmationLink}' style='display: inline-block; background: #d4af37; color: #000; padding: 12px 25px; text-decoration: none; font-weight: bold; margin: 20px 0;'>ACTIVATE_ACCOUNT</a>
                    </div>
                </div>";

            await emailService.SendEmailAsync(user.Email!, "Account Activation Required", message);

            return MapToReadDto(user);
        }

        public async Task<UserReadDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                Console.WriteLine("--- LOGIN DEBUG: User not found ---");
                return null;
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                Console.WriteLine("--- LOGIN DEBUG: Email NOT confirmed! ---");
                return null;
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                Console.WriteLine($"--- LOGIN DEBUG: Password check failed. Succeeded: {result.Succeeded} ---");
                return null;
            }

            Console.WriteLine("--- LOGIN DEBUG: SUCCESS! ---");
            var readDto = MapToReadDto(user);
            readDto.Token = GenerateToken(user);
            return readDto;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task<bool> SendResetCodeAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var code = new Random().Next(100000, 999999).ToString();

            var oldCodes = context.PasswordResetCodes.Where(c => c.Email == email);
            context.PasswordResetCodes.RemoveRange(oldCodes);

            context.PasswordResetCodes.Add(new PasswordResetCode
            {
                Email = email,
                Code = code,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            });

            await context.SaveChangesAsync();

            var message = $@"
                <div style='background-color: #050505; padding: 40px 20px; font-family: ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif; text-align: center; border-radius: 12px;'>
                    <div style='max-width: 400px; margin: 0 auto; background: #0d0d12; border: 1px solid #1a1a1a; border-top: 4px solid #00ff88; padding: 30px; border-radius: 8px; box-shadow: 0 10px 30px rgba(0,0,0,0.5);'>
                        
                        <div style='text-transform: uppercase; letter-spacing: 3px; font-size: 12px; color: #00ff88; margin-bottom: 20px; font-weight: bold;'>
                            System Authentication
                        </div>
                        
                        <h2 style='color: #ffffff; margin: 0 0 10px 0; font-size: 24px; font-weight: 300;'>APEX <span style='color: #00ff88; font-weight: 800;'>STORE</span></h2>
                        
                        <p style='color: #a29c9c; font-size: 14px; line-height: 1.5;'>A request was made to access your account. Use the secure passkey below to proceed.</p>
                        
                        <div style='background: rgba(0, 255, 136, 0.05); border: 1px dashed rgba(0, 255, 136, 0.3); margin: 30px 0; padding: 20px; border-radius: 4px;'>
                            <h1 style='color: #00ff88; letter-spacing: 12px; font-size: 38px; margin: 0; font-family: monospace;'>{code}</h1>
                        </div>
                        
                        <p style='color: #666; font-size: 11px; margin-bottom: 0;'>
                            This code will expire in <span style='color: #00ff88;'>10 minutes</span>.<br>
                            If you did not request this, please ignore this email.
                        </p>
                    </div>
                    
                    <div style='margin-top: 20px; color: #444; font-size: 10px; text-transform: uppercase; letter-spacing: 1px;'>
                        &copy; {DateTime.Now.Year} Apex Store Terminal. All Rights Reserved.
                    </div>
                </div>";

            await emailService.SendEmailAsync(email, "Your Access Code", message);
            return true;
        }

        public async Task<bool> ResetPasswordWithCodeAsync(ResetPasswordDto dto)
        {
            var codeRecord = await context.PasswordResetCodes
                .FirstOrDefaultAsync(c => c.Email == dto.Email && c.Code == dto.Token && c.ExpiryTime > DateTime.UtcNow);

            if (codeRecord == null)
            {
                return false;
            }

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return false;
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var internalToken = await userManager.GeneratePasswordResetTokenAsync(user);

                var result = await userManager.ResetPasswordAsync(user, internalToken, dto.NewPassword);

                if (result.Succeeded)
                {
                    context.PasswordResetCodes.Remove(codeRecord);
                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        private static UserReadDto MapToReadDto(User user)
        {
            return new UserReadDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };
        }
    }
}