using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _context;

        public CouponService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CouponValidationResult> ValidateAsync(string code, decimal subTotal)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code && c.IsActive);
            
            if (coupon == null)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Mã giảm giá không tồn tại hoặc đã hết hạn." };
            }

            if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Mã giảm giá đã hết hạn." };
            }

            if (coupon.MaxUsage.HasValue && coupon.UsageCount >= coupon.MaxUsage.Value)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = "Mã giảm giá đã hết lượt sử dụng." };
            }

            if (coupon.MinOrderAmount > 0 && subTotal < coupon.MinOrderAmount)
            {
                return new CouponValidationResult { IsValid = false, ErrorMessage = $"Đơn hàng tối thiểu {coupon.MinOrderAmount.ToString("C0", new System.Globalization.CultureInfo("vi-VN"))} để sử dụng mã này." };
            }

            decimal discountAmount = 0;
            string discountText = "";

            if (coupon.DiscountType == "Percentage")
            {
                discountAmount = subTotal * (coupon.DiscountValue / 100m);
                discountText = $"-{coupon.DiscountValue}%";
            }
            else
            {
                discountAmount = coupon.DiscountValue;
                discountText = $"-{coupon.DiscountValue.ToString("C0", new System.Globalization.CultureInfo("vi-VN"))}";
            }

            if (discountAmount > subTotal) discountAmount = subTotal;

            return new CouponValidationResult
            {
                IsValid = true,
                DiscountAmount = discountAmount,
                DiscountText = discountText
            };
        }

        public async Task IncrementUsageAsync(string code)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
            if (coupon != null)
            {
                coupon.UsageCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}
