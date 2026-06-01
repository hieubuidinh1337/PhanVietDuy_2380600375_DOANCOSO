using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int? ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductSku { get; set; }

    public string? ImageUrl { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal? LineTotal { get; set; }

    public string? SelectedColor { get; set; }

    public string? SelectedSize { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product? Product { get; set; }
}
