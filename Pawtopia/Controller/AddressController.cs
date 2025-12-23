using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("my-addresses/{userId}")]
        public async Task<IActionResult> GetMyAddresses(string userId)
        {
            var list = await _db.Addresses
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault)
                .ToListAsync();
            return Ok(list);
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
                if (string.IsNullOrEmpty(dto.Id)) dto.Id = Guid.NewGuid().ToString();
                _db.Addresses.Add(dto);
                await _db.SaveChangesAsync();
                return Ok(dto);
            }
            else
            {
                _db.Entry(existing).CurrentValues.SetValues(dto);
                existing.UserId = dto.UserId;
                await _db.SaveChangesAsync();
                return Ok(existing);
            }
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