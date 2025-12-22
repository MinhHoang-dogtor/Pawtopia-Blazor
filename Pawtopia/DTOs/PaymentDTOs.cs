namespace Pawtopia.DTOs.Payment
{
    public class PaymentRequestDto
    {
        public string UserId { get; set; }

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine { get; set; }
        public string Ward { get; set; }
        public string Province { get; set; }
        public string District { get; set; }

        public List<PaymentItemDto> Items { get; set; }
    }

    public class PaymentItemDto
    {
        public string ProductId { get; set; }   // PHẢI là ProductId
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
