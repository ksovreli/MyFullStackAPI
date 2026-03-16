using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(BackpackContext context) : ControllerBase
    {
        private readonly BackpackContext _context = context;

        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register(UserCreateDto dto)
        {
            var item = await _context.Users.AnyAsync(u => u.Email == dto.Email);

            if (item)
            {
                return BadRequest("Email is already in use.");
            }

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserReadDto>> Login(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordHash == dto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetUserProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}
