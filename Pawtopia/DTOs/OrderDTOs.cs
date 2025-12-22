namespace Pawtopia.DTOs.Order
{
    // DTO cơ bản cho danh sách
    public class OrderHistoryDto
    {
        public string OrderId { get; set; }
        public string CreatedAt { get; set; }
        public string Status { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public int TotalItems { get; set; }
    }

    // DTO cho Admin (Thêm thông tin khách hàng)
    public class AdminOrderDto : OrderHistoryDto
    {
        public string UserId { get; set; }
        public string CustomerName { get; set; } // Tên người nhận
        public string PhoneNumber { get; set; }  // SĐT người nhận
    }

    // DTO chi tiết đơn hàng
    public class OrderDetailDto : OrderHistoryDto
    {
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }

    // DTO từng món hàng
    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double SubTotal => Quantity * Price;
    }
}