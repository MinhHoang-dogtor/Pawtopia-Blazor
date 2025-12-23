using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("history/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderHistory(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            if (orders == null || !orders.Any()) return Ok(new List<OrderSummaryDto>());

            var result = orders.Select(o => new OrderSummaryDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                Status = o.Status,
                StatusName = MapStatus(o.Status),
                PaymentStatus = o.PaymentStatus ?? "Chưa thanh toán",
                TotalItems = o.OrderItems.Count,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.Price)
            });

            return Ok(result);
        }
        [HttpGet("detail/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });

            var result = new OrderDetailDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt ?? "N/A",
                Status = order.Status,
                StatusName = MapStatus(order.Status),
                PaymentStatus = order.PaymentStatus ?? "N/A",
                ReceiverName = order.Address?.FullName ?? "N/A",
                ReceiverPhone = order.Address?.PhoneNumber ?? "N/A",
                ShippingAddress = GetFullAddress(order.Address),
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Sản phẩm đã bị xóa",
                    ImageUrl = oi.Product?.ThumbImageLink,
                    Quantity = (int)oi.Quantity,
                    Price = (double)oi.Price
                }).ToList()
            };

            result.TotalItems = result.Items.Count;
            result.TotalAmount = result.Items.Sum(x => x.SubTotal);

            return Ok(result);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var result = orders.Select(o => new OrderSummaryDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt ?? "N/A",
                Status = o.Status,
                StatusName = MapStatus(o.Status),
                PaymentStatus = o.PaymentStatus ?? "Chưa thanh toán",
                TotalItems = o.OrderItems.Count,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.Price),

                CustomerName = o.Address?.FullName ?? "Khách vãng lai",
                CustomerPhone = o.Address?.PhoneNumber ?? "N/A"
            });

            return Ok(result);
        }
        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateStatus(string orderId, [FromBody] int newStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound("Đơn hàng không tồn tại");

            if (newStatus < -1 || newStatus > 4) return BadRequest("Trạng thái không hợp lệ");

            order.Status = newStatus;
            if (newStatus == 4)
            {
                order.IsPaid = 1;
                order.PaymentStatus = "Đã thanh toán";
            }
            else if (newStatus == 0 || newStatus == -1)
            {
                order.PaymentStatus = "Đã hủy";
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Đã cập nhật trạng thái: {MapStatus(newStatus)}" });
        }
        [HttpPut("update-info/{orderId}")]
        [Authorize(AuthenticationSchemes = "MyCookie", Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderInfo(string orderId, [FromBody] AdminUpdateOrderInfoDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound("Đơn hàng không tồn tại");
            if (order.Address == null)
            {
                var newAddr = new Address { Id = Guid.NewGuid().ToString(), UserId = order.UserId };
                order.Address = newAddr;
                order.AddressId = newAddr.Id;
                _context.Addresses.Add(newAddr);
            }
            order.Address.FullName = dto.ReceiverName;
            order.Address.PhoneNumber = dto.ReceiverPhone;
            order.Address.DetailAddress = dto.DetailAddress;
            order.Address.Ward = dto.Ward;
            order.Address.District = dto.District;
            order.Address.Province = dto.Province;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin nhận hàng thành công!" });
        }
        [HttpDelete("delete/{orderId}")]
        [Authorize(AuthenticationSchemes = "MyCookie", Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(string orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound("Đơn hàng không tồn tại");

            try
            {
                if (order.OrderItems.Any())
                {
                    _context.OrderItems.RemoveRange(order.OrderItems);
                }
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xóa đơn hàng vĩnh viễn!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi xóa", error = ex.Message });
            }
        }


        [HttpGet("admin/filter")]
        public async Task<IActionResult> FilterOrders(
            [FromQuery] int? status,       
            [FromQuery] string? sortBy,   
            [FromQuery] string? sortDir) 
        {
            var query = _context.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }
            bool isDesc = string.IsNullOrEmpty(sortDir) || sortDir.ToLower() == "desc";
            switch (sortBy?.ToLower())
            {
                case "price":
                    if (isDesc)
                        query = query.OrderByDescending(o => o.OrderItems.Sum(oi => oi.Quantity * oi.Price));
                    else
                        query = query.OrderBy(o => o.OrderItems.Sum(oi => oi.Quantity * oi.Price));
                    break;

                case "status":
                    if (isDesc)
                        query = query.OrderByDescending(o => o.Status);
                    else
                        query = query.OrderBy(o => o.Status);
                    break;

                case "date":
                default:
                    if (isDesc)
                        query = query.OrderByDescending(o => o.CreatedAt);
                    else
                        query = query.OrderBy(o => o.CreatedAt);
                    break;
            }
            var orders = await query.ToListAsync();
            var result = orders.Select(o => new OrderSummaryDto
            {
                OrderId = o.Id,
                CreatedAt = o.CreatedAt ?? "N/A",
                Status = o.Status,
                StatusName = MapStatus(o.Status),
                PaymentStatus = o.PaymentStatus ?? "Chưa thanh toán",
                TotalItems = o.OrderItems.Count,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.Price),
                CustomerName = o.Address?.FullName ?? "Khách vãng lai",
                CustomerPhone = o.Address?.PhoneNumber ?? "N/A"
            });

            return Ok(result);
        }
        private string MapStatus(long status)
        {
            return status switch
            {
                1 => "Chờ xác nhận",
                2 => "Đang giao hàng",
                3 => "Giao thành công",
                4 => "Hoàn thành",
                0 => "Đã hủy",
                -1 => "Đã hủy",
                _ => "Không xác định"
            };
        }
        private string GetFullAddress(Address? addr)
        {
            if (addr == null) return "Không có thông tin địa chỉ";
            return $"{addr.DetailAddress}, {addr.Ward}, {addr.District}, {addr.Province}";
        }
    }
}