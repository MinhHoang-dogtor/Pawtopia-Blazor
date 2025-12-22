namespace Pawtopia.Client.DTOs
{
    public class GetProduct
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public double? Price { get; set; }
        public string? Description { get; set; }
        public string? ThumbImageLink { get; set; }
        public string CategoryId { get; set; } = null!;
        public long? QuantityInStock { get; set; }
        public long IsActive { get; set; }

        // Thêm các trường này để tính toán giá
        public double? SaleDiscount { get; set; }
        public long IsDiscount { get; set; }
    }
    public class CreateProduct
    {
        public string Name { get; set; } = null!;
        public double? Price { get; set; } // Khớp kiểu double?
        public string? Description { get; set; }
        public string? ThumbImageLink { get; set; }
        public string CategoryId { get; set; } = null!;
        public long QuantityInStock { get; set; }
        public long IsActive { get; set; }
    }
}