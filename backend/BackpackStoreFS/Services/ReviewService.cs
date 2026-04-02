using BackpackStoreFS.Constants;
using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public class ReviewService(BackpackContext context) : IReviewService
    {
        public async Task<IEnumerable<ReviewReadDto>> GetReviewsByBackpackAsync(int backpackId)
        {
            return await context.Reviews
                .Include(r => r.User)
                .Where(r => r.BackpackId == backpackId)
                .Select(r => new ReviewReadDto
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    BackpackId = r.BackpackId,
                    Username = r.User!.UserName ?? BackpackConstants.Users.Anonymous,
                    Rating = context.Ratings
                        .Where(rat => rat.BackpackId == backpackId && rat.UserId == r.UserId)
                        .Select(rat => rat.Value)
                        .FirstOrDefault()
                }).ToListAsync();
        }

        public async Task<bool> HasUserReviewedAsync(int userId, int backpackId)
        {
            return await context.Reviews.AnyAsync(r => r.UserId == userId && r.BackpackId == backpackId);
        }

        public async Task<ReviewReadDto> CreateReviewAsync(ReviewCreateDto dto, int userId, string userName)
        {
            var review = new Review { Comment = dto.Comment, BackpackId = dto.BackpackId, UserId = userId, CreatedAt = DateTime.UtcNow };
            var rating = new Rating { Value = dto.Rating, UserId = userId, BackpackId = dto.BackpackId };

            context.Reviews.Add(review);
            context.Ratings.Add(rating);
            await context.SaveChangesAsync();

            return new ReviewReadDto { Id = review.Id, Comment = review.Comment, Rating = rating.Value, CreatedAt = review.CreatedAt, BackpackId = review.BackpackId, Username = userName };
        }

        public async Task<bool> UpdateReviewAsync(int reviewId, ReviewCreateDto dto, int userId)
        {
            var review = await context.Reviews.FindAsync(reviewId);
            if (review == null || review.UserId != userId)
            {
                return false;
            }

            review.Comment = dto.Comment;
            var rating = await context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.BackpackId == review.BackpackId);
            if (rating != null)
            {
                rating.Value = dto.Rating;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await context.Reviews.FindAsync(reviewId);
            if (review == null || review.UserId != userId)
            {
                return false;
            }

            var rating = await context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.BackpackId == review.BackpackId);
            if (rating != null)
            {
                context.Ratings.Remove(rating);
            }

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            return true;
        }

    }
}
