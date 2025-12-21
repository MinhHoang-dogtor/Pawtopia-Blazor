namespace Pawtopia.Models
{
    public class Category
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string? NormalizedName { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}