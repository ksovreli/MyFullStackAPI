using BackpackStoreFS.Models.Entities;

namespace BackpackStoreFS.ServiceContracts
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(int userId, string shippingAddress);
        Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId, int userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> UpdateStatusAsync(int orderId, string status);
    }
}
