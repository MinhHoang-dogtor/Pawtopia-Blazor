using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class ProductItem
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public string StockKeepingUnit { get; set; } = String.Empty;
        public uint QuantityInStock { get; set; }
        [Range(0, float.MaxValue, ErrorMessage = "Giá không được âm!")]
        public float Price { get; set; }
        public string ThumbImageLink { get; set; } = String.Empty;

        public string ProductId { get; set; } = String.Empty;
        public Product Product { get; set; } = default!;
    }
}
