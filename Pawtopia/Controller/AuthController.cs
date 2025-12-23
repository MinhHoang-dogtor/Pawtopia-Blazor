using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Data;
using Pawtopia.Models;
using Pawtopia.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
                UserName = dto.Name,
                DisplayName = dto.Name,
                ProfileImageLink = "",
                Role = UserRole.User
            };

            user.PasswordHash = _hasher.HashPassword(user, dto.Password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Đăng ký thành công!" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null) return BadRequest("Dữ liệu không hợp lệ");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null) return BadRequest("Tài khoản không tồn tại");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Mật khẩu không chính xác");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookie");

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

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
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email))
                return BadRequest("Dữ liệu Google không hợp lệ");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = dto.Email,
                    UserName = dto.Email,
                    DisplayName = dto.Name, 
                    ProfileImageLink = dto.Picture,
                    Role = UserRole.User
                };

                // YÊU CẦU: Lưu Token Google thành Password (đã mã hóa)
                user.PasswordHash = _hasher.HashPassword(user, dto.Token);

                _db.Users.Add(user);
            }
            else
            {
                // --- B. Nếu đã có -> CẬP NHẬT ---
                user.DisplayName = dto.Name;
                user.ProfileImageLink = dto.Picture;

                // Cập nhật lại Password bằng Token mới để logic Login khớp nhau
                user.PasswordHash = _hasher.HashPassword(user, dto.Token);

                _db.Users.Update(user);
            }

            // Lưu thay đổi vào Database
            await _db.SaveChangesAsync();

            // 2. --- TẠO COOKIE ĐĂNG NHẬP (Copy y hệt logic hàm Login ở trên) ---
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookie");

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            // Ra lệnh trình duyệt lưu Cookie
            await HttpContext.SignInAsync("MyCookie", new ClaimsPrincipal(claimsIdentity), authProperties);

            // 3. Trả về kết quả
            return Ok(new
            {
                message = "Đăng nhập Google thành công",
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