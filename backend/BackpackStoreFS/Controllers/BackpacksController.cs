using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackpacksController(IBackpackService backpackService, ILogger<BackpacksController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BackpackReadDto>>> GetBackpacks()
        {
            try
            {
                var result = await backpackService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching all backpacks.");
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BackpackReadDto>> GetBackpack(int id)
        {
            try
            {
                var result = await backpackService.GetByIdAsync(id);

                if (result != null)
                {
                    return Ok(result);
                }
                return NotFound(new { message = $"Backpack with ID {id} not found." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while fetching backpack with ID {0}.", id);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Backpack>> CreateBackpack([FromBody] BackpackCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await backpackService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetBackpack), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating a backpack.");
                return StatusCode(500, new { message = "Could not create backpack due to a server error." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBackpack(int id, [FromBody] BackpackCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await backpackService.UpdateAsync(id, dto);

                if (success)
                {
                    return NoContent();
                }
                return NotFound(new { message = $"Backpack with ID {id} not found for update." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating backpack with ID {0}.", id);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBackpack(int id)
        {
            try
            {
                var success = await backpackService.DeleteAsync(id);

                if (success)
                {
                    return NoContent();
                }
                return NotFound(new { message = $"Backpack with ID {id} not found for deletion." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting backpack with ID {0}.", id);
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<BackpackReadDto>>> GetFilteredBackpacks(
            [FromQuery] string? category,
            [FromQuery] string? sortBy)
        {
            try
            {
                var results = await backpackService.GetFilteredAsync(category, sortBy);
                return Ok(results);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while filtering backpacks.");
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }
    }
}