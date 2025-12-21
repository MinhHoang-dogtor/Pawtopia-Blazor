using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.Client.DTOs;

namespace pawtopia.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly PawtopiaDbContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(PawtopiaDbContext db)
        {
            _db = db;
        }

        // ========= ĐĂNG KÝ (REGISTER) =========
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            // 1. Kiểm tra email tồn tại
            var exists = await _db.Users.AnyAsync(x => x.Email == dto.Email);
            if (exists)
                return BadRequest("Tài khoản này đã tồn tại trong hệ thống");

            // 2. Tạo User mới
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email,
                UserName = dto.Email,
                DisplayName = dto.Name,
                ProfileImageLink = "",
                // SỬA LỖI CS0029: Gán bằng Enum thay vì string
                Role = UserRole.User
            };

            // 3. Mã hóa mật khẩu
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            // 4. Lưu vào DB
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!", role = user.Role.ToString() });
        }

        // ========= ĐĂNG NHẬP (LOGIN) =========
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            // 1. Tìm user theo Email
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null)
                return BadRequest("Tài khoản không tồn tại");

            // 2. Kiểm tra mật khẩu
            // SỬA CẢNH BÁO CS8604: Thêm dấu ! để xác nhận PasswordHash không null
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Mật khẩu không chính xác");

            // 3. Trả về thông tin kèm theo Role
            return Ok(new
            {
                message = "Đăng nhập thành công",
                user = new
                {
                    user.Id,
                    user.Email,
                    user.DisplayName,
                    // Trả về string của Enum để Client dễ đọc
                    Role = user.Role.ToString()
                }
            });
        }
    }
}