using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    /// <summary>
    /// Triển khai WishlistRepository — thao tác trực tiếp với bảng WishlistItems trong SQL Server.
    /// Sử dụng EF Core để CRUD, có xử lý duplicate (unique constraint).
    /// </summary>
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm yêu thích của user, kèm thông tin Product và Category.
        /// Sắp xếp mới nhất lên đầu.
        /// </summary>
        public async Task<List<WishlistItem>> GetByUserIdAsync(string userId)
        {
            return await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Category)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Reviews)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Kiểm tra xem sản phẩm đã có trong wishlist của user chưa.
        /// </summary>
        public async Task<bool> ExistsAsync(string userId, int productId)
        {
            return await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        /// <summary>
        /// Thêm sản phẩm vào wishlist.
        /// Nếu đã tồn tại (vi phạm unique constraint), bỏ qua exception duplicate.
        /// </summary>
        public async Task AddAsync(string userId, int productId)
        {
            // Kiểm tra trước để tránh duplicate exception
            var exists = await ExistsAsync(userId, productId);
            if (exists) return;

            var item = new WishlistItem
            {
                UserId = userId,
                ProductId = productId,
                AddedAt = DateTime.UtcNow
            };

            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Xóa sản phẩm khỏi wishlist của user.
        /// </summary>
        public async Task RemoveAsync(string userId, int productId)
        {
            // Dùng ExecuteDeleteAsync (EF Core 7+) để tránh load entity vào memory
            await _context.WishlistItems
                .Where(w => w.UserId == userId && w.ProductId == productId)
                .ExecuteDeleteAsync();
        }

        /// <summary>
        /// Xóa toàn bộ wishlist của user — dùng khi user xóa tài khoản.
        /// </summary>
        public async Task ClearAsync(string userId)
        {
            await _context.WishlistItems
                .Where(w => w.UserId == userId)
                .ExecuteDeleteAsync();
        }

        /// <summary>
        /// Đếm số sản phẩm đang được yêu thích — dùng để hiển thị badge trên navbar.
        /// </summary>
        public async Task<int> CountAsync(string userId)
        {
            return await _context.WishlistItems
                .CountAsync(w => w.UserId == userId);
        }
    }
}
