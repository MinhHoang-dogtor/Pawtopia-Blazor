using System.ComponentModel.DataAnnotations;

namespace Pawtopia.DTOs
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

        public List<string> ImageUrls { get; set; } = new List<string>();
    }
    public class CreateProduct
    {
        [Required]
        public string Name { get; set; } = null!;
        public double? Price { get; set; }
        public string? Description { get; set; }
        public string? ThumbImageLink { get; set; }
        public string CategoryId { get; set; } = null!;
        public long QuantityInStock { get; set; }
        public long IsActive { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
    public class UpdateProduct
    {
        public string Name { get; set; } = null!;
        public double? Price { get; set; }
        public string? Description { get; set; }
        public string? ThumbImageLink { get; set; }
        public string CategoryId { get; set; } = null!;
        public long QuantityInStock { get; set; }
        public long IsActive { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}