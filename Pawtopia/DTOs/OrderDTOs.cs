namespace Pawtopia.DTOs.Order
{
    // DTO cho danh sách lịch sử đơn hàng (Tóm tắt)
    public class OrderHistoryDto
    {
        public string OrderId { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; } // Trạng thái dạng chữ (VD: Đang xử lý)
        public double TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public int TotalItems { get; set; } // Tổng số lượng món hàng
    }

    // DTO cho chi tiết đơn hàng (Kế thừa từ History + thêm thông tin chi tiết)
    public class OrderDetailDto : OrderHistoryDto
    {
        // Thông tin giao hàng
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ShippingAddress { get; set; }

        // Danh sách sản phẩm
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; } // Giá 1 sản phẩm
        public double SubTotal => Quantity * Price; // Thành tiền
    }
}