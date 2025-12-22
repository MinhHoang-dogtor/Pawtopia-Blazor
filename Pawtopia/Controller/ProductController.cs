using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.DTOs;

namespace Pawtopia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly PawtopiaDbContext _context;
        public ProductController(PawtopiaDbContext context) => _context = context;

        // --- 1.1. LẤY TẤT CẢ SẢN PHẨM (Khách xem) ---
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<GetProduct>>> GetAll()
        {
            // Trả về dữ liệu gốc, bỏ trường FinalPrice
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
                    IsActive = p.IsActive,
                    IsDiscount = p.IsDiscount,
                    SaleDiscount = p.SaleDiscount
                }).ToListAsync();
            return Ok(products);
        }

        // --- 1.2. THÊM SẢN PHẨM MỚI (Admin) ---
        [HttpPost("add")]
        [Authorize(AuthenticationSchemes = "MyCookie", Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CreateProduct dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                ThumbImageLink = dto.ThumbImageLink,
                CategoryId = dto.CategoryId,
                QuantityInStock = dto.QuantityInStock,
                IsActive = dto.IsActive,
                IsDiscount = dto.IsDiscount,
                SaleDiscount = dto.SaleDiscount
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm sản phẩm thành công!", id = product.Id });
        }

        // --- 1.2. CẬP NHẬT THÔNG TIN (Admin) ---
        [HttpPut("update")]
        [Authorize(AuthenticationSchemes = "MyCookie", Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] GetProduct dto)
        {
            var p = await _context.Products.FindAsync(dto.Id);
            if (p == null) return NotFound("Không tìm thấy sản phẩm");

            p.Name = dto.Name;
            p.Price = dto.Price;
            p.Description = dto.Description;
            p.ThumbImageLink = dto.ThumbImageLink;
            p.CategoryId = dto.CategoryId;
            p.QuantityInStock = dto.QuantityInStock;
            p.IsActive = dto.IsActive;
            p.IsDiscount = dto.IsDiscount;
            p.SaleDiscount = dto.SaleDiscount;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công!" });
        }

        // --- 1.3. BẬT/TẮT TRẠNG THÁI ẨN HIỆN (Admin) ---
        [HttpPatch("toggle-active/{id}")]
        [Authorize(AuthenticationSchemes = "MyCookie", Roles = "Admin")]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();

            p.IsActive = p.IsActive == 1 ? 0 : 1;
            await _context.SaveChangesAsync();
            return Ok(new { message = p.IsActive == 1 ? "Đã hiện sản phẩm" : "Đã ẩn sản phẩm" });
        }

        // --- 1.3. XÓA VĨNH VIỄN KHỎI DATABASE (Admin) ---
        [HttpDelete("delete-permanent/{id}")]
        [Authorize(AuthenticationSchemes = "MyCookie", Roles = "Admin")]
        public async Task<IActionResult> DeletePermanent(string id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();

            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa vĩnh viễn khỏi hệ thống!" });
        }
    }
}