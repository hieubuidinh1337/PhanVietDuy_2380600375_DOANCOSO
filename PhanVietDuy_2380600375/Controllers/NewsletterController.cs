using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsletterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string email, string? fullName)
        {
            if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email))
            {
                return Json(new { success = false, message = "Email không hợp lệ." });
            }

            var existing = await _context.NewsletterSubscribers.FirstOrDefaultAsync(n => n.Email == email);
            if (existing != null)
            {
                if (existing.IsActive)
                    return Json(new { success = false, message = "Bạn đã đăng ký nhận bản tin rồi." });
                else
                {
                    existing.IsActive = true;
                    existing.IsConfirmed = false;
                    existing.ConfirmToken = Guid.NewGuid().ToString();
                    await _context.SaveChangesAsync();
                    // Gửi email xác nhận
                    return Json(new { success = true, message = "Đã gửi email xác nhận. Vui lòng kiểm tra hộp thư." });
                }
            }

            var subscriber = new NewsletterSubscriber
            {
                Email = email,
                FullName = fullName,
                IsConfirmed = false,
                ConfirmToken = Guid.NewGuid().ToString(),
                IsActive = true,
                SubscribedAt = DateTime.UtcNow
            };

            await _context.NewsletterSubscribers.AddAsync(subscriber);
            await _context.SaveChangesAsync();

            // Gửi email xác nhận (mocked)
            return Json(new { success = true, message = "Cảm ơn bạn đã đăng ký! Vui lòng kiểm tra email để xác nhận." });
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(string token)
        {
            var subscriber = await _context.NewsletterSubscribers.FirstOrDefaultAsync(n => n.ConfirmToken == token);
            if (subscriber != null)
            {
                subscriber.IsConfirmed = true;
                subscriber.ConfirmToken = null;
                await _context.SaveChangesAsync();
                return View("Confirmed");
            }

            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe(string email)
        {
            var subscriber = await _context.NewsletterSubscribers.FirstOrDefaultAsync(n => n.Email == email);
            if (subscriber != null)
            {
                subscriber.IsActive = false;
                await _context.SaveChangesAsync();
                return View("Unsubscribed");
            }

            return View("Error");
        }
    }
}
