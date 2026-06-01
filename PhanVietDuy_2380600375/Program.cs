using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Repositories.Implementations;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using PhanVietDuy_2380600375.Services;
using PhanVietDuy_2380600375.Services.Implementations;
using PhanVietDuy_2380600375.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity (ApplicationUser + ApplicationRole)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt => {
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 4;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ═══════════════════════════════════════════════════════════════
// THÊM GOOGLE & FACEBOOK AUTHENTICATION
// AddGoogle/AddFacebook phải được chain từ AddAuthentication(),
// KHÔNG phải từ AddIdentity() (AddIdentity trả về IdentityBuilder)
// ═══════════════════════════════════════════════════════════════
builder.Services.AddAuthentication()
    // ── Google OAuth ──────────────────────────────────────────
    // Lấy credentials tại: https://console.cloud.google.com
    // Authorized redirect URI: https://localhost:PORT/signin-google
    .AddGoogle(options =>
    {
        options.ClientId     = builder.Configuration["Authentication:Google:ClientId"] ?? "YOUR_GOOGLE_CLIENT_ID";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "YOUR_GOOGLE_CLIENT_SECRET";
        options.CallbackPath = "/signin-google";
        options.Scope.Add("profile");
        options.Scope.Add("email");
    })
    // ── Facebook OAuth ────────────────────────────────────────
    // Lấy credentials tại: https://developers.facebook.com
    // Valid OAuth Redirect URI: https://localhost:PORT/signin-facebook
    .AddFacebook(options =>
    {
        options.AppId     = builder.Configuration["Authentication:Facebook:AppId"] ?? "YOUR_FACEBOOK_APP_ID";
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? "YOUR_FACEBOOK_APP_SECRET";
        options.CallbackPath = "/signin-facebook";
    });

// Configure application cookie to properly handle redirects for unauthenticated users
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 3. Session (dùng cho Cart)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt => {
    opt.IdleTimeout = TimeSpan.FromHours(2);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

// 4. Authorization Policies
builder.Services.AddAuthorization(opt => {
    opt.AddPolicy("AdminOrManager", p => p.RequireRole("Admin", "Manager"));
    opt.AddPolicy("StaffAndAbove", p => p.RequireRole("Admin", "Manager", "Staff"));
});

// 5. AutoMapper
builder.Services.AddAutoMapper(cfg => {}, typeof(Program).Assembly);

// 6. DI — Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IApiLogRepository, ApiLogRepository>();
builder.Services.AddScoped<INewsletterSubscriberRepository, NewsletterSubscriberRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductColorRepository, ProductColorRepository>();
builder.Services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Wishlist Repository — lưu sản phẩm yêu thích vào Database
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

// 7. DI — Services
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IApiLogService, ApiLogService>();
// VNPay Service — tích hợp cổng thanh toán VNPay
// IVNPayService được định nghĩa trong Services/VNPayService.cs
builder.Services.AddScoped<IVNPayService, VNPayService>();

// 8. HttpContextAccessor (CartService cần)
builder.Services.AddHttpContextAccessor();

// 9. Memory Cache (rate limiting cho ContactController)
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// ── Middleware Pipeline (thứ tự BẮT BUỘC) ──
app.UseStaticFiles();
app.UseRouting();

// SESSION PHẢI TRƯỚC AUTHENTICATION
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 10. Routes

// ── Admin Routes (English primary + Vietnamese aliases) ──
app.MapControllerRoute(
    name: "admin_product",
    pattern: "admin/product/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Product" });

app.MapControllerRoute(
    name: "admin_category",
    pattern: "admin/category/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Category" });

app.MapControllerRoute(
    name: "admin_order",
    pattern: "admin/order/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Order" });

app.MapControllerRoute(
    name: "admin_user",
    pattern: "admin/user/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "User" });

app.MapControllerRoute(
    name: "admin_review",
    pattern: "admin/review/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Review" });

app.MapControllerRoute(
    name: "admin_contact",
    pattern: "admin/contact/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Contact" });

// ── Admin Vietnamese aliases (backward compatibility) ──
app.MapControllerRoute(
    name: "admin_san_pham",
    pattern: "admin/san-pham/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Product" });

app.MapControllerRoute(
    name: "admin_danh_muc",
    pattern: "admin/danh-muc/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Category" });

app.MapControllerRoute(
    name: "admin_don_hang",
    pattern: "admin/don-hang/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Order" });

app.MapControllerRoute(
    name: "admin_nguoi_dung",
    pattern: "admin/nguoi-dung/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "User" });

app.MapControllerRoute(
    name: "admin_danh_gia",
    pattern: "admin/danh-gia/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Review" });

app.MapControllerRoute(
    name: "admin_lien_he",
    pattern: "admin/lien-he/{action=Index}/{id?}",
    defaults: new { area = "Admin", controller = "Contact" });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// ── Public Routes (English primary) ──
app.MapControllerRoute(
    name: "categories",
    pattern: "categories",
    defaults: new { controller = "Home", action = "Categories" });

app.MapControllerRoute(
    name: "danh_muc_page",
    pattern: "danh-muc",
    defaults: new { controller = "Home", action = "Categories" });

app.MapControllerRoute(
    name: "account_login",
    pattern: "account/login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "account_access_denied",
    pattern: "account/accessdenied",
    defaults: new { controller = "Account", action = "AccessDenied" });

