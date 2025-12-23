namespace Pawtopia.Models
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = null!;
        public string? DisplayName { get; set; }
    }

    public class ChangePasswordDto
    {
        public string UserId { get; set; } = null!;
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!; 
    }
}