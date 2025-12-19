using Microsoft.AspNetCore.Identity;

namespace Pawtopia.Models
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public string ProfileImageLink { get; set; } = string.Empty;

        // --- Các trường bổ sung cho Hồ sơ ---
        public string? FullName { get; set; }         // Tên đầy đủ
        public string? Gender { get; set; } = "Khac"; // Nam, Nu, Khac
        public DateOnly? DateOfBirth { get; set; }    // Ngày sinh

        // Quan hệ: Một User có nhiều Address
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}