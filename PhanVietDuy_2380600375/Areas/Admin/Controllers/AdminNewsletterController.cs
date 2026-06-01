using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrManager")]
    public class NewsletterSubscriberController : Controller
    {
        private readonly INewsletterSubscriberRepository _newsletterRepo;

        public NewsletterSubscriberController(INewsletterSubscriberRepository newsletterRepo)
        {
            _newsletterRepo = newsletterRepo;
        }

        public async Task<IActionResult> Index()
        {
            var subscribers = await _newsletterRepo.GetAllAsync();
            return View(subscribers.OrderByDescending(s => s.SubscribedAt));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var subscriber = await _newsletterRepo.GetByIdAsync(id);
            if (subscriber == null) return Json(new { success = false, message = "Email đăng ký không tồn tại." });

            _newsletterRepo.Remove(subscriber);
            return Json(new { success = true, message = "Đã xóa email khỏi danh sách nhận tin." });
        }
    }
}
