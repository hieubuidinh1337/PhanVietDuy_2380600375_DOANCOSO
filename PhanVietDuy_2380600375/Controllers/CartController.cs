using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Models.DTOs;
using PhanVietDuy_2380600375.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(ICartService cartService, ICouponService couponService)
        {
            _cartService = cartService;
            _couponService = couponService;
        }

        private string GetSessionKey()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return "user_" + User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var key = HttpContext.Session.GetString("CartKey");
            if (string.IsNullOrEmpty(key))
            {
                key = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartKey", key);
            }
            return key;
        }

        [HttpGet("cart")]
        [HttpGet("cart/index")]
        public async Task<IActionResult> Index()
        {
            var key = GetSessionKey();
            var vm = await _cartService.GetCartAsync(key);

            var appliedCoupon = HttpContext.Session.GetString("AppliedCoupon");
            if (!string.IsNullOrEmpty(appliedCoupon))
            {
                var couponResult = await _couponService.ValidateAsync(appliedCoupon, vm.Subtotal);
                if (couponResult.IsValid)
                {
                    vm.CouponCode = appliedCoupon;
                    vm.DiscountAmount = couponResult.DiscountAmount;
                    vm.Total = vm.Subtotal + vm.ShippingFee - vm.DiscountAmount;
                }
                else
                {
                    HttpContext.Session.Remove("AppliedCoupon");
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(vm);
            }

            return View(vm);
        }

        [HttpGet("cart/items")]
        public async Task<IActionResult> GetCartItems()
        {
            var key = GetSessionKey();
            var vm = await _cartService.GetCartAsync(key);
            return Json(vm);
        }

        [HttpPost("cart/add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest req)
        {
            try
            {
                var key = GetSessionKey();
                var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
                
                await _cartService.AddItemAsync(key, req, userId);
                var vm = await _cartService.GetCartAsync(key);

                return Json(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("cart/update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartRequest req)
        {
            try
            {
                var key = GetSessionKey();
                await _cartService.UpdateQuantityAsync(key, req.CartItemId, req.Quantity);

                var vm = await _cartService.GetCartAsync(key);
                return Json(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("cart/remove/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var key = GetSessionKey();
                await _cartService.RemoveItemAsync(key, cartItemId);

                var vm = await _cartService.GetCartAsync(key);
                return Json(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("cart/coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] CouponApplyRequest req)
        {
            var key = GetSessionKey();
            var vm = await _cartService.GetCartAsync(key);
            
            var result = await _couponService.ValidateAsync(req.Code, vm.Subtotal);
            
            if (result.IsValid)
            {
                HttpContext.Session.SetString("AppliedCoupon", req.Code);
                decimal newTotal = vm.Subtotal + vm.ShippingFee - result.DiscountAmount;
                
                // Let's reload the cart state to return to client
                vm.CouponCode = req.Code;
                vm.DiscountAmount = result.DiscountAmount;
                vm.Total = newTotal;
                
                return Json(new { success = true, message = "Áp dụng mã giảm giá thành công", cart = vm });
            }
            
            return Json(new { success = false, message = result.ErrorMessage ?? "Mã giảm giá không hợp lệ" });
        }

        [HttpPost("cart/remove-coupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            HttpContext.Session.Remove("AppliedCoupon");
            var key = GetSessionKey();
            var vm = await _cartService.GetCartAsync(key);
            return Json(new { success = true, newTotal = vm.Total });
        }

        [HttpGet("cart/count")]
        public async Task<IActionResult> GetCartCount()
        {
            var key = GetSessionKey();
            var count = await _cartService.GetCountAsync(key);
            return Json(new { count = count });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MergeGuestCart(string guestKey)
        {
            var userKey = "user_" + User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                await _cartService.MergeAsync(guestKey, userKey, userId);
            }
            return Ok();
        }
    }
}
