using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BackpackStoreFS.Services
{
    public interface IAuthService
    {
        Task<UserReadDto?> RegisterAsync(UserCreateDto dto);
        Task<UserReadDto?> LoginAsync(UserLoginDto dto);
    }

    public class AuthService(UserManager<User> userManager, SignInManager<User> signInManager) : IAuthService
    {
        public async Task<UserReadDto?> RegisterAsync(UserCreateDto dto)
        {
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password
            };
            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return null;
            }

            return new UserReadDto
            {
                Id = user.Id,
                Username = dto.Username!,
                Email = user.Email!
            };
        }
        public async Task<UserReadDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return null;
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!
            };
        }
    }
}
