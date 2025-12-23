using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.DTOs.Payment;
using Pawtopia.Models;

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
            if (request == null) return BadRequest("Request null");
            if (request.Items == null || request.Items.Count == 0) return BadRequest("Giỏ hàng trống");

            // Check User tồn tại (như đã sửa trước đó)
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists) return BadRequest("User không tồn tại");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Tạo Address
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
                await _context.SaveChangesAsync();

                // 2. Tạo Order - SỬA ĐOẠN NÀY
                // Kiểm tra phương thức thanh toán
                bool isBanking = request.PaymentMethod == "Banking";

                var order = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    AddressId = address.Id,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),

                    Status = 1, // 1: Chờ xử lý (hoặc Chờ xác nhận)

                    // Nếu là Banking thì coi như Đã thanh toán (IsPaid = 1)
                    IsPaid = isBanking ? 1 : 0,
                    PaymentStatus = isBanking ? "Paid" : "COD"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 3. OrderItems + Trừ kho
                foreach (var item in request.Items)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product == null) throw new Exception($"Product {item.ProductId} not found");
                    if (product.QuantityInStock < item.Quantity) throw new Exception($"Hết hàng: {product.Name}");

                    product.QuantityInStock -= item.Quantity;

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = order.Id,
                        ProductItemId = product.Id,
                        Quantity = item.Quantity
                    };
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { success = true, orderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Giữ nguyên các API history/all nếu có...
    }
}