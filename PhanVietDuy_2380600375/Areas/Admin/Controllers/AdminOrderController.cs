using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Services.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.EFCore;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "StaffAndAbove")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;

        public OrderController(ApplicationDbContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(string? status, string? search, DateTime? from, DateTime? to, int page = 1)
        {
            var query = _context.VwOrderSummaries.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(o => o.OrderCode.Contains(search) || o.CustomerName.Contains(search) || o.CustomerPhone.Contains(search));

            if (from.HasValue)
                query = query.Where(o => o.OrderedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(o => o.OrderedAt <= to.Value.AddDays(1).AddTicks(-1));

            var orders = await query.OrderByDescending(o => o.OrderedAt).ToPagedListAsync(page, 20);

            // Stats for badges
            ViewBag.PendingCount = await _context.Orders.CountAsync(o => o.Status == "Pending");
            ViewBag.ConfirmedCount = await _context.Orders.CountAsync(o => o.Status == "Confirmed");
            ViewBag.ProcessingCount = await _context.Orders.CountAsync(o => o.Status == "Processing");

            var vm = new AdminOrderIndexViewModel
            {
                Orders = orders,
                Status = status,
                Search = search,
                From = from,
                To = to
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            
            // Assuming admin detail view model is just the order itself or a wrapper
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            try
            {
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _orderService.UpdateStatusAsync(id, newStatus, adminId);
                return Json(new { success = true, newStatus, updatedAt = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(int id, string paymentStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                if (order.PaymentStatus == "Unpaid" && (paymentStatus == "Paid" || paymentStatus == "Refunded"))
                {
                    order.PaymentStatus = paymentStatus;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Invalid status transition" });
        }

        [HttpPost]
        public async Task<IActionResult> AddNote(int id, string note)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                var date = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm");
                order.Note = (order.Note ?? "") + $"\n[Admin {date}]: {note}";
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrManager")]
        public async Task<IActionResult> ExportCsv(string? status, DateTime? from, DateTime? to)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (from.HasValue)
                query = query.Where(o => o.OrderedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(o => o.OrderedAt <= to.Value.AddDays(1).AddTicks(-1));

            var orders = await query.ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("OrderCode,CustomerName,Email,Phone,TotalAmount,Status,PaymentStatus,OrderedAt");

            foreach (var order in orders)
            {
                sb.AppendLine($"{order.OrderCode},{order.CustomerName},{order.CustomerEmail},{order.CustomerPhone},{order.TotalAmount},{order.Status},{order.PaymentStatus},{order.OrderedAt:yyyy-MM-dd HH:mm:ss}");
            }

            // USER REQUIREMENT: UTF-8 BOM for CSV
            var preamble = Encoding.UTF8.GetPreamble();
            var csvBytes = preamble.Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();

            return File(csvBytes, "text/csv; charset=utf-8", $"orders_{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var stats = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var pending = stats.FirstOrDefault(s => s.Status == "Pending")?.Count ?? 0;
            var confirmed = stats.FirstOrDefault(s => s.Status == "Confirmed")?.Count ?? 0;
            var processing = stats.FirstOrDefault(s => s.Status == "Processing")?.Count ?? 0;
            var shipped = stats.FirstOrDefault(s => s.Status == "Shipped")?.Count ?? 0;
            var delivered = stats.FirstOrDefault(s => s.Status == "Delivered")?.Count ?? 0;
            var cancelled = stats.FirstOrDefault(s => s.Status == "Cancelled")?.Count ?? 0;

            return Json(new { pending, confirmed, processing, shipped, delivered, cancelled });
        }
    }
}
