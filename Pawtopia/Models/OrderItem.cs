using System.ComponentModel.DataAnnotations.Schema; // Nhớ thêm thư viện này

namespace Pawtopia.Models
{
    public class OrderItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public long Quantity { get; set; }

        public string ProductItemId { get; set; } = null!; // Khóa ngoại hiện tại
        public string OrderId { get; set; } = null!;

        public virtual Order Order { get; set; } = null!;

        // --- THÊM DÒNG NÀY ĐỂ FIX LỖI .ThenInclude(oi => oi.Product) ---
        [ForeignKey("ProductItemId")]
        public virtual Product? Product { get; set; }
    }
}