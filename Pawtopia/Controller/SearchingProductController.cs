using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace pawtopia.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly PawtopiaDbContext _db;

        public ProductsController(PawtopiaDbContext db)
        {
            _db = db;
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            string? keyword,
            string? categoryId,
            int page = 1,
            int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _db.Products
                .Where(p => p.IsActive == 1)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }
            var totalItems = await query.CountAsync();
            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ThumbImageLink,
                    p.IsDiscount,
                    p.SaleDiscount,
                    p.QuantityInStock,
                    p.CategoryId
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                data = products
            });
        }
    }
}