using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController(IBasketService basketService) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<BasketItemReadDto>>> GetBasket(string userId)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQty(int id, [FromQuery] int quantity)
        {
            await basketService.UpdateQuantityAsync(id, quantity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            await basketService.RemoveItemAsync(id);
            return NoContent();
        }

        private BasketItem FromBasketDto(BasketItemCreateDto dto)
        {
            return new BasketItem
            {
                UserId = dto.UserId,
                BackpackId = dto.BackpackId,
                Quantity = dto.Quantity
            };
        }
    }
}
