using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController(IWishlistService wishlistService) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Backpack>>> GetWishlist(int userId)
        {
            var items = await wishlistService.GetWishlistAsync(userId);
            return Ok(items);
        }

        [HttpPost("Toggle")]
        public async Task<IActionResult> ToggleWishlist([FromBody] WishlistDto dto)
        {
            var result = await wishlistService.ToggleWishlistAsync(dto);
            return Ok(new { status = result });
        }
    }
}
