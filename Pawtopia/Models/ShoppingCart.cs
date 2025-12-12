using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class ShoppingCart
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public string UserId { get; set; } = String.Empty;
        public User User { get; set; } = default!;
    }
}
