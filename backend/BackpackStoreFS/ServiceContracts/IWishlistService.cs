using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;

namespace BackpackStoreFS.ServiceContracts
{
    public interface IWishlistService
    {
        Task<IEnumerable<Backpack>> GetWishlistAsync(int userId);
        Task<string> ToggleWishlistAsync(WishlistDto dto);
    }
}
