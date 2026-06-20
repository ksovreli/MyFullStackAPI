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
        private const string EmailBg = "#050505";
        private const string CardBg = "#0d0d12";
        private const string AccentColor = "#00ff88";
        private const string GoldColor = "#d4af37";

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
                <div style='background-color: {EmailBg}; padding: 50px 20px; font-family: ""Segoe UI"", Tahoma, sans-serif; text-align: center;'>
                    <div style='max-width: 450px; margin: 0 auto; background: {CardBg}; border: 1px solid #1a1a1a; border-top: 4px solid {GoldColor}; padding: 40px; border-radius: 12px; box-shadow: 0 15px 35px rgba(0,0,0,0.5);'>
                        <div style='text-transform: uppercase; letter-spacing: 5px; font-size: 10px; color: {GoldColor}; margin-bottom: 20px; font-weight: bold;'>New Operator Detected</div>
                        <h2 style='color: #ffffff; font-size: 26px; margin: 0;'>WELCOME <span style='color: {GoldColor};'>{user.UserName?.ToUpper()}</span></h2>
                        <p style='color: #a29c9c; font-size: 14px; line-height: 1.6; margin: 20px 0;'>Your account has been provisioned. Finalize the uplink to activate your profile.</p>
                        <a href='{confirmationLink}' style='display: inline-block; background: {GoldColor}; color: #000; padding: 14px 30px; text-decoration: none; font-weight: bold; border-radius: 4px; text-transform: uppercase; letter-spacing: 1px;'>Establish Connection</a>
                        <p style='color: #444; font-size: 11px; margin-top: 30px;'>&copy; {DateTime.Now.Year} APEX STORE FS. Secure Environment.</p>
                    </div>
                </div>";

            await emailService.SendEmailAsync(user.Email!, "Uplink Required: Activate Account", message);

            return MapToReadDto(user);
        }

        public async Task<UserReadDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await userManager.IsEmailConfirmedAsync(user)) return null;

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) return null;

            var readDto = MapToReadDto(user);
            readDto.Token = GenerateToken(user);
            return readDto;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var result = await userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<bool> SendResetCodeAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var code = new Random().Next(100000, 999999).ToString();

            // ძველი კოდების გასუფთავება
            var oldCodes = context.PasswordResetCodes.Where(c => c.Email == email);
            context.PasswordResetCodes.RemoveRange(oldCodes);

            context.PasswordResetCodes.Add(new PasswordResetCode
            {
                Email = email,
                Code = code,
                ExpiryTime = DateTime.UtcNow.AddHours(24) // 24 საათიანი ვადა
            });

            await context.SaveChangesAsync();

            var message = $@"
                <div style='background-color: {EmailBg}; padding: 50px 20px; font-family: ""JetBrains Mono"", monospace, sans-serif; text-align: center;'>
                    <div style='max-width: 450px; margin: 0 auto; background: {CardBg}; border: 1px solid #1a1a1a; border-top: 4px solid {AccentColor}; padding: 40px; border-radius: 12px; box-shadow: 0 15px 35px rgba(0,0,0,0.5);'>
                        <div style='text-transform: uppercase; letter-spacing: 4px; font-size: 11px; color: {AccentColor}; margin-bottom: 20px; font-weight: bold;'>Security Override Protocol</div>
                        <h2 style='color: #ffffff; font-size: 24px; font-weight: 300; margin: 0;'>APEX <span style='color: {AccentColor}; font-weight: 800;'>SECURE</span></h2>
                        <p style='color: #a29c9c; font-size: 13px; margin: 20px 0;'>Authentication required for password reassignment. Use the terminal key below:</p>
                        
                        <div style='background: rgba(0, 255, 136, 0.03); border: 1px dashed rgba(0, 255, 136, 0.3); padding: 25px; margin: 25px 0;'>
                            <h1 style='color: {AccentColor}; letter-spacing: 15px; font-size: 40px; margin: 0;'>{code}</h1>
                        </div>

                        <p style='color: #555; font-size: 11px;'>
                            Key TTL: <span style='color: {AccentColor};'>24 Hours</span>. 
                            If you did not initiate this, secure your terminal immediately.
                        </p>
                    </div>
                </div>";

            await emailService.SendEmailAsync(email, "Apex Security: Access Key", message);
            return true;
        }

        public async Task<bool> ResetPasswordWithCodeAsync(ResetPasswordDto dto)
        {
            var codeRecord = await context.PasswordResetCodes
                .FirstOrDefaultAsync(c => c.Email == dto.Email && c.Code == dto.Token && c.ExpiryTime > DateTime.UtcNow);

            if (codeRecord == null) return false;

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null) return false;

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