using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryReadDto>> GetAllAsync();
    }

    public class CategoryService(BackpackContext context) : ICategoryService
    {

        public async Task<IEnumerable<CategoryReadDto>> GetAllAsync()
        {
            return await context.Categories
                .Select(c => new CategoryReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();
        }
    }
}
