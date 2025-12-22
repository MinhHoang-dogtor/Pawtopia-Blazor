using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.DTOs.Payment;
using Pawtopia.Models;
using System;
using System.Threading.Tasks;

namespace Pawtopia.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PawtopiaDbContext _context;

        public PaymentController(PawtopiaDbContext context)
        {
            _context = context;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] PaymentRequestDto request)
        {
            // 1. Kiểm tra dữ liệu đầu vào xem có bị null không
            if (request == null)
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên bị null" });

            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(new { success = false, message = "Giỏ hàng trống" });

            // 2. [QUAN TRỌNG] Kiểm tra UserId có tồn tại trong bảng Users không
            // Lỗi SQLite 19 thường do UserId bên FE gửi lên không khớp với bất kỳ ai trong DB
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Lỗi: Người dùng có ID '{request.UserId}' không tồn tại trong Database. Vui lòng đăng xuất và đăng ký lại tài khoản mới."
                });
            }

            // Bắt đầu giao dịch (Transaction)
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 3. Tạo và lưu Address trước
                var address = new Address
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    DetailAddress = request.AddressLine,
                    Ward = request.Ward,
                    Province = request.Province,
                    District = request.District,
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync(); // Lưu để lấy AddressId

                // 4. Tạo Order
                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    AddressId = address.Id,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = 1,
                    IsPaid = 0,
                    PaymentStatus = "COD"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu Order

                // 5. Tạo OrderItems và trừ kho
                foreach (var item in request.Items)
                {
                    // Tìm sản phẩm trong DB
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    if (product == null)
                        throw new Exception($"Sản phẩm ID {item.ProductId} không tồn tại");

                    if (product.QuantityInStock < item.Quantity)
                        throw new Exception($"Sản phẩm '{product.Name}' không đủ hàng (còn {product.QuantityInStock})");

                    // Trừ số lượng tồn kho
                    product.QuantityInStock -= item.Quantity;

                    // Tạo chi tiết đơn hàng
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        ProductItemId = product.Id,
                        Quantity = item.Quantity
                    };

                    _context.OrderItems.Add(orderItem);
                }

                // Lưu tất cả thay đổi còn lại (OrderItems và update Product)
                await _context.SaveChangesAsync();

                // Xác nhận giao dịch thành công
                await transaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    orderId = order.Id,
                    message = "Đặt hàng thành công!"
                });
            }
            catch (Exception ex)
            {
                // Nếu có bất kỳ lỗi gì thì hoàn tác (không lưu rác vào DB)
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}