using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // DÒNG NÀY ĐỂ HẾT LỖI CS1061
using Pawtopia.Data;
using Pawtopia.Models;

namespace Pawtopia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AddressController : ControllerBase
    {
        private readonly PawtopiaDbContext _db;
        public AddressController(PawtopiaDbContext db) => _db = db;

        [HttpGet("my-addresses")]
        public async Task<IActionResult> GetMyAddresses()
        {
            // Trả về hết để bố test cho nhanh
            return Ok(await _db.Addresses.ToListAsync());
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] Address dto)
        {
            if (string.IsNullOrEmpty(dto.UserId)) dto.UserId = "guest";

            if (dto.IsDefault)
            {
                var others = _db.Addresses.Where(x => x.UserId == dto.UserId);
                await others.ForEachAsync(x => x.IsDefault = false);
            }

            var existing = await _db.Addresses.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (existing == null)
            {
                _db.Addresses.Add(dto);
            }
            else
            {
                _db.Entry(existing).CurrentValues.SetValues(dto);
                existing.UserId = dto.UserId;
            }

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var addr = await _db.Addresses.FindAsync(id);
            if (addr == null) return NotFound();
            _db.Addresses.Remove(addr);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}