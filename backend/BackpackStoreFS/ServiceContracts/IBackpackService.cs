using BackpackStoreFS.Models.DTOs;

namespace BackpackStoreFS.ServiceContracts
{
    public interface IBackpackService
    {
        Task<IEnumerable<BackpackReadDto>> GetAllAsync();
        Task<BackpackReadDto?> GetByIdAsync(int id);
        Task<BackpackReadDto> CreateAsync(BackpackCreateDto dto);
        Task<bool> UpdateAsync(int id, BackpackCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<BackpackReadDto>> GetFilteredAsync(string? category, string? sortBy);
    }
}
