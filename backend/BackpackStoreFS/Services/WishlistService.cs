using BackpackStoreFS.Data;
using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public class WishlistService(BackpackContext context) : IWishlistService
    {
        public async Task<IEnumerable<Backpack>> GetWishlistAsync(int userId)
        {
            var backpackIds = await context.WishlistItems
                .Where(w => w.UserId == userId)
                .Select(w => w.BackpackId)
                .ToListAsync();

            return await context.Backpacks
                .Include(b => b.Category)
                .Include(b => b.Images)
                .Where(b => backpackIds.Contains(b.Id))
                .ToListAsync();
        }

        public async Task<string> ToggleWishlistAsync(WishlistDto dto)
        {
            var existing = await context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == dto.UserId && w.BackpackId == dto.BackpackId);

            if (existing != null)
            {
                context.WishlistItems.Remove(existing);
                await context.SaveChangesAsync();
                return "removed";
            }

            var newItem = new WishlistItem
            {
                UserId = dto.UserId,
                BackpackId = dto.BackpackId,
            };

            context.WishlistItems.Add(newItem);
            await context.SaveChangesAsync();
            return "added";
        }
    }
}
