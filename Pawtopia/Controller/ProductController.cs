using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.Client.DTOs;

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

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GetProduct>>> GetAll()
        {
            // Fix lỗi CS0103 bằng cách đảm bảo code nằm trong hàm
            var products = await _context.Products
                .Select(p => new GetProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    ThumbImageLink = p.ThumbImageLink, // Dùng đúng tên ThumbImageLink
                    CategoryId = p.CategoryId,
                    QuantityInStock = p.QuantityInStock,
                    IsActive = p.IsActive
                }).ToListAsync();

            return Ok(products);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreateProduct dto)
        {
            if (dto == null) return BadRequest();

            // Fix lỗi CS0103 cho biến dto
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
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

            return Ok(new { message = "Thêm thành công!" });
        }
    }
}