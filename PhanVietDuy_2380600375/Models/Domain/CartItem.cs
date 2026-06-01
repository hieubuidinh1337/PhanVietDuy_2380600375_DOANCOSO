using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class CartItem
{
    public int Id { get; set; }

    public string SessionKey { get; set; } = null!;

    public string? UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string? SelectedColor { get; set; }

    public string? SelectedSize { get; set; }

    public DateTime AddedAt { get; set; }

    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;

    [JsonIgnore]
    public virtual ApplicationUser? User { get; set; }

    public string ProductName => Product?.Name ?? "";
    public string? ThumbnailUrl => Product?.ThumbnailUrl;
    public string SelectedVariant => !string.IsNullOrEmpty(SelectedColor) && !string.IsNullOrEmpty(SelectedSize) 
        ? $"{SelectedColor} / {SelectedSize}" 
        : SelectedColor ?? SelectedSize ?? "";
    public decimal LineTotal => (Product?.Price ?? 0) * Quantity;
}
