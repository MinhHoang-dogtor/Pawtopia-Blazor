using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;

namespace Pawtopia.Controllers
{
    [Route("api/admin/orders")]
    [ApiController]
    public class AdminOrdersController : ControllerBase
    {
        private readonly PawtopiaDbContext _context;

        public AdminOrdersController(PawtopiaDbContext context)
        {
            _context = context;
        }

        // 1. API Lấy danh sách đơn hàng
        [HttpGet]
        public async Task<IActionResult> GetOrders(string? fromDate, string? toDate)
        {
            var query = _context.Orders.Include(o => o.User).AsQueryable();

            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fDate))
                query = query.Where(o => o.CreatedAt >= fDate);

            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var tDate))
                query = query.Where(o => o.CreatedAt < tDate.AddDays(1));

            var result = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new {
                    Id = o.Id,
                    AccountName = o.User != null ? o.User.DisplayName : "Khách ẩn danh",
                    Total = o.OrderItems.Sum(oi => (decimal)(oi.ProductItem != null ? oi.ProductItem.Price : 0) * oi.Quantity),
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.ToString(),
                    PaymentStatus = o.PaymentStatus
                })
                .ToListAsync();

            return Ok(result);
        }

        // 2. API Lấy chi tiết đơn hàng
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(string id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.ProductItem).ThenInclude(pi => pi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound("Không tìm thấy đơn hàng");

            return Ok(new
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus,
                IsPaid = order.IsPaid,
                CustomerName = order.Address != null ? order.Address.FullName : (order.User?.DisplayName ?? "Khách hàng"),
                CustomerPhone = order.Address != null ? order.Address.PhoneNumber : (order.User?.PhoneNumber ?? "Chưa có SĐT"),
                ShippingAddress = order.Address != null ? $"{order.Address.AddressLine}, {order.Address.Ward}, {order.Address.Province}" : "Chưa có địa chỉ",
                Items = order.OrderItems.Select(oi => new {
                    ProductName = oi.ProductItem?.Product?.Name ?? "Sản phẩm",
                    Price = oi.ProductItem?.Price ?? 0,
                    Quantity = oi.Quantity,
                    Total = (decimal)(oi.ProductItem?.Price ?? 0) * oi.Quantity,
                    Image = oi.ProductItem?.ThumbImageLink
                }).ToList(),
                TotalAmount = order.OrderItems.Sum(oi => (decimal)(oi.ProductItem?.Price ?? 0) * oi.Quantity)
            });
        }

        // 3. API LƯU TRẠNG THÁI (CHỈ GIỮ LẠI DUY NHẤT 1 HÀM NÀY)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (Enum.TryParse<OrderStatus>(newStatus, out var statusEnum))
            {
                order.Status = statusEnum;
                await _context.SaveChangesAsync(); // Lưu thực tế vào Database
                return Ok();
            }

            return BadRequest("Trạng thái không hợp lệ");
        }
    }
}