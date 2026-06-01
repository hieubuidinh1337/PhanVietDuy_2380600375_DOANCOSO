namespace PhanVietDuy_2380600375.Models.DTOs
{
    public class ProductFilterParams
    {
        public string? CategorySlug { get; set; }
        public string? SearchText { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public bool? IsFeatured { get; set; }
    }
}
