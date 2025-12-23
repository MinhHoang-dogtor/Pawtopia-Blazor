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


        [Required]
        public string District { get; set; } = null!; 
        
        [Required]
        public string Ward { get; set; } = null!; 


        [Required]
        public string DetailAddress { get; set; } = null!; 

        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]





        public bool IsDefault { get; set; } = false;
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}