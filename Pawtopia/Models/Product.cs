namespace Pawtopia.Models
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long IsActive { get; set; } = 1;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ThumbImageLink { get; set; } = null!;

        // Trường mới theo SQL của bạn
        public double? SaleDiscount { get; set; }

        // Thêm trường IsDiscount (1 là có giảm giá, 0 là không)
        public long IsDiscount { get; set; } = 0;

        public string CategoryId { get; set; } = null!;
        public long? QuantityInStock { get; set; }
        public double? Price { get; set; }

        public virtual Category Category { get; set; } = null!;
    }
}