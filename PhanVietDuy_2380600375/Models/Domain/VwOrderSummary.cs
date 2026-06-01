using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class VwOrderSummary
{
    public int Id { get; set; }

    public string? OrderCode { get; set; }

    public string CustomerName { get; set; } = null!;

    public string CustomerEmail { get; set; } = null!;

    public string CustomerPhone { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public string ShipCountry { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime OrderedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public string? RegisteredUserName { get; set; }

    public string? RegisteredUserEmail { get; set; }

    public int? ItemCount { get; set; }

    public int? TotalQty { get; set; }
}
