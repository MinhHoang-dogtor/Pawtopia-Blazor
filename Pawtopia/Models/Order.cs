using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public enum OrderStatus
    {
        Pending,    // Cho xac nhan
        Shipping,   // Dang van chuyen
        Completed,  // Hoan thanh
        Cancelled   // Da bi huy
    }

    public class Order
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        /// <summary>
        /// True if paid (QR, etc.), false for COD.
        /// </summary>
        public bool IsPaid { get; set; }

        public string UserId { get; set; } = String.Empty;
        public User User { get; set; } = default!;

        public string AddressId { get; set; } = String.Empty;
        public Address Address { get; set; } = default!;
    }
}
