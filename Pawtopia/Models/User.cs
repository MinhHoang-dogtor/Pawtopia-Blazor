using Microsoft.AspNetCore.Identity;

namespace Pawtopia.Models
{
    // Định nghĩa danh sách Role cố định
    public enum UserRole
    {
        User,
        Admin
    }

    public class User
    {
        // Các thuộc tính cơ bản
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }

        // Các thuộc tính tùy chỉnh
        public string? DisplayName { get; set; }
        public string? ProfileImageLink { get; set; }

        // ÉP BUỘC: Chỉ được là User hoặc Admin. Mặc định là User
        public UserRole Role { get; set; } = UserRole.User;

        // Quan hệ bảng
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}