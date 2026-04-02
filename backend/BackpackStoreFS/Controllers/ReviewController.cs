using BackpackStoreFS.Constants;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IReviewService reviewService) : ControllerBase
    {
        [HttpGet("backpack/{backpackId}")]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetReviews(int backpackId)
        {
            var reviews = await reviewService.GetReviewsByBackpackAsync(backpackId);
            return Ok(reviews);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostReview(ReviewCreateDto dto)
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(claimId, out int userId))
            {
                return Unauthorized("User ID in token is not a valid number.");
            }

            if (await reviewService.HasUserReviewedAsync(userId, dto.BackpackId))
            {
                return Conflict(BackpackConstants.Messages.DuplicateReview);
            }

            return Ok(await reviewService.CreateReviewAsync(dto, userId, User.Identity?.Name ?? "User"));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, ReviewCreateDto dto)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }
            return await reviewService.UpdateReviewAsync(id, dto, userId) ? NoContent() : NotFound();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }
            return await reviewService.DeleteReviewAsync(id, userId) ? NoContent() : NotFound();
        }
    }
}
