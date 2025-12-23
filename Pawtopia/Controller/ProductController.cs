using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies; // Thêm cái này

namespace Pawtopia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly PawtopiaDbContext _context;
        public ProductController(PawtopiaDbContext context) => _context = context;

        [HttpGet("all")]
        [AllowAnonymous]
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

        [HttpGet("by-category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetProduct>>> GetByCategoryId(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId)) return BadRequest("Category id is required.");

            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
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

        [HttpPost("add")]
        // SỬA Ở ĐÂY: Chỉ định rõ dùng MyCookie để kiểm tra Role Admin
        [Authorize(AuthenticationSchemes = "MyCookie", Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CreateProduct dto)
        {
            if (dto == null) return BadRequest();
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