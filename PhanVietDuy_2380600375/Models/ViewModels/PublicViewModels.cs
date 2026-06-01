using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace PhanVietDuy_2380600375.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<ProductListItem> FeaturedProducts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Review> HomepageReviews { get; set; } = new();
    }

    public class ProductIndexViewModel
    {
        public IPagedList<ProductListItem> Products { get; set; } = null!;
        public List<Category> Categories { get; set; } = new();
        public string? CurrentCategory { get; set; }
        public string? CurrentSort { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int TotalCount { get; set; }
        public (decimal Min, decimal Max) PriceRange { get; set; }
    }

    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = null!;
        public List<ProductImage> Images { get; set; } = new();
        public List<ProductColor> Colors { get; set; } = new();
        public List<ProductSize> Sizes { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<ProductListItem> RelatedProducts { get; set; } = new();
        public decimal AvgRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingBreakdown { get; set; } = new();
    }

    public class SubmitReviewViewModel
    {
        public string Slug { get; set; } = null!;
        public int ProductId { get; set; }
        
        [Range(1, 5, ErrorMessage = "Đánh giá từ 1 đến 5 sao")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
        [MaxLength(2000, ErrorMessage = "Nội dung tối đa 2000 ký tự")]
        public string Content { get; set; } = null!;
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string CustomerName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string CustomerEmail { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string CustomerPhone { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string ShipAddress { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập thành phố")]
        public string ShipCity { get; set; } = null!;

        public string ShipCountry { get; set; } = "Vietnam";
        public string? ShipPostalCode { get; set; }
        public string? Note { get; set; }

        public string PaymentMethod { get; set; } = "COD";

        public List<CartItem> CartItems { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public string? AppliedCouponCode { get; set; }
    }

    public class MyOrdersViewModel
    {
        public IPagedList<Order> Orders { get; set; } = null!;
    }

    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Nội dung từ 20-2000 ký tự")]
        public string Message { get; set; } = null!;
        public string? Phone { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = null!;

        public byte? Age { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }

    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<Order> RecentOrders { get; set; } = new();
        public List<ProductListItem> WishlistProducts { get; set; } = new();
    }

    public class UpdateProfileViewModel
    {
        [Required]
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public byte? Age { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
