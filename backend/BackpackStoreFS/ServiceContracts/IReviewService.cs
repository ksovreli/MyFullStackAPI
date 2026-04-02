using BackpackStoreFS.Models.DTOs;

namespace BackpackStoreFS.ServiceContracts
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewReadDto>> GetReviewsByBackpackAsync(int backpackId);
        Task<bool> HasUserReviewedAsync(int userId, int backpackId);
        Task<ReviewReadDto> CreateReviewAsync(ReviewCreateDto dto, int userId, string userName);
        Task<bool> UpdateReviewAsync(int reviewId, ReviewCreateDto dto, int userId); 
        Task<bool> DeleteReviewAsync(int reviewId, int userId);
    }
}
