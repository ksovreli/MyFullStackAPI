using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
