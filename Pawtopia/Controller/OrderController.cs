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

        // 1. API Lấy danh sách đơn hàng của User
        // GET: api/order/history/{userId}
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetOrderHistory(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Join sang Product để lấy giá tính tổng tiền
                .OrderByDescending(o => o.CreatedAt) // Mới nhất lên đầu
                .ToListAsync();

            if (!orders.Any())
            {
                return Ok(new List<OrderHistoryDto>()); // Trả về mảng rỗng nếu chưa mua gì
            }

            var result = orders.Select(o => new OrderHistoryDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt,
                Status = MapStatus(o.Status),
                PaymentStatus = o.PaymentStatus ?? "Chưa thanh toán",
                TotalItems = o.OrderItems.Count,
                // Tính tổng tiền = Tổng (Số lượng * Giá Product)
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * (oi.Product?.Price ?? 0))
            }).ToList();

            return Ok(result);
        }

        // 2. API Lấy chi tiết một đơn hàng cụ thể
        // GET: api/order/detail/{orderId}
        [HttpGet("detail/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Address) // Lấy địa chỉ giao hàng
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Lấy thông tin sản phẩm
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

                // Thông tin người nhận từ bảng Address
                ReceiverName = order.Address?.FullName ?? "N/A",
                ReceiverPhone = order.Address?.PhoneNumber ?? "N/A",
                ShippingAddress = GetFullAddress(order.Address),

                // Map danh sách sản phẩm
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductItemId,
                    ProductName = oi.Product?.Name ?? "Sản phẩm đã bị xóa",
                    ImageUrl = oi.Product?.ThumbImageLink ?? "",
                    Quantity = (int)oi.Quantity,
                    Price = oi.Product?.Price ?? 0
                }).ToList()
            };

            // Tính lại tổng tiền cho chính xác để hiển thị
            result.TotalAmount = result.Items.Sum(x => x.SubTotal);
            result.TotalItems = result.Items.Count;

            return Ok(result);
        }

        // --- Helper Methods ---

        // Hàm chuyển đổi trạng thái từ số sang chữ
        private string MapStatus(long status)
        {
            return status switch
            {
                1 => "Chờ xác nhận",
                2 => "Đang đóng gói",
                3 => "Đang giao hàng",
                4 => "Đã giao thành công",
                0 => "Đã hủy",
                _ => "Không xác định"
            };
        }

        // Hàm ghép chuỗi địa chỉ
        private string GetFullAddress(Pawtopia.Models.Address? addr)
        {
            if (addr == null) return "Không có địa chỉ";
            return $"{addr.DetailAddress}, {addr.Ward}, {addr.District}, {addr.Province}";
        }
    }
}