using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pawtopia.Models
{
    [Table("Orders")] // Thêm dòng này để chắc chắn nó map đúng bảng "Orders"
    public class Order
    {
        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }

        // --- PHẦN QUAN TRỌNG CẦN SỬA ---
        public string AddressId { get; set; } // 1. Khai báo khóa ngoại

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; } // 2. Khai báo quan hệ "Một Order có một Address"
        // ---------------------------------

        public string? CreatedAt { get; set; }
        public long Status { get; set; }
        public long IsPaid { get; set; }
        public string? PaymentStatus { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}