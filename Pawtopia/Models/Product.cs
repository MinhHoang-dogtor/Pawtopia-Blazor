namespace Pawtopia.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long IsActive { get; set; } = 1;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ThumbImageLink { get; set; } = null!;
        public double? SaleDiscount { get; set; }
        public long IsDiscount { get; set; } = 0;

        public string CategoryId { get; set; } = null!;
        public long? QuantityInStock { get; set; }
        public double? Price { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}