using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public interface IBasketService
    {
        Task<IEnumerable<BasketItemReadDto>> GetUserBasketAsync(string userId);
        Task AddToBasket(BasketItemCreateDto dto);
        Task UpdateQuantityAsync(int id, int quantity);
        Task RemoveItemAsync(int id);
    }

    public class BasketService(BackpackContext context) : IBasketService
    {
        private readonly BackpackContext _context = context;

        public async Task<IEnumerable<BasketItemReadDto>> GetUserBasketAsync(string userId)
        {
            return await _context.BasketsItems
                .Where(x => x.UserId == userId)
                .Include(b => b.Backpack)
                .ThenInclude(p => p.Category)
                .Select(b => new BasketItemReadDto
                {
                    Id = b.Id,
                    BackpackId = b.BackpackId,
                    Name = b.Backpack!.Name,
                    Price = b.Backpack.Price,
                    SalePrice = b.Backpack.SalePrice,
                    Image = b.Backpack.Image,
                    Quantity = b.Quantity,
                    CategoryName = b.Backpack.Category != null ? b.Backpack.Category.Name : "General"
                })
                .ToListAsync();
        }

        public async Task AddToBasket(BasketItemCreateDto dto)
        {
            var existingItem = await _context.BasketsItems
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
                await _context.BasketsItems.AddAsync(newItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int id, int quantity)
        {
            var item = await _context.BasketsItems.FirstOrDefaultAsync(x => x.Id == id);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    _context.BasketsItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                    _context.Entry(item).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveItemAsync(int id)
        {
            var item = await _context.BasketsItems.FindAsync(id);
            if (item != null)
            {
                _context.BasketsItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
