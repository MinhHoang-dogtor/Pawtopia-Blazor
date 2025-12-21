namespace Pawtopia.Models
{
    public class ShoppingCart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
    }
}