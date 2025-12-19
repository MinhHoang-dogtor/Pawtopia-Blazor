using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pawtopia.Models
{
    public class Address
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string AddressLine { get; set; } = string.Empty; // Số nhà, đường
        public string Ward { get; set; } = string.Empty;        // Phường/Xã
        public string Province { get; set; } = string.Empty;    // Tỉnh/Thành phố

        public bool IsDefault { get; set; } = false;            // Cần trường này cho nút "Thiết lập mặc định"

        // Khóa ngoại liên kết với User
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}