using BackpackStoreFS.Constants;
using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public class BasketService(BackpackContext context) : IBasketService
    {
        public async Task<IEnumerable<BasketItemReadDto>> GetUserBasketAsync(int userId)
        {
            return await context.BasketsItems
                .Where(x => x.UserId == userId)
                .Include(b => b.Backpack)
                .ThenInclude(p => p!.Category)
                .Select(b => new BasketItemReadDto
                {
                    Id = b.Id,
                    BackpackId = b.BackpackId,
                    Name = b.Backpack!.Name,
                    Price = b.Backpack.Price,
                    SalePrice = b.Backpack.SalePrice,
                    Image = b.Backpack.Images.Select(i => i.Url).FirstOrDefault() ?? "/images/placeholder.png",
                    Quantity = b.Quantity,
                    CategoryName = b.Backpack.Category != null ? b.Backpack.Category.Name : BackpackConstants.Reviews.DefaultCategory
                })
                .ToListAsync();
        }

        public async Task AddToBasket(BasketItemCreateDto dto)
        {
            var existingItem = await context.BasketsItems
                .FirstOrDefaultAsync(b => b.UserId == dto.UserId && b.BackpackId == dto.BackpackId);

            if (existingItem != null)
            {
                existingItem.Quantity += (int)dto.Quantity;
            }
            else
            {
                var newItem = new BasketItem
                {
                    UserId = dto.UserId,
                    BackpackId = dto.BackpackId,
                    Quantity = (int)dto.Quantity
                };
                await context.BasketsItems.AddAsync(newItem);
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int userId, int backpackId, int quantity)
        {
            var item = await context.BasketsItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BackpackId == backpackId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    context.BasketsItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                    context.Entry(item).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();
            }
        }
        public async Task RemoveItemAsync(int userId, int backpackId)
        {
            var item = await context.BasketsItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.BackpackId == backpackId);

            if (item != null)
            {
                context.BasketsItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
