using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class Product
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public bool IsActive { get; set; }

        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ThumbImageLink { get; set; } = String.Empty;

        [Range(0, float.MaxValue, ErrorMessage = "Discount không được âm!")]
        public float? SaleDiscount { get; set; }

        public string CategoryId { get; set; } = String.Empty;
        public Category Category { get; set; } = default!;
    }
}
