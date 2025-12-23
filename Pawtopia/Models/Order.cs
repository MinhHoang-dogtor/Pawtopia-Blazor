using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pawtopia.Models
{
    [Table("Orders")] 
    public class Order
    {
        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }
        public string AddressId { get; set; } 

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; } 
        public string? CreatedAt { get; set; }
        public long Status { get; set; }
        public long IsPaid { get; set; }
        public string? PaymentStatus { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}