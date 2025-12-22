using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{
    public class Address
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } = null!; // Liên kết với bảng User

        [Required]
        public string FullName { get; set; } = null!; // Họ và tên người nhận

        [Required]
        public string PhoneNumber { get; set; } = null!; // Số điện thoại

        [Required]
        public string Province { get; set; } = null!; // Tỉnh/Thành phố

        [Required]
        public string District { get; set; } = null!; // Quận/Huyện
        
        [Required]
        public string Ward { get; set; } = null!; // Phường/Xã


        [Required]
        public string DetailAddress { get; set; } = null!; // Số nhà, tên đường cụ thể

        public bool IsDefault { get; set; } = false; // Đặt làm mặc định hay không

        // Link ngược lại bảng User (nếu cần)
        public virtual User? User { get; set; }
    }
}