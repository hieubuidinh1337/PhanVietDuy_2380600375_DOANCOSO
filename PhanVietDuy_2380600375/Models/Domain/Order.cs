using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class Order
{
    public int Id { get; set; }

    public string? OrderCode { get; set; }

    public string? UserId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerEmail { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string ShipAddress { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public string ShipCountry { get; set; } = null!;

    public string? ShipPostalCode { get; set; }

    public string? Note { get; set; }

    public decimal SubTotal { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime OrderedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? ProcessedByAdminId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ApplicationUser? ProcessedByAdmin { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
