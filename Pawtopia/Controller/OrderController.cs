using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.DTOs.Order;
using Pawtopia.Models;

namespace Pawtopia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly PawtopiaDbContext _context;

        public OrderController(PawtopiaDbContext context)
        {
            _context = context;
        }

        // 1. API Lấy danh sách đơn hàng của User (Lịch sử mua hàng)
        // GET: api/order/history/{userId}
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetOrderHistory(string userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return Ok(new List<OrderHistoryDto>());
                }

                var result = orders.Select(o => new OrderHistoryDto
                {
                    OrderId = o.Id,
                    CreatedAt = o.CreatedAt ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = MapStatus(o.Status),
                    PaymentStatus = o.PaymentStatus ?? "Chưa thanh toán",
                    TotalItems = o.OrderItems?.Count ?? 0,
                    TotalAmount = o.OrderItems?.Sum(oi => oi.Quantity * (oi.Product?.Price ?? 0)) ?? 0
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi lấy lịch sử", error = ex.Message });
            }
        }

        // 2. API Lấy chi tiết một đơn hàng cụ thể
        // GET: api/order/detail/{orderId}
        [HttpGet("detail/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng." });
            }

            var result = new OrderDetailDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = MapStatus(order.Status),
                PaymentStatus = order.PaymentStatus ?? "Chưa thanh toán",
                ReceiverName = order.Address?.FullName ?? "N/A",
                ReceiverPhone = order.Address?.PhoneNumber ?? "N/A",
                ShippingAddress = GetFullAddress(order.Address),
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductItemId,
                    ProductName = oi.Product?.Name ?? "Sản phẩm đã bị xóa",
                    ImageUrl = oi.Product?.ThumbImageLink ?? "",
                    Quantity = (int)oi.Quantity,
                    Price = oi.Product?.Price ?? 0
                }).ToList()
            };

            result.TotalAmount = result.Items.Sum(x => x.SubTotal);
            result.TotalItems = result.Items.Count;

            return Ok(result);
        }

        // 3. API Lấy TOÀN BỘ danh sách đơn hàng (Dành cho Admin)
        // GET: api/order/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                // Lấy dữ liệu từ DB (Include đầy đủ)
                var orders = await _context.Orders
                    .Include(o => o.Address)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return Ok(new List<AdminOrderDto>());
                }

                // Map dữ liệu sang DTO an toàn (tránh lỗi Null)
                var result = orders.Select(o => new AdminOrderDto
                {
                    OrderId = o.Id,
                    UserId = o.UserId ?? "",
                    CreatedAt = o.CreatedAt ?? "N/A",
                    Status = MapStatus(o.Status),
                    PaymentStatus = o.PaymentStatus ?? "Chưa thanh toán",

                    // Xử lý null cho Address
                    CustomerName = o.Address?.FullName ?? "Khách vãng lai",
                    PhoneNumber = o.Address?.PhoneNumber ?? "N/A",

                    // Xử lý null cho OrderItems
                    TotalItems = o.OrderItems?.Count ?? 0,
                    TotalAmount = o.OrderItems?.Sum(oi => oi.Quantity * (oi.Product?.Price ?? 0)) ?? 0
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Trả về lỗi chi tiết để debug
                return StatusCode(500, new { message = "Lỗi Server khi lấy danh sách đơn", detail = ex.ToString() });
            }
        }

        // 4. API Cập nhật trạng thái đơn hàng (Dành cho Admin duyệt/hủy)
        // PUT: api/order/update-status/{orderId}
        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] int newStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng");

            // Cập nhật trạng thái (1: Chờ, 2: Giao, 3: Xong, -1: Hủy)
            order.Status = newStatus;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Cập nhật trạng thái thành công" });
        }

        // --- Helper Methods ---

        private string MapStatus(long status)
        {
            return status switch
            {
                1 => "Chờ xác nhận",
                2 => "Đang giao hàng",
                3 => "Hoàn thành",
                4 => "Đã giao thành công",
                0 => "Đã hủy",
                -1 => "Đã hủy", // Support cả -1
                _ => "Không xác định"
            };
        }

        private string GetFullAddress(Pawtopia.Models.Address? addr)
        {
            if (addr == null) return "Không có địa chỉ";
            return $"{addr.DetailAddress}, {addr.Ward}, {addr.District}, {addr.Province}";
        }
    }
}