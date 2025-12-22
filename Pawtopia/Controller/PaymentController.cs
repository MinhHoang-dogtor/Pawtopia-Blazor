using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.DTOs.Payment;
using Pawtopia.Models;

namespace Pawtopia.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PawtopiaDbContext _context;

        public PaymentController(PawtopiaDbContext context)
        {
            _context = context;
        }

        // POST: api/payment/place-order
        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] PaymentRequestDto request)
        {
            // 1. Validate dữ liệu cơ bản
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest("Giỏ hàng trống, không thể thanh toán.");
            }

            // Dùng Transaction để đảm bảo tính toàn vẹn (tạo đơn + trừ kho cùng lúc)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. Tạo địa chỉ giao hàng (Address)
                // Vì bảng Orders yêu cầu AddressId, ta phải tạo Address trước
                var newAddress = new Address
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    DetailAddress = request.AddressLine,
                    Ward = request.Ward,
                    Province = request.Province,
                    UserId = request.UserId // Gắn địa chỉ này với User đó
                };
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

                // 3. Tạo Đơn hàng (Order)
                var newOrder = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    AddressId = newAddress.Id,
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = 1, // 1 = Mới tạo / Chờ xử lý
                    IsPaid = 0, // 0 = Chưa thanh toán
                    PaymentStatus = "COD", // Mặc định là COD, nếu online thì update sau
                    // TotalAmount sẽ tính sau khi duyệt qua các sản phẩm
                };

                // Lưu tạm Order để lấy ID (chưa commit transaction)
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                double totalAmountCalculated = 0;
                var orderItemsList = new List<OrderItem>();

                // 4. Duyệt qua từng sản phẩm để tạo OrderItem và Trừ kho
                foreach (var itemDto in request.Items)
                {
                    // Lấy thông tin sản phẩm thực tế từ DB để check tồn kho và giá
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == itemDto.ProductId);

                    if (product == null)
                    {
                        throw new Exception($"Sản phẩm ID {itemDto.ProductId} không tồn tại.");
                    }

                    if (product.QuantityInStock < itemDto.Quantity)
                    {
                        throw new Exception($"Sản phẩm '{product.Name}' không đủ hàng. Tồn kho: {product.QuantityInStock}");
                    }

                    // Trừ tồn kho
                    product.QuantityInStock -= itemDto.Quantity;

                    // Tạo OrderItem
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = newOrder.Id,
                        ProductItemId = product.Id,
                        Quantity = itemDto.Quantity,
                        // Lưu ý: Có thể thêm trường Price vào bảng OrderItems nếu muốn lưu giá tại thời điểm mua
                    };
                    orderItemsList.Add(orderItem);

                    // Cộng dồn tổng tiền 
                    // Ở đây dùng giá từ DB * số lượng
                    if (product.Price != null)
                    {
                        totalAmountCalculated += (double)product.Price * itemDto.Quantity;
                    }
                }

                // Lưu danh sách OrderItems
                _context.OrderItems.AddRange(orderItemsList);

                // Cập nhật lại sản phẩm (đã trừ kho)
                _context.Products.UpdateRange(
                    request.Items.Select(i => _context.Products.Local.FirstOrDefault(p => p.Id == i.ProductId))
                );

                await _context.SaveChangesAsync();

                // 5. Commit Transaction
                await transaction.CommitAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Đặt hàng thành công",
                    OrderId = newOrder.Id,
                    TotalAmount = totalAmountCalculated
                });
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, rollback lại toàn bộ (không tạo đơn, không trừ kho)
                await transaction.RollbackAsync();
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}