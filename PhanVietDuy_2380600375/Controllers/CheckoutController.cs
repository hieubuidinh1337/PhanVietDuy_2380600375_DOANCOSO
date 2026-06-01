using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Services.Interfaces;
using PhanVietDuy_2380600375.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;

namespace PhanVietDuy_2380600375.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICouponService _couponService;
        private readonly IVNPayService _vnpayService;
        private readonly ApplicationDbContext _context;

        public CheckoutController(
            ICartService cartService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager,
            ICouponService couponService,
            IVNPayService vnpayService,
            ApplicationDbContext context)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
            _couponService = couponService;
            _vnpayService = vnpayService;
            _context = context;
        }

        private string GetSessionKey()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return "user_" + User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return HttpContext.Session.GetString("CartKey") ?? System.Guid.NewGuid().ToString();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var key = GetSessionKey();
            var cart = await _cartService.GetCartAsync(key);

            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            decimal discountAmount = 0;
            var appliedCoupon = HttpContext.Session.GetString("AppliedCoupon");
            if (!string.IsNullOrEmpty(appliedCoupon))
            {
                var couponResult = await _couponService.ValidateAsync(appliedCoupon, cart.Subtotal);
                if (couponResult.IsValid)
                {
                    discountAmount = couponResult.DiscountAmount;
                }
                else
                {
                    HttpContext.Session.Remove("AppliedCoupon");
                    appliedCoupon = null;
                }
            }

            var vm = new CheckoutViewModel
            {
                CartItems = cart.Items,
                SubTotal = cart.Subtotal,
                ShippingFee = cart.ShippingFee,
                DiscountAmount = discountAmount,
                Total = cart.Subtotal + cart.ShippingFee - discountAmount,
                AppliedCouponCode = appliedCoupon
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    vm.CustomerName = user.FullName ?? "";
                    vm.CustomerEmail = user.Email ?? "";
                    vm.CustomerPhone = user.PhoneNumber ?? "";
                    vm.ShipAddress = user.Address ?? "";
                    vm.ShipCity = user.City ?? "";
                    vm.ShipCountry = user.Country ?? "Vietnam";
                }
            }

            return View(vm);
        }

        [HttpPost("/checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel vm)
        {
            var key = GetSessionKey();
            var cart = await _cartService.GetCartAsync(key);

            if (!cart.Items.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                vm.CartItems = cart.Items;
                vm.SubTotal = cart.Subtotal;
                vm.ShippingFee = cart.ShippingFee;
                vm.DiscountAmount = 0;
                vm.Total = cart.Subtotal + cart.ShippingFee;
                return View("Index", vm);
            }

            decimal discountAmount = 0;
            var appliedCoupon = HttpContext.Session.GetString("AppliedCoupon");
            if (!string.IsNullOrEmpty(appliedCoupon))
            {
                var couponResult = await _couponService.ValidateAsync(appliedCoupon, cart.Subtotal);
                if (couponResult.IsValid)
                {
                    discountAmount = couponResult.DiscountAmount;
                }
                else
                {
                    HttpContext.Session.Remove("AppliedCoupon");
                    appliedCoupon = null;
                }
            }

            if (!ModelState.IsValid)
            {
                vm.CartItems = cart.Items;
                vm.SubTotal = cart.Subtotal;
                vm.ShippingFee = cart.ShippingFee;
                vm.DiscountAmount = discountAmount;
                vm.Total = cart.Subtotal + cart.ShippingFee - discountAmount;
                vm.AppliedCouponCode = appliedCoupon;
                return View("Index", vm);
            }

            string? userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            var req = new CreateOrderRequest
            {
                UserId = userId,
                CustomerName = vm.CustomerName,
                CustomerEmail = vm.CustomerEmail,
                CustomerPhone = vm.CustomerPhone,
                ShipAddress = vm.ShipAddress,
                ShipCity = vm.ShipCity,
                ShipCountry = vm.ShipCountry,
                ShipPostalCode = vm.ShipPostalCode,
                Note = vm.Note,
                PaymentMethod = vm.PaymentMethod,
                SubTotal = cart.Subtotal,
                ShippingFee = cart.ShippingFee,
                DiscountAmount = discountAmount,
                TotalAmount = cart.Subtotal + cart.ShippingFee - discountAmount,
                CouponCode = appliedCoupon,
                CartItems = cart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product?.Price ?? 0,
                    SelectedColor = i.SelectedColor,
                    SelectedSize = i.SelectedSize
                }).ToList()
            };

            // Tạo đơn hàng trong database (trạng thái PaymentStatus = "Unpaid" mặc định)
            int orderId = await _orderService.CreateOrderAsync(req);

            await _cartService.ClearAsync(key);
            HttpContext.Session.Remove("AppliedCoupon");

            // Cập nhật địa chỉ user nếu chưa có
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && string.IsNullOrEmpty(user.Address))
                {
                    user.Address = vm.ShipAddress;
                    user.City = vm.ShipCity;
                    await _userManager.UpdateAsync(user);
                }
            }

            // ════════════════════════════════════════════════════════════
            // XỬ LÝ THANH TOÁN VNPAY
            // Nếu chọn VNPay → redirect sang cổng thanh toán VNPay
            // ════════════════════════════════════════════════════════════
            if (vm.PaymentMethod?.ToLower() == "vnpay")
            {
                // Lấy đơn hàng vừa tạo để lấy OrderCode
                var order = await _orderService.GetByIdAsync(orderId);
                if (order == null) return NotFound();

                // Lấy IP người dùng (quan trọng cho VNPay anti-fraud)
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                if (ipAddress == "::1") ipAddress = "127.0.0.1"; // IPv6 localhost → IPv4

                // Tạo request VNPay với thông tin đơn hàng
                var vnpayRequest = new VNPayPaymentRequest
                {
                    // OrderCode: mã đơn hàng duy nhất (VD-000001)
                    OrderCode = order.OrderCode ?? orderId.ToString(),
                    // Amount: số tiền VND (không có phần thập phân)
                    Amount = (long)order.TotalAmount,
                    // Mô tả hiển thị trên trang thanh toán VNPay
                    OrderInfo = $"Thanh toan don hang {order.OrderCode ?? orderId.ToString()} tai Vietduy Atelier",
                    IpAddress = ipAddress,
                    Locale = "vn"
                };

                // Lưu orderId vào session để xác minh khi VNPay gọi về
                HttpContext.Session.SetInt32("VNPayPendingOrderId", orderId);

                // Tạo URL thanh toán và redirect người dùng sang VNPay
                var paymentUrl = _vnpayService.CreatePaymentUrl(vnpayRequest, HttpContext);
                return Redirect(paymentUrl);
            }

            // Phương thức khác (COD, Bank) → chuyển thẳng đến trang xác nhận
            return RedirectToAction("Confirmation", new { orderId });
        }

        // ════════════════════════════════════════════════════════════════════
        // VNPAY RETURN — VNPay gọi về URL này sau khi thanh toán (GET)
        // URL: /checkout/vnpay-return?vnp_ResponseCode=00&vnp_TxnRef=VD-000001...
        // ════════════════════════════════════════════════════════════════════
        [HttpGet("checkout/vnpay-return")]
        public async Task<IActionResult> VNPayReturn()
        {
            // Xử lý callback từ VNPay — xác thực chữ ký HMAC-SHA512
            var result = _vnpayService.ProcessCallback(Request.Query);

            if (result.IsSuccess)
            {
                // ── Tìm đơn hàng theo OrderCode (VD-XXXXXX) ──
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderCode == result.OrderCode ||
                                              o.Id.ToString() == result.OrderCode);

                if (order != null)
                {
                    // Cập nhật trạng thái thanh toán thành "Paid"
                    order.PaymentStatus = "Paid";
                    // Cập nhật trạng thái đơn hàng thành "Confirmed" (đã thanh toán online)
                    order.Status = "Confirmed";
                    order.ConfirmedAt = System.DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Xóa session pending
                    HttpContext.Session.Remove("VNPayPendingOrderId");

                    TempData["Success"] = $"Thanh toán VNPay thành công! Mã giao dịch: {result.TransactionId}";
                    return RedirectToAction("Confirmation", new { orderId = order.Id });
                }
            }

            // Thanh toán thất bại hoặc bị hủy → chuyển đến trang thất bại
            TempData["VNPayMessage"] = result.Message;
            TempData["VNPayCode"] = result.ResponseCode;
            return RedirectToAction("PaymentFailed");
        }

        // ════════════════════════════════════════════════════════════════════
        // TRANG XÁC NHẬN ĐẶT HÀNG THÀNH CÔNG
        // ════════════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != null && order.UserId != userId)
            {
                return Forbid();
            }

            return View(order);
        }

        // ════════════════════════════════════════════════════════════════════
        // TRANG THÔNG BÁO THANH TOÁN THẤT BẠI
        // ════════════════════════════════════════════════════════════════════
        [HttpGet]
        public IActionResult PaymentFailed()
        {
            // PaymentFailed.cshtml đã có sẵn trong Views/Checkout/
            ViewBag.Message = TempData["VNPayMessage"] ?? "Thanh toán không thành công";
            ViewBag.Code = TempData["VNPayCode"] ?? "";
            return View();
        }
    }
}
