using BackpackStoreFS.Models.DTOs;

namespace BackpackStoreFS.ServiceContracts
{
    public interface IBasketService
    {
        Task<IEnumerable<BasketItemReadDto>> GetUserBasketAsync(int userId);
        Task AddToBasket(BasketItemCreateDto dto);
        Task UpdateQuantityAsync(int userId, int backpackId, int quantity);
        Task RemoveItemAsync(int userId, int backpackId);
    }
}
