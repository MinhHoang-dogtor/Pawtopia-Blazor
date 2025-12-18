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

        // 1. Lấy danh sách (Trang Admin)
        [HttpGet]
        public async Task<IActionResult> GetOrders(string? fromDate, string? toDate)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var fDate))
                query = query.Where(o => o.CreatedAt >= fDate);

            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var tDate))
                query = query.Where(o => o.CreatedAt < tDate.AddDays(1));

            var result = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    Id = o.Id,
                    AccountName = o.User != null ? o.User.DisplayName : "Khách ẩn danh",
                    CustomerName = o.User != null ? o.User.UserName : "No Email",
                    Total = o.OrderItems.Sum(oi => oi.Quantity * oi.ProductItem.Price),
                    CreatedAt = o.CreatedAt,
                    Status = o.Status.ToString(),
                    PaymentStatus = o.PaymentStatus
                })
                .ToListAsync();

            return Ok(result);
        }

        // 2. Lấy chi tiết (Trang OrderDetail)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(string id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductItem) // Lấy ProductItem
                        .ThenInclude(pi => pi.Product) // Lấy tiếp Product để có Tên
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound("Không tìm thấy đơn hàng");

            // Xử lý địa chỉ
            string fullAddress = "Chưa có địa chỉ";
            if (order.Address != null)
            {
                // Ghép địa chỉ từ Address.cs
                fullAddress = $"{order.Address.AddressLine}, {order.Address.Ward}, {order.Address.Province}";
            }

            var result = new
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus,
                IsPaid = order.IsPaid,
                CustomerName = order.User != null ? order.User.DisplayName : "Khách vãng lai",
                CustomerEmail = order.User != null ? order.User.Email : "",
                CustomerPhone = order.User != null ? order.User.PhoneNumber : "",
                ShippingAddress = fullAddress,

                Items = order.OrderItems.Select(oi => new
                {
                    // Lấy tên từ Product cha
                    ProductName = oi.ProductItem.Product != null ? oi.ProductItem.Product.Name : "Sản phẩm (Lỗi tên)",

                    Price = oi.ProductItem.Price,
                    Quantity = oi.Quantity,
                    Total = oi.ProductItem.Price * oi.Quantity,

                    // --- SỬA LỖI TẠI ĐÂY ---
                    // Đổi ImageUrl thành ThumbImageLink (khớp với ProductItem.cs)
                    Image = oi.ProductItem.ThumbImageLink
                }).ToList(),

                TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.ProductItem.Price)
            };

            return Ok(result);
        }
    }
}