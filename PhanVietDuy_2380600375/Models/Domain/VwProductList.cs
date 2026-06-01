using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class VwProductList
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? ShortDesc { get; set; }

    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    public int? DiscountPct { get; set; }

    public string? Badge { get; set; }

    public string? BadgeStyle { get; set; }

    public bool IsFeatured { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public string? ThumbnailUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CategorySlug { get; set; } = null!;

    public string CategoryLabel { get; set; } = null!;

    public decimal AvgRating { get; set; }

    public int TotalReviews { get; set; }

    public int ImageCount { get; set; }
}
