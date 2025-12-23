namespace Pawtopia.DTOs.Order
{

    public class OrderSummaryDto
    {
        public string OrderId { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
        public long Status { get; set; }
        public string StatusName { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public double TotalAmount { get; set; }
        public int TotalItems { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
    }

    public class OrderDetailDto : OrderSummaryDto
    {
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; } 
        public double SubTotal => Quantity * Price;
    }

    public class AdminUpdateOrderInfoDto
    {
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string DetailAddress { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Province { get; set; } = null!;
    }
}