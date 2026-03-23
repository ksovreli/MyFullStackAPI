using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackpackStoreFS.Services
{
    public interface IAuthService
    {
        Task<UserReadDto?> RegisterAsync(UserCreateDto dto);
        Task<UserReadDto?> LoginAsync(UserLoginDto dto);
    }

    public class AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration) : IAuthService
    {
        public async Task<UserReadDto?> RegisterAsync(UserCreateDto dto)
        {
            var user = new User { UserName = dto.Username, Email = dto.Email };
            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return null;
            }

            var readDto = MapToReadDto(user);
            readDto.Token = GenerateToken(user);
            return readDto;
        }

        public async Task<UserReadDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return null;
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return null;
            }

            var readDto = MapToReadDto(user);
            readDto.Token = GenerateToken(user);
            return readDto;
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        // Use the standard ClaimTypes so [Authorize] recognizes you
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email!),
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
