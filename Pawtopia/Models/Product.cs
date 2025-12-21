namespace Pawtopia.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long IsActive { get; set; } 
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ThumbImageLink { get; set; }
        public string CategoryId { get; set; } = null!;
        public long QuantityInStock { get; set; }
        public double? Price { get; set; }

        public virtual Category Category { get; set; } = null!;
    }
}