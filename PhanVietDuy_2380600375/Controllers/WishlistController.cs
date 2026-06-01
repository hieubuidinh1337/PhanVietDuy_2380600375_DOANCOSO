using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Controllers
{
    /// <summary>
    /// Controller xử lý Wishlist (Sản phẩm yêu thích).
    /// Lưu dữ liệu vào Database thay vì Cookie — đảm bảo đồng bộ đa thiết bị.
    /// Yêu cầu đăng nhập [Authorize] — khách vãng lai không có wishlist DB.
    /// </summary>
    public class WishlistController : Controller
    {
        private readonly IWishlistRepository _wishlistRepo;

        public WishlistController(IWishlistRepository wishlistRepo)
        {
            _wishlistRepo = wishlistRepo;
        }

        // ── Request model ─────────────────────────────────────────────────
        public class WishlistToggleRequest
        {
            public string ProductId { get; set; } = null!;
        }

        // ─────────────────────────────────────────────────────────────────
        // POST /wishlist/toggle
        // Thêm hoặc xóa sản phẩm khỏi wishlist (AJAX).
        // Trả về JSON: { isWishlisted, wishlistCount, message }
        // ─────────────────────────────────────────────────────────────────
        [HttpPost("/wishlist/toggle")]
        [Authorize] // Bắt buộc đăng nhập — không thể lưu DB nếu không biết userId
        public async Task<IActionResult> ToggleWishlist([FromBody] WishlistToggleRequest req)
        {
            // Validate input
            if (req == null || string.IsNullOrEmpty(req.ProductId) || !int.TryParse(req.ProductId, out int prodId))
            {
                return BadRequest(new { message = "Id sản phẩm không hợp lệ" });
            }

            // Lấy userId từ claim (người dùng đã đăng nhập)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // Trả về 401 để frontend redirect sang trang đăng nhập
                return Unauthorized(new { message = "Vui lòng đăng nhập để sử dụng tính năng Yêu thích" });
            }

            bool isWishlisted;

            // Kiểm tra xem đã yêu thích chưa — toggle dựa trên trạng thái hiện tại
            var exists = await _wishlistRepo.ExistsAsync(userId, prodId);

            if (exists)
            {
                // Đã yêu thích → xóa khỏi DB
                await _wishlistRepo.RemoveAsync(userId, prodId);
                isWishlisted = false;
            }
            else
            {
                // Chưa yêu thích → thêm vào DB
                await _wishlistRepo.AddAsync(userId, prodId);
                isWishlisted = true;
            }

            // Đếm lại tổng số wishlist sau toggle — để cập nhật badge navbar
            var wishlistCount = await _wishlistRepo.CountAsync(userId);

            return Json(new
            {
                isWishlisted,
                wishlistCount,
                // Message hiển thị toast notification trên UI
                message = isWishlisted ? "Đã thêm vào Yêu thích" : "Đã xóa khỏi Yêu thích"
            });
        }

        // ─────────────────────────────────────────────────────────────────
        // GET /wishlist/count
        // Trả về số lượng sản phẩm yêu thích — dùng để cập nhật badge navbar
        // ─────────────────────────────────────────────────────────────────
        [HttpGet("/wishlist/count")]
        [Authorize]
        public async Task<IActionResult> GetCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { count = 0 });

            var count = await _wishlistRepo.CountAsync(userId);
            return Json(new { count });
        }

        // ─────────────────────────────────────────────────────────────────
        // GET /wishlist/check/{productId}
        // Kiểm tra sản phẩm có trong wishlist không — dùng để render trạng thái nút
        // ─────────────────────────────────────────────────────────────────
        [HttpGet("/wishlist/check/{productId}")]
        [Authorize]
        public async Task<IActionResult> CheckItem(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { isWishlisted = false });

            var exists = await _wishlistRepo.ExistsAsync(userId, productId);
            return Json(new { isWishlisted = exists });
        }
    }
}
