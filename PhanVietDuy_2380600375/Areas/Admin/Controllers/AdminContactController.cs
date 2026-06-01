using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.EFCore;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "StaffAndAbove")]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(bool? isRead, int page = 1)
        {
            var query = _context.ContactMessages.AsQueryable();

            if (isRead.HasValue)
                query = query.Where(c => c.IsRead == isRead.Value);

            var messages = await query.OrderByDescending(c => c.CreatedAt).ToPagedListAsync(page, 20);

            ViewBag.IsRead = isRead;

            return View(messages);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null) return NotFound();

            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(int id, string replyContent)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null) return NotFound();

            // Mock sending email
            // await _emailSender.SendEmailAsync(message.Email, "Reply to your contact", replyContent);

            message.IsReplied = true;
            message.RepliedAt = DateTime.UtcNow;
            message.RepliedById = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
