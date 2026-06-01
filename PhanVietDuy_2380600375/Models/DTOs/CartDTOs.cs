using System.Collections.Generic;
using PhanVietDuy_2380600375.Models.Domain;

namespace PhanVietDuy_2380600375.Models.DTOs
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public int ItemCount => Items?.Sum(i => i.Quantity) ?? 0;
        public decimal Discount => DiscountAmount;
    }

    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? SelectedColor { get; set; }
        public string? SelectedSize { get; set; }
        public string? SelectedVariant { get; set; }
    }

    public class UpdateCartRequest
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class CouponApplyRequest
    {
        public string Code { get; set; } = null!;
    }
}
