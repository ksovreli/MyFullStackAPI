using BackpackStoreFS.Data;
using BackpackStoreFS.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<Backpack>> GetWishlistAsync(string userId);
        Task<string> ToggleWishlistAsync(WishlistItem item);
    }

    public class WishlistService(BackpackContext _context) : IWishlistService
    {
        public async Task<IEnumerable<Backpack>> GetWishlistAsync(string userId)
        {
            var backpackIds = await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Select(w => w.BackpackId)
                .ToListAsync();

            return await _context.Backpacks
                .Include(b => b.Category)
                .Where(b => backpackIds.Contains(b.Id))
                .ToListAsync();
        }

        public async Task<string> ToggleWishlistAsync(WishlistItem dto)
        {
            var existing = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == dto.UserId && w.BackpackId == dto.BackpackId);

            if (existing != null)
            {
                _context.WishlistItems.Remove(existing);
                await _context.SaveChangesAsync();
                return "removed";
            }

            _context.WishlistItems.Add(dto);
            await _context.SaveChangesAsync();
            return "added";
        }
    }
}
