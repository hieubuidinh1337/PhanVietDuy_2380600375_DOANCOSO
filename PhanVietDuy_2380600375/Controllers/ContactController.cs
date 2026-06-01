using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.ViewModels;
using System;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ContactController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Send(ContactFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", vm);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"ContactRateLimit_{ipAddress}";

            if (_cache.TryGetValue(cacheKey, out int requestCount))
            {
                if (requestCount >= 3)
                {
                    ModelState.AddModelError("", "Bạn đã gửi quá nhiều yêu cầu. Vui lòng thử lại sau 1 giờ.");
                    return View("Index", vm);
                }
                _cache.Set(cacheKey, requestCount + 1, TimeSpan.FromHours(1));
            }
            else
            {
                _cache.Set(cacheKey, 1, TimeSpan.FromHours(1));
            }

            var message = new ContactMessage
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Phone = vm.Phone,
                Subject = vm.Subject,
                Message = vm.Message,
                IsRead = false,
                IsReplied = false,
                IpAddress = ipAddress,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ContactMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            TempData["ContactSent"] = true;
            return RedirectToAction("Index");
        }
    }
}
