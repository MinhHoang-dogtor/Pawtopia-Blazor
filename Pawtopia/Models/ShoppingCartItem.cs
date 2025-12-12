using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class ShoppingCartItem
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public string ShoppingCartId { get; set; } = String.Empty;
        public ShoppingCart ShoppingCart { get; set; } = default!;

        public string ProductItemId { get; set; } = String.Empty;
        public ProductItem ProductItem { get; set; } = default!;
    }
}
