using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.ServiceContracts;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register(UserCreateDto dto)
        {
            var result = await authService.RegisterAsync(dto);

            if (result == null)
            {
                return BadRequest("Registration failed. Email may be in use or password does not meet requirements.");
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserReadDto>> Login(UserLoginDto dto)
        {
            var result = await authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("INVALID_COORDINATES");
            }

            var result = await authService.ConfirmEmailAsync(email, token);

            if (result)
            {
                return Ok(new { message = "ACCOUNT_ACTIVATED_SUCCESSFULLY" });
            }

            return BadRequest("ACTIVATION_FAILED: TOKEN_EXPIRED_OR_INVALID");
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email)) return BadRequest("Email is required");

            var result = await authService.SendResetCodeAsync(dto.Email);

            if (result) return Ok();

            return BadRequest("User not found or SMTP failure.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await authService.ResetPasswordWithCodeAsync(dto);
            if (result) return Ok(new { message = "Password updated successfully." });
            return BadRequest("Invalid code or email.");
        }

        [Authorize]
        [HttpDelete("terminate-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await authService.DeleteUserAsync(userId);

            if (success) return Ok(new { message = "USER_TERMINATED_SUCCESSFULLY" });

            return BadRequest("TERMINATION_FAILED");
        }
    }
}
