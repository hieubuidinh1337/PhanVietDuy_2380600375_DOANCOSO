using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.EFCore;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrManager")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(bool? approved, int? productId, int page = 1)
        {
            var query = _context.Reviews.Include(r => r.Product).AsQueryable();

            if (approved.HasValue)
                query = query.Where(r => r.IsApproved == approved.Value);

            if (productId.HasValue)
                query = query.Where(r => r.ProductId == productId.Value);

            var reviews = await query.OrderByDescending(r => r.CreatedAt).ToPagedListAsync(page, 20);

            ViewBag.Approved = approved;
            ViewBag.ProductId = productId;

            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                review.IsApproved = true;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleHomepage(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null && review.IsApproved)
            {
                review.IsHomepage = !review.IsHomepage;
                await _context.SaveChangesAsync();
                return Json(new { success = true, isHomepage = review.IsHomepage });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> BulkAction([FromBody] BulkActionRequest req)
        {
            if (req.Ids == null || !req.Ids.Any()) return Json(new { success = false });

            var reviews = await _context.Reviews.Where(r => req.Ids.Contains(r.Id)).ToListAsync();
            
            if (req.Action == "approve")
            {
                foreach (var r in reviews) r.IsApproved = true;
            }
            else if (req.Action == "reject" || req.Action == "delete")
            {
                _context.Reviews.RemoveRange(reviews);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, affected = reviews.Count });
        }
    }

    public class BulkActionRequest
    {
        public List<int> Ids { get; set; } = new();
        public string Action { get; set; } = null!;
    }
}
