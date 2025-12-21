using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.DTOs; // <--- SỬA DÒNG NÀY: Dùng DTO của Backend, không dùng của Client

namespace Pawtopia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly PawtopiaDbContext _context;

        public ProductController(PawtopiaDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả sản phẩm
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GetProduct>>> GetAll()
        {
            var products = await _context.Products
                .Select(p => new GetProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    ThumbImageLink = p.ThumbImageLink,
                    CategoryId = p.CategoryId,
                    QuantityInStock = p.QuantityInStock,
                    IsActive = p.IsActive
                }).ToListAsync();

            return Ok(products);
        }

        // Thêm sản phẩm mới
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreateProduct dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var product = new Product
            {
                Id = Guid.NewGuid().ToString(), // Tạo ID tự động
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                ThumbImageLink = dto.ThumbImageLink,
                CategoryId = dto.CategoryId,
                QuantityInStock = dto.QuantityInStock,
                IsActive = dto.IsActive
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm sản phẩm thành công!" });
        }
    }
}