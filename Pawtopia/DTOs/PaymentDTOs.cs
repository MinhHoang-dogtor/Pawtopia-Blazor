namespace Pawtopia.DTOs.Payment
{
    // Dữ liệu React gửi lên
    public class PaymentRequestDto
    {
        public string UserId { get; set; } // ID người dùng mua hàng

        // Thông tin giao hàng (Bắt buộc phải có để tạo Address cho đơn hàng)
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine { get; set; }
        public string Ward { get; set; }
        public string Province { get; set; }

        public List<PaymentItemDto> Items { get; set; }
    }

    // Chi tiết từng sản phẩm trong mảng
    public class PaymentItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; } // Giá lấy từ frontend (Lưu ý: Backend nên check lại)
    }
}