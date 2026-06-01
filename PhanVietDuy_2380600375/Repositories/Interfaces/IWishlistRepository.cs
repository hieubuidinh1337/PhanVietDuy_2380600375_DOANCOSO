using PhanVietDuy_2380600375.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Interfaces
{
    /// <summary>
    /// Interface repository quản lý Wishlist trong Database.
    /// Hỗ trợ: thêm, xóa, lấy danh sách, kiểm tra trạng thái yêu thích.
    /// </summary>
    public interface IWishlistRepository
    {
        /// <summary>Lấy toàn bộ WishlistItem kèm Product của một user</summary>
        Task<List<WishlistItem>> GetByUserIdAsync(string userId);

        /// <summary>Kiểm tra sản phẩm đã được yêu thích chưa</summary>
        Task<bool> ExistsAsync(string userId, int productId);

        /// <summary>Thêm sản phẩm vào wishlist — ném exception nếu đã tồn tại</summary>
        Task AddAsync(string userId, int productId);

        /// <summary>Xóa sản phẩm khỏi wishlist</summary>
        Task RemoveAsync(string userId, int productId);

        /// <summary>Xóa toàn bộ wishlist của user</summary>
        Task ClearAsync(string userId);

        /// <summary>Đếm số sản phẩm trong wishlist của user</summary>
        Task<int> CountAsync(string userId);
    }
}