app.MapControllerRoute(
    name: "account_logout",
    pattern: "account/logout",
    defaults: new { controller = "Account", action = "Logout" });

app.MapControllerRoute(
    name: "account_profile",
    pattern: "account/profile",
    defaults: new { controller = "Account", action = "Profile" });

app.MapControllerRoute(
    name: "account_change_password",
    pattern: "account/change-password",
    defaults: new { controller = "Account", action = "ChangePassword" });

app.MapControllerRoute(
    name: "account_register",
    pattern: "account/register",
    defaults: new { controller = "Account", action = "Register" });

app.MapControllerRoute(
    name: "wishlist",
    pattern: "wishlist",
    defaults: new { controller = "Account", action = "Wishlist" });

app.MapControllerRoute(
    name: "order_detail",
    pattern: "order/detail/{id?}",
    defaults: new { controller = "Order", action = "Detail" });

app.MapControllerRoute(
    name: "order_cancel",
    pattern: "order/cancel/{id?}",
    defaults: new { controller = "Order", action = "Cancel" });

app.MapControllerRoute(
    name: "order",
    pattern: "order/{action=MyOrders}/{id?}",
    defaults: new { controller = "Order" });

app.MapControllerRoute(
    name: "product_detail",
    pattern: "product/{slug}",
    defaults: new { controller = "Product", action = "Detail" });

app.MapControllerRoute(
    name: "product",
    pattern: "product/{action=Index}/{id?}",
    defaults: new { controller = "Product" });

app.MapControllerRoute(
    name: "contact",
    pattern: "contact/{action=Index}/{id?}",
    defaults: new { controller = "Contact" });

app.MapControllerRoute(
    name: "cart",
    pattern: "cart/{action=Index}/{id?}",
    defaults: new { controller = "Cart" });

app.MapControllerRoute(
    name: "checkout",
    pattern: "checkout/{action=Index}/{id?}",
    defaults: new { controller = "Checkout" });

// Route cho VNPay return callback
// VNPay sẽ gọi GET /checkout/vnpay-return?vnp_ResponseCode=00&...
app.MapControllerRoute(
    name: "checkout_vnpay_return",
    pattern: "checkout/vnpay-return",
    defaults: new { controller = "Checkout", action = "VNPayReturn" });

// ── Public Vietnamese aliases (backward compatibility) ──
app.MapControllerRoute(
    name: "tai_khoan_dang_nhap",
    pattern: "tai-khoan/dang-nhap",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "tai_khoan_dang_xuat",
    pattern: "tai-khoan/dang-xuat",
    defaults: new { controller = "Account", action = "Logout" });

app.MapControllerRoute(
    name: "tai_khoan_ho_so",
    pattern: "tai-khoan/ho-so",
    defaults: new { controller = "Account", action = "Profile" });

app.MapControllerRoute(
    name: "tai_khoan_doi_mat_khau",
    pattern: "tai-khoan/doi-mat-khau",
    defaults: new { controller = "Account", action = "ChangePassword" });

app.MapControllerRoute(
    name: "tai_khoan_dang_ky",
    pattern: "tai-khoan/dang-ky",
    defaults: new { controller = "Account", action = "Register" });

app.MapControllerRoute(
    name: "yeu_thich",
    pattern: "yeu-thich",
    defaults: new { controller = "Account", action = "Wishlist" });

app.MapControllerRoute(
    name: "don_hang_chi_tiet",
    pattern: "don-hang/chi-tiet/{id?}",
    defaults: new { controller = "Order", action = "Detail" });

app.MapControllerRoute(
    name: "don_hang_cancel",
    pattern: "don-hang/Cancel/{id?}",
    defaults: new { controller = "Order", action = "Cancel" });

app.MapControllerRoute(
    name: "don_hang",
    pattern: "don-hang/{action=MyOrders}/{id?}",
    defaults: new { controller = "Order" });

app.MapControllerRoute(
    name: "san_pham_detail",
    pattern: "san-pham/{slug}",
    defaults: new { controller = "Product", action = "Detail" });

app.MapControllerRoute(
    name: "san_pham",
    pattern: "san-pham/{action=Index}/{id?}",
    defaults: new { controller = "Product" });

app.MapControllerRoute(
    name: "lien_he",
    pattern: "lien-he/{action=Index}/{id?}",
    defaults: new { controller = "Contact" });

app.MapControllerRoute(
    name: "gio_hang",
    pattern: "gio-hang/{action=Index}/{id?}",
    defaults: new { controller = "Cart" });

app.MapControllerRoute(
    name: "thanh_toan",
    pattern: "thanh-toan/{action=Index}/{id?}",
    defaults: new { controller = "Checkout" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 11. Seed Admin User
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    
    // Ensure Admin role exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new ApplicationRole { Name = "Admin", Description = "Quản trị viên toàn hệ thống" });
    }
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new ApplicationRole { Name = "User", Description = "Khách hàng đã đăng ký" });
    }

    var adminEmail = "Vietduy@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Vietduy Admin",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = System.DateTime.UtcNow
        };
        var createResult = await userManager.CreateAsync(adminUser, "1110");
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            await userManager.AddToRoleAsync(adminUser, "User");
        }
    }
}

app.Run();
