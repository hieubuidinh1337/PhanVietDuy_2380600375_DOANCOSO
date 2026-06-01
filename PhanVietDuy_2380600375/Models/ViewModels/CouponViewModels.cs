using System;
using System.ComponentModel.DataAnnotations;

namespace PhanVietDuy_2380600375.Models.ViewModels
{
    public class CouponCreateViewModel
    {
        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã giảm giá không vượt quá 50 ký tự")]
        public string Code { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Mô tả không vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [StringLength(20)]
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "FixedAmount"

        [Required(ErrorMessage = "Giá trị giảm là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu không hợp lệ")]
        public decimal? MinOrderAmount { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CouponEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã giảm giá không vượt quá 50 ký tự")]
        public string Code { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Mô tả không vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [StringLength(20)]
        public string DiscountType { get; set; } = "Percentage";

        [Required(ErrorMessage = "Giá trị giảm là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị đơn hàng tối thiểu không hợp lệ")]
        public decimal? MinOrderAmount { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
