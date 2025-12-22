using System.ComponentModel.DataAnnotations.Schema; // Nhớ thêm thư viện này

namespace Pawtopia.Models
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CreatedAt { get; set; } = DateTime.Now.ToString();
        public long Status { get; set; }
        public long IsPaid { get; set; }
        public string UserId { get; set; } = null!;

        public string AddressId { get; set; } = null!; // Khóa ngoại hiện tại
        public string? PaymentStatus { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // --- THÊM DÒNG NÀY ĐỂ FIX LỖI .Include(o => o.Address) ---
        [ForeignKey("AddressId")]
        public virtual Address? Address { get; set; }
    }
}