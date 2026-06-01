using System;

namespace PhanVietDuy_2380600375.Models.Domain
{
    /// <summary>
    /// Lưu trữ sản phẩm yêu thích của người dùng trong database.
    /// Khác với CartItem (session), WishlistItem gắn với UserId và tồn tại lâu dài.
    /// </summary>
    public class WishlistItem
    {
        public int Id { get; set; }

        /// <summary>Id người dùng — FK đến AspNetUsers</summary>
        public string UserId { get; set; } = null!;

        /// <summary>Id sản phẩm — FK đến Products</summary>
        public int ProductId { get; set; }

        /// <summary>Thời điểm thêm vào wishlist</summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation Properties ──
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
