using BackpackStoreFS.Models.DTOs;

namespace BackpackStoreFS.ServiceContracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryReadDto>> GetAllAsync();
    }
}
