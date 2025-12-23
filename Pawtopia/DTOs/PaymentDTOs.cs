using System.ComponentModel.DataAnnotations;

namespace Pawtopia.DTOs.Payment
{
    public class PaymentRequestDto
    {
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public string AddressId { get; set; } = null!;
        // -----------------------------

        [Required]
        public string PaymentMethod { get; set; } = null!;

        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressLine { get; set; }
        public string? Ward { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
    }

    public class CartItemDto
    {
        public string ProductId { get; set; } = null!;
        public long Quantity { get; set; }
        public long Price { get; set; }
    }
}