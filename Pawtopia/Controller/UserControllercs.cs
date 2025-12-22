using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Microsoft.AspNetCore.Identity; // Cần thiết để dùng PasswordHasher
using Microsoft.AspNetCore.Authorization;

namespace Pawtopia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly PawtopiaDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher; // Khai báo bộ mã hóa

        public UserController(PawtopiaDbContext db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<User>(); // Khởi tạo bộ mã hóa
        }

        // 1. Cập nhật thông tin Profile
        [HttpPost("update-profile")]
        public async Task<ActionResult<User>> UpdateProfile([FromBody] UserProfileDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null) return NotFound("Không thấy người dùng");

            if (!string.IsNullOrEmpty(dto.DisplayName)) user.DisplayName = dto.DisplayName;
            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.ProfileImageLink)) user.ProfileImageLink = dto.ProfileImageLink;

            await _db.SaveChangesAsync();
            return Ok(user);
        }

        // 2. Đổi mật khẩu có kiểm tra Hash
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null) return NotFound("Người dùng không tồn tại");

            // BƯỚC 1: Kiểm tra mật khẩu hiện tại (Verify)
            // Nó sẽ tự lấy Salt và Hash trong DB ra để so sánh với cái người dùng vừa nhập
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, dto.CurrentPassword);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Mật khẩu hiện tại không chính xác bố ơi!");
            }

            // BƯỚC 2: Mã hóa mật khẩu mới (Hash) và lưu vào DB
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);

            await _db.SaveChangesAsync();
            return Ok(new { message = "Đổi mật khẩu thành công rực rỡ!" });
        }
    }
}