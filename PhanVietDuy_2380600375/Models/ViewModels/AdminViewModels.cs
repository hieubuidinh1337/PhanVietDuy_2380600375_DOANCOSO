using Microsoft.AspNetCore.Http;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace PhanVietDuy_2380600375.Models.ViewModels
{
    public class DashboardStats
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
    }

    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardViewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public List<MonthlyRevenue> RevenueChart { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
        public List<VwOrderSummary> RecentOrders { get; set; } = new();
        public List<Product> LowStockProducts { get; set; } = new();
    }

    public class ProductAdminIndexViewModel
    {
        public IPagedList<ProductListItem> Products { get; set; } = null!;
        public List<Category> Categories { get; set; } = new();
        public string? Search { get; set; }
        public string? Category { get; set; }
        public bool? Featured { get; set; }
    }

    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên phải từ 2–200 ký tự")]
        public string Name { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public string? ShortDesc { get; set; }
        public string? Description { get; set; }
        public string? Materials { get; set; }
        public string? Dimensions { get; set; }
        public string? ShippingInfo { get; set; }

        [Range(0.01, 99999999, ErrorMessage = "Giá phải lớn hơn 0")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public decimal? OriginalPrice { get; set; }
        public string? Badge { get; set; }
        public string? BadgeStyle { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Vui lòng tải lên ảnh đại diện")]
        public IFormFile ThumbnailFile { get; set; } = null!;

        public List<IFormFile> AdditionalImages { get; set; } = new();

        public List<ColorInputVM> Colors { get; set; } = new();
        public List<string> Sizes { get; set; } = new();
    }

    public class ColorInputVM
    {
        public string ColorName { get; set; } = null!;
        public string HexCode { get; set; } = null!;
    }

    public class ProductEditViewModel : ProductCreateViewModel
    {
        public int Id { get; set; }
        public string? ExistingThumbnailUrl { get; set; }
        public new IFormFile? ThumbnailFile { get; set; } // Optional in Edit

        public List<ProductImage> ExistingImages { get; set; } = new();
        public List<int> DeleteImageIds { get; set; } = new();
    }

    public class CategoryCreateViewModel
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Label { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CategoryEditViewModel : CategoryCreateViewModel
    {
        public int Id { get; set; }
        public string? ExistingImageUrl { get; set; }
    }

    public class AdminOrderIndexViewModel
    {
        public IPagedList<VwOrderSummary> Orders { get; set; } = null!;
        public string? Status { get; set; }
        public string? Search { get; set; }
        public System.DateTime? From { get; set; }
        public System.DateTime? To { get; set; }
    }

    public class AdminEditUserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public byte? Age { get; set; }
        public bool IsActive { get; set; }
        public List<string> SelectedRoles { get; set; } = new();
    }

    public class AdminDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        
        public decimal RevenueGrowth { get; set; }
        public decimal OrderGrowth { get; set; }
        public decimal CustomerGrowth { get; set; }

        public List<MonthlySaleItem> MonthlySales { get; set; } = new();

        public int PendingOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int PendingReviews { get; set; }
        public int UnreadContacts { get; set; }

        public List<TopCategoryItem> TopCategories { get; set; } = new();
        public List<VwOrderSummary> RecentOrders { get; set; } = new();
    }

    public class MonthlySaleItem
    {
        public string Month { get; set; } = null!;
        public decimal Revenue { get; set; }
    }

    public class TopCategoryItem
    {
        public string Name { get; set; } = null!;
        public decimal Percentage { get; set; }
    }
}
