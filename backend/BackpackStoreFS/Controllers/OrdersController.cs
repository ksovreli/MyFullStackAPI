using BackpackStoreFS.Models.DTOs;
using BackpackStoreFS.Models.Entities;
using BackpackStoreFS.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackpackStoreFS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        [HttpPost("checkout")]
        public async Task<ActionResult<Order>> Checkout([FromBody] CheckoutRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("sub")?.Value;

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User ID is missing or invalid.");
            }

            if (string.IsNullOrEmpty(request.ShippingAddress))
            {
                return BadRequest("Shipping address is required.");
            }

            var order = await orderService.CreateOrderAsync(userId, request.ShippingAddress);

            if (order == null)
            {
                return BadRequest("Failed to create order. Your basket might be empty or items are out of stock.");
            }

            return Ok(order);
        }

        [HttpGet("my-history")]
        public async Task<ActionResult<IEnumerable<Order>>> GetMyHistory()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                   ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Invalid user ID in token.");
            }

            var orders = await orderService.GetUserOrderHistoryAsync(userId);

            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var success = await orderService.UpdateStatusAsync(id, status);
            return success ? Ok(new { message = $"Order {id} updated to {status}" }) : NotFound();
        }
    }
}
