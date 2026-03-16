using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public interface IBackpackService
    {
        Task<IEnumerable<BackpackReadDto>> GetAllAsync();
        Task<BackpackReadDto?> GetByIdAsync(int id);
        Task<BackpackReadDto> CreateAsync(BackpackCreateDto dto);
        Task<bool> UpdateAsync(int id, BackpackCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class BackpackService(BackpackContext context) : IBackpackService
    {
        private readonly BackpackContext _context = context;

        public async Task<IEnumerable<BackpackReadDto>> GetAllAsync()
        {
            return await _context.Backpacks
                .Include(b => b.Category)
                .Select(b => MapToReadDto(b))
                .ToListAsync();
        }

        public async Task<BackpackReadDto?> GetByIdAsync(int id)
        {
            var backpack = await _context.Backpacks
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            return backpack != null ? MapToReadDto(backpack) : null;
        }

        public async Task<BackpackReadDto> CreateAsync(BackpackCreateDto dto)
        {
            var backpack = new Backpack
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

            _context.Backpacks.Add(backpack);
            await _context.SaveChangesAsync();

            return MapToReadDto(backpack);
        }

        public async Task<bool> UpdateAsync(int id, BackpackCreateDto dto)
        {
            var backpack = await _context.Backpacks.FindAsync(id);
            if (backpack == null)
            {
                return false;
            }

            backpack.Name = dto.Name;
            backpack.Image = dto.ImageUrl;
            backpack.Price = dto.Price;
            backpack.Quantity = dto.Quantity;
            backpack.SalePrice = dto.SalePrice;
            backpack.CategoryId = dto.CategoryId;
            backpack.IsNew = dto.IsNew;
            backpack.Rating = dto.Rating;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var backpack = await _context.Backpacks.FindAsync(id);
            if (backpack == null) return false;

            _context.Backpacks.Remove(backpack);
            await _context.SaveChangesAsync();
            return true;
        }

        private static BackpackReadDto MapToReadDto(Backpack b) => new BackpackReadDto
        {
            Id = b.Id,
            Name = b.Name,
            Image = b.Image,
            Price = b.Price,
            Quantity = b.Quantity,
            SalePrice = b.SalePrice,
            IsNew = b.IsNew,
            Rating = b.Rating,
            CategoryId = b.CategoryId,
            CategoryName = b.Category?.Name ?? "No Category"
        };
    }
}
