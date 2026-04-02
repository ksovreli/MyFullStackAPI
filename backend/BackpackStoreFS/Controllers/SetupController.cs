using BackpackStoreFS.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager) : ControllerBase
    {
        [HttpPost("init-roles")]
        public async Task<IActionResult> InitRoles()
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            return Ok("Roles initialized successfully.");
        }

        [HttpPost("promote/{email}")]
        public async Task<IActionResult> PromoteToAdmin(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Role = "Admin";
            await userManager.UpdateAsync(user);

            await userManager.AddToRoleAsync(user, "Admin");

            return Ok($"{email} is now an Admin.");
        }
    }
}
