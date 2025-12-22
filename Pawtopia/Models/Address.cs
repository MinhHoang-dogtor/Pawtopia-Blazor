using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pawtopia.Models
{
    public class Address
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        public string Province { get; set; } = null!;

<<<<<<< HEAD
        [Required]
        public string District { get; set; } = null!; // Quận/Huyện
        
        [Required]
        public string Ward { get; set; } = null!; // Phường/Xã


        [Required]
        public string DetailAddress { get; set; } = null!; // Số nhà, tên đường cụ thể
=======
        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public string District { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public string Ward { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể")]
        public string DetailAddress { get; set; } = null!;
>>>>>>> 4b5d52ac0a3aca102f47077e2d92ce033ef34967

        public bool IsDefault { get; set; } = false;

        // THÊM DÒNG NÀY ĐỂ HẾT LỖI CS1061
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}