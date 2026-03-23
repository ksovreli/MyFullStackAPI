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
        Task<IEnumerable<BackpackReadDto>> GetFilteredAsync(string? category, string? sortBy);
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

        public async Task<IEnumerable<BackpackReadDto>> GetFilteredAsync(string? category, string? sortBy)
        {
            var query = _context.Backpacks
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "All Collections")
            {
                if (category == "New Arrivals")
                {
                    query = query.Where(p => p.IsNew);
                }
                else
                {
                    query = query.Where(p => p.Category != null && p.Category.Name == category);
                }
            }

            query = sortBy switch
            {
                "Price: Low to High" => query.OrderBy(p => p.SalePrice ?? p.Price),
                "Price: High to Low" => query.OrderByDescending(p => p.SalePrice ?? p.Price),
                "Top Rated" => query.OrderByDescending(p => p.Rating),
                "Newest" => query.OrderByDescending(p => p.IsNew),
                "Recommended" => query.OrderByDescending(p => p.IsNew).ThenByDescending(p => p.Rating),
                _ => query.OrderBy(p => p.Id)
            };

            // 4. Map to DTO
            return await query.Select(p => new BackpackReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                SalePrice = p.SalePrice,
                Rating = p.Rating,
                IsNew = p.IsNew,
                CategoryName = p.Category != null ? p.Category.Name : "General",
                Image = p.Image
            }).ToListAsync();
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
