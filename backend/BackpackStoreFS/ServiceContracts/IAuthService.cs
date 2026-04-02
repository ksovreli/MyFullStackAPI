using BackpackStoreFS.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace BackpackStoreFS.ServiceContracts
{
    public interface IAuthService
    {
        Task<UserReadDto?> RegisterAsync(UserCreateDto dto);
        Task<UserReadDto?> LoginAsync(UserLoginDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<bool> SendResetCodeAsync(string email);
        Task<bool> ResetPasswordWithCodeAsync(ResetPasswordDto dto);
        Task<bool> DeleteUserAsync(string userId);
    }
}
