using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Pawtopia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly PawtopiaDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserController(PawtopiaDbContext db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<User>();
        }
        [HttpPost("update-profile")]
        public async Task<ActionResult<User>> UpdateProfile([FromBody] UserProfileDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null) return NotFound("Không thấy người dùng");

            if (!string.IsNullOrEmpty(dto.DisplayName)) user.DisplayName = dto.DisplayName;

            await _db.SaveChangesAsync();
            return Ok(user);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null) return NotFound("Người dùng không tồn tại");
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, dto.CurrentPassword);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Mật khẩu hiện tại không chính xác bố ơi!");
            }
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            await _db.SaveChangesAsync();
            return Ok(new { message = "Đổi mật khẩu thành công rực rỡ!" });
        }
    }
}