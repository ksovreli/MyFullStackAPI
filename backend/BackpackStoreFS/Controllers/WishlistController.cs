using BackpackStoreFS.Data;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController(IWishlistService wishlistService) : ControllerBase
    {
        private readonly IWishlistService _wishlistService = wishlistService;

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Backpack>>> GetWishlist(string userId)
        {
            var items = await _wishlistService.GetWishlistAsync(userId);
            return Ok(items);
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleWishlist([FromBody] WishlistItem dto)
        {
            var status = await _wishlistService.ToggleWishlistAsync(dto);
            return Ok(new { status = status });
        }
    }
}
