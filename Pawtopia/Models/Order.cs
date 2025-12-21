namespace Pawtopia.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CreatedAt { get; set; } = DateTime.Now.ToString();
        public long Status { get; set; }
        public long IsPaid { get; set; }
        public string UserId { get; set; } = null!;
        public string AddressId { get; set; } = null!;
        public string? PaymentStatus { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}