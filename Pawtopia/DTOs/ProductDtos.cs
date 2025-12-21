namespace Pawtopia.DTOs
{
    public class GetProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; } // Đổi từ decimal sang double?
        public string? Description { get; set; }
        public string? ThumbImageLink { get; set; }
        public string? CategoryId { get; set; }
        public long QuantityInStock { get; set; } // Đổi từ int sang long
        public long IsActive { get; set; } // Đổi từ bool sang long (SQLite dùng 0/1)
    }

    public class CreateProduct
    {
        public string Name { get; set; }
        public double? Price { get; set; } // Đổi thành double?
        public string? Description { get; set; }
        public string? ThumbImageLink { get; set; }
        public string? CategoryId { get; set; }
        public long QuantityInStock { get; set; } // Đổi thành long
        public long IsActive { get; set; } // Đổi thành long
    }
}