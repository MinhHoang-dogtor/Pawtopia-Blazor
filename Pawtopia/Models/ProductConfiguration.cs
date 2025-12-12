using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class ProductConfiguration
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public string ProductItemId { get; set; } = String.Empty;
        public ProductItem ProductItem { get; set; } = default!;

        public string VariationOptionId { get; set; } = String.Empty;
        public VariationOption VariationOption { get; set; } = default!;
    }
}
