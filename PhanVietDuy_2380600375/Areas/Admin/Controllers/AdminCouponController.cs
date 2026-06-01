using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrManager")]
    public class CouponController : Controller
    {
        private readonly ICouponRepository _couponRepo;

        public CouponController(ICouponRepository couponRepo)
        {
            _couponRepo = couponRepo;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _couponRepo.GetAllAsync();
            return View(coupons.OrderByDescending(c => c.CreatedAt));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CouponCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var existingCoupons = await _couponRepo.GetAllAsync();
            if (existingCoupons.Any(c => c.Code.Equals(vm.Code, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
                return View(vm);
            }

            var coupon = new Coupon
            {
                Code = vm.Code.ToUpper(),
                Description = vm.Description,
                DiscountType = vm.DiscountType,
                DiscountValue = vm.DiscountValue,
                MinOrderAmount = vm.MinOrderAmount ?? 0m,
                ExpiresAt = vm.ExpiresAt,
                IsActive = vm.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _couponRepo.AddAsync(coupon);
            TempData["Success"] = "Tạo mã giảm giá thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponRepo.GetByIdAsync(id);
            if (coupon == null) return NotFound();

            var vm = new CouponEditViewModel
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                MinOrderAmount = coupon.MinOrderAmount,
                ExpiresAt = coupon.ExpiresAt,
                IsActive = coupon.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CouponEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var coupon = await _couponRepo.GetByIdAsync(id);
            if (coupon == null) return NotFound();

            var existingCoupons = await _couponRepo.GetAllAsync();
            if (existingCoupons.Any(c => c.Id != id && c.Code.Equals(vm.Code, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
                return View(vm);
            }

            coupon.Code = vm.Code.ToUpper();
            coupon.Description = vm.Description;
            coupon.DiscountType = vm.DiscountType;
            coupon.DiscountValue = vm.DiscountValue;
            coupon.MinOrderAmount = vm.MinOrderAmount ?? 0m;
            coupon.ExpiresAt = vm.ExpiresAt;
            coupon.IsActive = vm.IsActive;

            _couponRepo.Update(coupon);
            TempData["Success"] = "Cập nhật mã giảm giá thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _couponRepo.GetByIdAsync(id);
            if (coupon == null) return Json(new { success = false, message = "Mã giảm giá không tồn tại." });

            _couponRepo.Remove(coupon);
            return Json(new { success = true, message = "Đã xóa mã giảm giá thành công." });
        }
    }
}
