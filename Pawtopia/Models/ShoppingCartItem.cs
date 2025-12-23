namespace Pawtopia.Models
{
    public class ShoppingCartItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ShoppingCartId { get; set; } = null!;
        public string ProductItemId { get; set; } = null!;

        public virtual ShoppingCart ShoppingCart { get; set; } = null!;
    }
}