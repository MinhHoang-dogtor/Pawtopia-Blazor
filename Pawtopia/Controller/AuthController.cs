using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.DTOs;
using Microsoft.AspNetCore.Authentication; // Thêm dòng này
using Microsoft.AspNetCore.Authentication.Cookies; // Thêm dòng này
using System.Security.Claims; // Thêm dòng này để dùng Claim

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

        // ========= ĐĂNG KÝ (Giữ nguyên logic của bố) =========
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");
            var exists = await _db.Users.AnyAsync(x => x.Email == dto.Email);
            if (exists) return BadRequest("Tài khoản này đã tồn tại");

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email,
                UserName = dto.Email,
                DisplayName = dto.Name,
                ProfileImageLink = "",
                Role = UserRole.User
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!" });
        }

        // ========= ĐĂNG NHẬP (BẢN FIX CÓ COOKIE) =========
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null) return BadRequest("Tài khoản không tồn tại");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Mật khẩu không chính xác");

            // --- BƯỚC QUAN TRỌNG: TẠO VÉ THÔNG HÀNH (CLAIMS) ---
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Đóng dấu Admin/User vào đây
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookie"); // Phải khớp tên "MyCookie" ở Program.cs

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Ghi nhớ đăng nhập
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            // Lệnh này ra lệnh cho Server kẹp Cookie vào trình duyệt của bố
            await HttpContext.SignInAsync("MyCookie", new ClaimsPrincipal(claimsIdentity), authProperties);

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