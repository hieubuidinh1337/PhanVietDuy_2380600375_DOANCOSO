using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Services.Interfaces;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using PhanVietDuy_2380600375.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace PhanVietDuy_2380600375.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepo;
        private readonly IWishlistRepository _wishlistRepo;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICartService cartService,
            ApplicationDbContext context,
            IProductRepository productRepo,
            IWishlistRepository wishlistRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
            _context = context;
            _productRepo = productRepo;
            _wishlistRepo = wishlistRepo;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var userExists = await _userManager.FindByEmailAsync(vm.Email);
            if (userExists != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng");
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.FullName,
                Age = vm.Age
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);

                var guestKey = HttpContext.Session.GetString("CartKey");
                if (!string.IsNullOrEmpty(guestKey))
                {
                    await _cartService.MergeAsync(guestKey, "user_" + user.Id, user.Id);
                }

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    user.LastLoginAt = System.DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    var guestKey = HttpContext.Session.GetString("CartKey");
                    if (!string.IsNullOrEmpty(guestKey))
                    {
                        await _cartService.MergeAsync(guestKey, "user_" + user.Id, user.Id);
                    }

                    if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Manager"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                }

                return LocalRedirect(returnUrl ?? "/");
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            return View(vm);
        }

        // ═══════════════════════════════════════════════════════════════════
        // SOCIAL LOGIN — BƯỚC 1: Khởi tạo challenge với provider bên ngoài
        // Gọi khi người dùng nhấn nút "Đăng nhập bằng Google/Facebook"
        // ═══════════════════════════════════════════════════════════════════
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl)
        {
            // Tạo redirect URL sau khi provider xác thực xong sẽ gọi về
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account",
                new { ReturnUrl = returnUrl });

            // Cấu hình thuộc tính challenge — Identity middleware sẽ xử lý phần còn lại
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            // Redirect người dùng tới trang đăng nhập của Google/Facebook
            return Challenge(properties, provider);
        }

        // ═══════════════════════════════════════════════════════════════════
        // SOCIAL LOGIN — BƯỚC 2: Nhận callback từ Google/Facebook
        // URL: /Account/ExternalLoginCallback?returnUrl=...
        // ═══════════════════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
        {
            // Nếu provider báo lỗi (user hủy hoặc bị từ chối)
            if (remoteError != null)
            {
                TempData["Error"] = $"Lỗi từ nhà cung cấp: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            // Lấy thông tin user từ provider (email, name, provider key...)
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["Error"] = "Không thể lấy thông tin từ tài khoản mạng xã hội.";
                return RedirectToAction(nameof(Login));
            }

            // ── Thử đăng nhập với tài khoản external đã liên kết trước đó ──
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                // Tìm user để cập nhật LastLoginAt
                var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (existingUser != null)
                {
                    existingUser.LastLoginAt = System.DateTime.UtcNow;
                    await _userManager.UpdateAsync(existingUser);

                    // Merge cart của guest vào cart của user đã đăng nhập
                    var guestKey = HttpContext.Session.GetString("CartKey");
                    if (!string.IsNullOrEmpty(guestKey))
                        await _cartService.MergeAsync(guestKey, "user_" + existingUser.Id, existingUser.Id);

                    // Redirect Admin về dashboard
                    if (await _userManager.IsInRoleAsync(existingUser, "Admin") ||
                        await _userManager.IsInRoleAsync(existingUser, "Manager"))
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                return LocalRedirect(returnUrl ?? "/");
            }

            // ── Tài khoản chưa liên kết: tạo user mới từ thông tin provider ──
            // Lấy email từ claim của provider
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Không thể lấy email từ tài khoản mạng xã hội. Vui lòng thử cách khác.";
                return RedirectToAction(nameof(Login));
            }

            // Kiểm tra email đã tồn tại trong hệ thống chưa
            var userByEmail = await _userManager.FindByEmailAsync(email);

            if (userByEmail != null)
            {
                // Email đã tồn tại → liên kết tài khoản external với user đã có
                await _userManager.AddLoginAsync(userByEmail, info);
                await _signInManager.SignInAsync(userByEmail, isPersistent: false);

                userByEmail.LastLoginAt = System.DateTime.UtcNow;
                await _userManager.UpdateAsync(userByEmail);

                // Merge cart
                var guestKey = HttpContext.Session.GetString("CartKey");
                if (!string.IsNullOrEmpty(guestKey))
                    await _cartService.MergeAsync(guestKey, "user_" + userByEmail.Id, userByEmail.Id);

                return LocalRedirect(returnUrl ?? "/");
            }

            // Email hoàn toàn mới → tạo tài khoản mới trong hệ thống
            var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0];

            var newUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true, // Email từ provider đã được xác minh
                IsActive = true,
                CreatedAt = System.DateTime.UtcNow
            };

            // Tạo user không có password (đăng nhập qua social)
            var createResult = await _userManager.CreateAsync(newUser);
            if (createResult.Succeeded)
            {
                // Gán role User mặc định
                await _userManager.AddToRoleAsync(newUser, "User");

                // Liên kết tài khoản external
                await _userManager.AddLoginAsync(newUser, info);

                // Đăng nhập ngay
                await _signInManager.SignInAsync(newUser, isPersistent: false);

                newUser.LastLoginAt = System.DateTime.UtcNow;
                await _userManager.UpdateAsync(newUser);

                // Merge cart
                var guestKey = HttpContext.Session.GetString("CartKey");
                if (!string.IsNullOrEmpty(guestKey))
                    await _cartService.MergeAsync(guestKey, "user_" + newUser.Id, newUser.Id);

                return LocalRedirect(returnUrl ?? "/");
            }

            // Tạo user thất bại — hiển thị lỗi
            foreach (var error in createResult.Errors)
                ModelState.AddModelError("", error.Description);

            TempData["Error"] = "Có lỗi xảy ra khi tạo tài khoản. Vui lòng thử lại.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("CartKey");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var recentOrders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderedAt)
                .Take(5)
                .ToListAsync();

            // ── Wishlist từ Database (thay vì Cookie) ──
            var wishlistItems = await _wishlistRepo.GetByUserIdAsync(user.Id);
            var wishlistProducts = wishlistItems
                .Where(w => w.Product != null && w.Product.IsActive)
                .Select(w => new ProductListItem
                {
                    Id = w.Product.Id,
                    Name = w.Product.Name,
                    Slug = w.Product.Slug,
                    ShortDesc = w.Product.ShortDesc,
                    Price = w.Product.Price,
                    OriginalPrice = w.Product.OriginalPrice,
                    ThumbnailUrl = w.Product.ThumbnailUrl,
                    Badge = w.Product.Badge,
                    BadgeStyle = w.Product.BadgeStyle,
                    CategoryName = w.Product.Category?.Name,
                    CategorySlug = w.Product.Category?.Slug,
                    IsFeatured = w.Product.IsFeatured,
                    IsActive = w.Product.IsActive,
                    AvgRating = w.Product.Reviews.Any()
                        ? (decimal)w.Product.Reviews.Average(r => r.Rating) : 0m,
                    ReviewCount = w.Product.Reviews.Count
                }).ToList();

            var vm = new ProfileViewModel
            {
                User = user,
                RecentOrders = recentOrders,
                WishlistProducts = wishlistProducts
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Wishlist()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // ── Lấy wishlist từ Database ──
            var wishlistItems = await _wishlistRepo.GetByUserIdAsync(user.Id);
            var wishlistProducts = wishlistItems
                .Where(w => w.Product != null && w.Product.IsActive)
                .Select(w => new ProductListItem
                {
                    Id = w.Product.Id,
                    Name = w.Product.Name,
                    Slug = w.Product.Slug,
                    ShortDesc = w.Product.ShortDesc,
                    Price = w.Product.Price,
                    OriginalPrice = w.Product.OriginalPrice,
                    ThumbnailUrl = w.Product.ThumbnailUrl,
                    Badge = w.Product.Badge,
                    BadgeStyle = w.Product.BadgeStyle,
                    CategoryName = w.Product.Category?.Name,
                    CategorySlug = w.Product.Category?.Slug,
                    IsFeatured = w.Product.IsFeatured,
                    IsActive = w.Product.IsActive,
                    AvgRating = w.Product.Reviews.Any()
                        ? (decimal)w.Product.Reviews.Average(r => r.Rating) : 0m,
                    ReviewCount = w.Product.Reviews.Count
                }).ToList();

            return View(wishlistProducts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel vm)
        {
            if (!ModelState.IsValid) return RedirectToAction("Profile");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            user.FullName = vm.FullName;
            user.Address = vm.Address;
            user.City = vm.City;
            user.Country = vm.Country;
            user.Age = vm.Age;
            user.PhoneNumber = vm.PhoneNumber;

            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Cập nhật thành công";

            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var result = await _userManager.ChangePasswordAsync(user, vm.CurrentPassword, vm.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Đổi mật khẩu thành công";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar, [FromServices] IFileService fileService)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            if (avatar != null && avatar.Length > 0 && avatar.Length <= 2 * 1024 * 1024)
            {
                var result = await fileService.SaveAvatarAsync(avatar, user.Id);
                user.AvatarUrl = result.Url;
                await _userManager.UpdateAsync(user);
                return Json(new { success = true, avatarUrl = result.Url });
            }

            return Json(new { success = false, message = "File không hợp lệ hoặc quá lớn." });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
