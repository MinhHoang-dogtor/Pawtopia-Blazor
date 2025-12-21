using Microsoft.AspNetCore.Identity;

namespace Pawtopia.Models
{
    public enum UserRole { User, Admin }

    // 1. THÊM : IdentityUser
    public class User : IdentityUser
    {
        // 2. XÓA Id, Email, UserName, PasswordHash (vì IdentityUser đã có sẵn rồi)

        // 3. GIỮ LẠI các thuộc tính tùy chỉnh của bạn
        public string? DisplayName { get; set; }
        public string? ProfileImageLink { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        // Quan hệ bảng
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}