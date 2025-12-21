namespace Pawtopia.Models
{
    public class OrderItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long Quantity { get; set; }
        public string ProductItemId { get; set; } = null!;
        public string OrderId { get; set; } = null!;

        public virtual Order Order { get; set; } = null!;
    }
}