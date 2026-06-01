using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.DTOs
{
    public class CreateOrderRequest
    {
        public string? UserId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string ShipAddress { get; set; } = null!;
        public string ShipCity { get; set; } = null!;
        public string ShipCountry { get; set; } = "Vietnam";
        public string? ShipPostalCode { get; set; }
        public string? Note { get; set; }
        public string PaymentMethod { get; set; } = "COD";
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CouponCode { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? SelectedColor { get; set; }
        public string? SelectedSize { get; set; }
    }
}
