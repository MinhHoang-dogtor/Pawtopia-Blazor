using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class OrderItem
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public uint Quantity { get; set; }

        public string ProductItemId { get; set; } = String.Empty;
        public ProductItem ProductItem { get; set; } = default!;

        public string OrderId { get; set; } = String.Empty;
        public Order Order { get; set; } = default!;
    }
}
