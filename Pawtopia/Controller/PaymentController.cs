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
            if (request == null) return BadRequest("Dữ liệu rỗng");
            if (string.IsNullOrEmpty(request.AddressId)) return BadRequest("Thiếu AddressId");
            if (request.Items == null || !request.Items.Any()) return BadRequest("Giỏ hàng trống");

            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists) return BadRequest($"User ID {request.UserId} không tồn tại");

                var addressExists = await _context.Addresses.AnyAsync(a => a.Id == request.AddressId);
                if (!addressExists) return BadRequest($"Địa chỉ ID {request.AddressId} không có trong Database");
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    bool isBanking = request.PaymentMethod?.ToLower() == "banking";
                    var order = new Order
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = request.UserId,
                        AddressId = request.AddressId,
                        CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Status = 1,
                        IsPaid = isBanking ? 1 : 0,
                        PaymentStatus = isBanking ? "Paid via Banking" : "COD Pending"
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    foreach (var item in request.Items)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                        if (product == null) throw new Exception($"Sản phẩm {item.ProductId} không tồn tại");
                        var orderItem = new OrderItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = order.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            Price = (long)item.Price
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
                    var realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return StatusCode(500, new { success = false, message = realError });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}