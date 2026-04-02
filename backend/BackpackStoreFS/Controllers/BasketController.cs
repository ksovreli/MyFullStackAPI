using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackpackStoreFS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController(IBasketService basketService) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<BasketItemReadDto>>> GetBasket(int userId)
        {
            var basket = await basketService.GetUserBasketAsync(userId);
            return Ok(basket);
        }

        [HttpPost("AddToBasket")]
        public async Task<IActionResult> AddItem([FromBody] BasketItemCreateDto dto)
        {
            await basketService.AddToBasket(dto);
            return Ok();
        }

        [HttpPut("{backpackId}/{userId}")]
        public async Task<IActionResult> UpdateQty(int backpackId, int userId, [FromQuery] int quantity)
        {
            await basketService.UpdateQuantityAsync(userId, backpackId, quantity);
            return NoContent();
        }

        [HttpDelete("{backpackId}/{userId}")]
        public async Task<IActionResult> Remove(int backpackId, int userId)
        {
            await basketService.RemoveItemAsync(userId, backpackId);
            return NoContent();
        }
    }
}
