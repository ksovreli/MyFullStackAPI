using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackpackStoreFS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController(IBasketService basketService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasketItemReadDto>>> GetBasket()
        {
            var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
                               ?? User.FindFirstValue("sub");

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("მომხმარებლის ID ტოკენში არასწორია ან არ არსებობს.");
            }

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
