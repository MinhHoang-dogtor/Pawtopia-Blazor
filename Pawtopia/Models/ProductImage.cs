using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pawtopia.Models
{
    public class ProductImage
    {
        [Key] // Khóa chính cho từng tấm ảnh
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ImageUrl { get; set; } = null!;

        // Khóa ngoại liên kết tới bảng Product
        public string ProductId { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}