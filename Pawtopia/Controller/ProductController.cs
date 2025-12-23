using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;

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
                .Where(p => p.IsActive == 1)
                .Select(p => new GetProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    ThumbImageLink = p.ThumbImageLink,
                    CategoryId = p.CategoryId,
                    QuantityInStock = p.QuantityInStock,
                    IsActive = p.IsActive,
                    ImageUrls = p.ProductImages.Select(img => img.ImageUrl).ToList()
                }).ToListAsync();
            return Ok(products);
        }
        [HttpGet("by-category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetProduct>>> GetByCategoryId(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId)) return BadRequest("Category id is required.");

            var cleanId = categoryId.Trim();
          

            var products = await _context.Products
                .Where(p => p.CategoryId == cleanId)
                .Include(p => p.ProductImages)
                .Select(p => new GetProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    ThumbImageLink = p.ThumbImageLink,
                    CategoryId = p.CategoryId,
                    QuantityInStock = p.QuantityInStock,
                    IsActive = p.IsActive,
                    ImageUrls = p.ProductImages.Select(img => img.ImageUrl).ToList()
                }).ToListAsync();

            return Ok(products);
        }
        [HttpPost("add")]
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
            if (dto.ImageUrls != null && dto.ImageUrls.Count > 0)
            {
                foreach (var url in dto.ImageUrls)
                {
                    var img = new ProductImage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ImageUrl = url,
                        ProductId = product.Id
                    };
                    _context.ProductImages.Add(img);
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm thành công!" });
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProduct dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ.");
            var existingProduct = await _context.Products
                                        .Include(p => p.ProductImages)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });
            existingProduct.Name = dto.Name;
            existingProduct.Price = dto.Price;
            existingProduct.Description = dto.Description;
            existingProduct.ThumbImageLink = dto.ThumbImageLink;
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.QuantityInStock = dto.QuantityInStock;
            existingProduct.IsActive = dto.IsActive;
            if (dto.ImageUrls != null)
            {
                _context.ProductImages.RemoveRange(existingProduct.ProductImages);
                foreach (var url in dto.ImageUrls)
                {
                    _context.ProductImages.Add(new ProductImage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ImageUrl = url,
                        ProductId = existingProduct.Id
                    });
                }
            }

            try
            {
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật thành công!", product = existingProduct });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteForever(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Đã xóa vĩnh viễn sản phẩm!" });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new
                {
                    message = "Không thể xóa vĩnh viễn vì sản phẩm này đã có trong đơn hàng cũ. Hãy dùng chức năng ẩn (IsActive = 0).",
                    detail = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}