using Microsoft.AspNetCore.Identity;

namespace Pawtopia.Models
{    public enum UserRole
    {
        User,
        Admin
    }

    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfileImageLink { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}