using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.DTOs; // <--- ĐÃ SỬA: Trỏ vào folder DTOs trong Backend
using Pawtopia.Models;

namespace Pawtopia.Controllers
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

            var exists = await _db.Users.AnyAsync(x => x.Email == dto.Email);
            if (exists)
                return BadRequest("Tài khoản này đã tồn tại trong hệ thống");

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email,
                UserName = dto.Email,
                DisplayName = dto.Name,
                ProfileImageLink = "",
                Role = UserRole.User // Đảm bảo trong User.cs có enum UserRole
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!", role = user.Role.ToString() });
        }

        // ========= ĐĂNG NHẬP (LOGIN) =========
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null)
                return BadRequest("Tài khoản không tồn tại");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Mật khẩu không chính xác");

            return Ok(new
            {
                message = "Đăng nhập thành công",
                user = new
                {
                    user.Id,
                    user.Email,
                    user.DisplayName,
                    Role = user.Role.ToString()
                }
            });
        }
    }
}