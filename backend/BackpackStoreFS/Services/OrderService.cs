using BackpackStoreFS.Data;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Services
{
    public class OrderService(BackpackContext context) : IOrderService
    {
        public async Task<Order?> CreateOrderAsync(int userId, string shippingAddress)
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var basketItems = await context.BasketsItems
                    .Include(b => b.Backpack)
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                if (basketItems == null || !basketItems.Any())
                {
                    return null;
                }

                var order = new Order
                {
                    UserId = userId,
                    ShippingAddress = shippingAddress,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    OrderItems = new List<OrderItem>()
                };

                decimal calculatedTotal = 0;

                foreach (var item in basketItems)
                {
                    if (item.Backpack == null)
                    {
                        continue;
                    }

                    if (item.Backpack.Quantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }

                    decimal currentPrice = item.Backpack.SalePrice ?? item.Backpack.Price;

                    order.OrderItems.Add(new OrderItem
                    {
                        BackpackId = item.BackpackId,
                        Quantity = item.Quantity,
                        PriceAtPurchase = currentPrice
                    });

                    item.Backpack.Quantity -= item.Quantity;

                    calculatedTotal += (currentPrice * item.Quantity);
                }

                order.TotalPrice = calculatedTotal;

                context.Orders.Add(order);

                context.BasketsItems.RemoveRange(basketItems);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
        public async Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId)
        {
            return await context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Backpack)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId, int userId)
        {
            return await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string status)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await context.SaveChangesAsync();
            return true;
        }
    }
}

