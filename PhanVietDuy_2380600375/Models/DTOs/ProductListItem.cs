namespace PhanVietDuy_2380600375.Models.DTOs
{
    public class ProductListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? ShortDesc { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Badge { get; set; }
        public string? BadgeStyle { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }
        public decimal AvgRating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }
}
