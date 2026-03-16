using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackpacksController(IBackpackService backpackService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BackpackReadDto>>> GetBackpacks()
        {
            var result = await backpackService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BackpackReadDto>> GetBackpack(int id)
        {
            var result = await backpackService.GetByIdAsync(id);

            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Backpack>> CreateBackpack([FromForm] BackpackCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await backpackService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBackpack), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBackpack(int id, BackpackCreateDto dto)
        {
            var success = await backpackService.UpdateAsync(id, dto);

            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBackpack(int id)
        {
            var success = await backpackService.DeleteAsync(id);

            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

        private Backpack FromDtoBackpack(BackpackCreateDto dto)
        {
            return new Backpack()
            {
                Name = dto.Name,
                Image = dto.ImageUrl,
                Price = dto.Price,
                Quantity = dto.Quantity,
                SalePrice = dto.SalePrice,
                CategoryId = dto.CategoryId,
                IsNew = dto.IsNew,
                Rating = dto.Rating
            };
        }
    }
}
