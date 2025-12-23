using System.ComponentModel.DataAnnotations.Schema;

namespace Pawtopia.Models
{
    public class OrderItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long Quantity { get; set; }
        public long Price { get; set; }

        public string OrderId { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
        public string ProductId { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}