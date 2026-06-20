using BackpackStoreFS.Constants;
using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public class BackpackService(BackpackContext context) : IBackpackService
    {
        public async Task<IEnumerable<BackpackReadDto>> GetAllAsync()
        {
            var backpacks = await context.Backpacks
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Include(b => b.Images)
                .Include(b => b.Ratings)
                .ToListAsync();

            return backpacks.Select(b => MapToReadDto(b));
        }

        public async Task<BackpackReadDto?> GetByIdAsync(int id)
        {
            var backpack = await context.Backpacks
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .Include(b => b.Images)
                .Include(b => b.Ratings)
                .FirstOrDefaultAsync(b => b.Id == id);

            return backpack != null ? MapToReadDto(backpack) : null;
        }

        public async Task<BackpackReadDto> CreateAsync(BackpackCreateDto dto)
        {
            var backpack = new Backpack(dto);

            context.Backpacks.Add(backpack);
            await context.SaveChangesAsync();

            return MapToReadDto(backpack);
        }

        public async Task<bool> UpdateAsync(int id, BackpackCreateDto dto)
        {
            var backpack = await context.Backpacks
                .Include(b => b.Images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (backpack == null) return false;

            backpack.Name = dto.Name;
            backpack.Description = dto.Description;
            backpack.Price = dto.Price;
            backpack.Quantity = dto.Quantity;
            backpack.CategoryId = dto.CategoryId;
            backpack.IsNew = dto.IsNew;
            backpack.SalePrice = dto.SalePrice;

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                var currentImage = backpack.Images.FirstOrDefault();
                if (currentImage == null)
                {
                    backpack.Images.Add(new BackpackImage { Url = dto.ImageUrl });
                }
                else if (currentImage.Url != dto.ImageUrl)
                {
                    currentImage.Url = dto.ImageUrl;
                }
            }

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var backpack = await context.Backpacks.FindAsync(id);
            if (backpack == null) return false;

            context.Backpacks.Remove(backpack);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BackpackReadDto>> GetFilteredAsync(string? category, string? sortBy)
        {
            var query = context.Backpacks
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.Images)
                .Include(p => p.Ratings)
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

                "Top Rated" => query.OrderByDescending(p => p.Ratings.Any() ? p.Ratings.Average(r => (decimal)r.Value) : 0),

                "Newest" => query.OrderByDescending(p => p.IsNew),

                "Recommended" => query.OrderByDescending(p => p.IsNew)
                        .ThenByDescending(p => p.Ratings.Any() ? p.Ratings.Average(r => (decimal)r.Value) : 0),
                _ => query.OrderBy(p => p.Id)
            };

            var results = await query.ToListAsync();

            return await query.Select(p => new BackpackReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                SalePrice = p.SalePrice,
                IsNew = p.IsNew,

                Image = p.Images.Select(img => img.Url).FirstOrDefault() ?? "placeholder.png",

                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : BackpackConstants.Reviews.DefaultCategory,

                Rating = p.Ratings.Any()
                         ? (decimal)Math.Round(p.Ratings.Average(r => (decimal)r.Value), 1)
                         : 0,

                ReviewCount = p.Reviews.Count

            }).ToListAsync();
        }
        private static BackpackReadDto MapToReadDto(Backpack b) => new BackpackReadDto
        {
            Id = b.Id,
            Name = b.Name,
            Image = b.Images.FirstOrDefault()?.Url ?? "placeholder.png",
            Price = b.Price,
            Description = b.Description,
            Quantity = b.Quantity,
            SalePrice = b.SalePrice,
            IsNew = b.IsNew,
            CategoryId = b.CategoryId,
            CategoryName = b.Category?.Name ?? BackpackConstants.Reviews.NoComment,

            Rating = b.Ratings != null && b.Ratings.Any()
                             ? (decimal)Math.Round(b.Ratings.Average(r => r.Value), 1)
                             : 0,

            ReviewCount = b.Reviews?.Count ?? 0
        };
    }
}
