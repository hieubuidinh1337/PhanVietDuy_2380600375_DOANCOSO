using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Review>> GetByProductIdAsync(int productId, bool approvedOnly = true)
        {
            var query = _context.Reviews.Where(r => r.ProductId == productId);
            if (approvedOnly)
            {
                query = query.Where(r => r.IsApproved);
            }
            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetRatingBreakdownAsync(int productId)
        {
            var ratings = await _context.Reviews
                .Where(r => r.ProductId == productId && r.IsApproved)
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            var breakdown = new Dictionary<int, int>
            {
                { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
            };

            foreach (var r in ratings)
            {
                breakdown[r.Rating] = r.Count;
            }

            return breakdown;
        }

        public async Task<bool> UserHasPurchasedAsync(string userId, int productId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .AnyAsync(o => o.UserId == userId && o.OrderDetails.Any(od => od.ProductId == productId));
        }
    }
}
