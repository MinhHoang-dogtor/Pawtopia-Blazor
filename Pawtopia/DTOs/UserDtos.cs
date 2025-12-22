namespace Pawtopia.Models
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = null!;
        public string? DisplayName { get; set; } // Đổi từ FullName sang DisplayName
        public string? Email { get; set; }
        public string? ProfileImageLink { get; set; }
    }

    public class ChangePasswordDto
    {
        public string UserId { get; set; } = null!;
        public string CurrentPassword { get; set; } = null!; // Mật khẩu hiện tại để so khớp
        public string NewPassword { get; set; } = null!;    // Mật khẩu mới để Hash
    }
}