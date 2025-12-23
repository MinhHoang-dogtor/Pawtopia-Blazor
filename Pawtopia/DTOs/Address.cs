namespace Pawtopia.DTOs;

public class Address
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!; // Để liên kết với người dùng
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Province { get; set; } = null!; // Tỉnh/Thành phố
    public string District { get; set; } = null!; // Quận/Huyện
    public string Ward { get; set; } = null!;     // Phường/Xã
    public string DetailAddress { get; set; } = null!;
    public bool IsDefault { get; set; } = false;
}