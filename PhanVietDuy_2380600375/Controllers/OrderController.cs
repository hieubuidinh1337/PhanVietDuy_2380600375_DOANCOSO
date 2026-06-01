using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.EFCore;

namespace PhanVietDuy_2380600375.Controllers
{
    [Authorize(Roles = "User")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;

        public OrderController(IOrderService orderService, ApplicationDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.OrderedAt);

            var orders = await query.ToPagedListAsync(page, 10);

            var vm = new MyOrdersViewModel
            {
                Orders = orders
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.GetByIdAsync(id);

            if (order == null || order.UserId != userId)
                return Forbid();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Forbid();

            try
            {
                await _orderService.CancelAsync(id, userId);
                TempData["Message"] = "Đơn hàng đã được hủy thành công.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Detail", new { id });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TrackOrder(string orderCode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                ViewBag.Error = "Không tìm thấy đơn hàng.";
                return View();
            }

            return View(order);
        }
    }
}
