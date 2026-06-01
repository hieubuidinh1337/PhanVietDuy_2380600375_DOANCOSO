using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrManager")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Note: Since sp_AdminDashboard returns 3 result sets, EF Core doesn't support mapping
            // multiple result sets natively through a single FromSqlRaw.
            // As a robust alternative without ADO.NET boilerplate, we do the queries directly here.
            
            var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.Where(o => o.Status != "Cancelled").SumAsync(o => o.TotalAmount);
            var totalCustomers = await _context.Users.CountAsync();
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
            var shippingOrders = await _context.Orders.CountAsync(o => o.Status == "Shipped");
            var pendingReviews = await _context.Reviews.CountAsync(r => !r.IsApproved);
            var unreadContacts = await _context.ContactMessages.CountAsync(c => !c.IsRead);

            var recentOrders = await _context.VwOrderSummaries
                .OrderByDescending(o => o.OrderCode) // Assuming OrderCode sorts chronologically
                .Take(5)
                .ToListAsync();

            var vm = new AdminDashboardViewModel
            {
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalCustomers = totalCustomers,
                PendingOrders = pendingOrders,
                ShippingOrders = shippingOrders,
                PendingReviews = pendingReviews,
                UnreadContacts = unreadContacts,

                RevenueGrowth = 12.5m,
                OrderGrowth = 8.2m,
                CustomerGrowth = 15.0m,

                MonthlySales = new List<MonthlySaleItem>
                {
                    new() { Month = "Jan", Revenue = 15000 },
                    new() { Month = "Feb", Revenue = 18000 },
                    new() { Month = "Mar", Revenue = 22000 },
                    new() { Month = "Apr", Revenue = 25000 },
                    new() { Month = "May", Revenue = 30000 },
                    new() { Month = "Jun", Revenue = 35000 },
                    new() { Month = "Jul", Revenue = 40000 },
                    new() { Month = "Aug", Revenue = 42000 },
                    new() { Month = "Sep", Revenue = 48000 },
                    new() { Month = "Oct", Revenue = 52000 },
                    new() { Month = "Nov", Revenue = 60000 },
                    new() { Month = "Dec", Revenue = 75000 }
                },

                TopCategories = new List<TopCategoryItem>
                {
                    new() { Name = "Bags", Percentage = 45 },
                    new() { Name = "Shoes", Percentage = 25 },
                    new() { Name = "Accessories", Percentage = 20 },
                    new() { Name = "Apparel", Percentage = 10 }
                },

                RecentOrders = recentOrders
            };

            return View(vm);
        }
    }
}
