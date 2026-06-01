using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new async Task<Product?> GetByIdAsync(object id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == (int)id);
        }

        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<PagedResult<ProductListItem>> SearchAsync(ProductFilterParams p)
        {
            // Call stored procedure sp_SearchProducts
            var pCategorySlug = new SqlParameter("@CategorySlug", p.CategorySlug ?? (object)DBNull.Value);
            var pSearchText = new SqlParameter("@SearchText", p.SearchText ?? (object)DBNull.Value);
            var pMinPrice = new SqlParameter("@MinPrice", p.MinPrice ?? (object)DBNull.Value);
            var pMaxPrice = new SqlParameter("@MaxPrice", p.MaxPrice ?? (object)DBNull.Value);
            var pSortBy = new SqlParameter("@SortBy", p.SortBy ?? (object)DBNull.Value);
            var pPageNumber = new SqlParameter("@PageNumber", p.PageNumber);
            var pPageSize = new SqlParameter("@PageSize", p.PageSize);
            
            // Note: Since sp_SearchProducts returns multiple result sets (TotalCount, Data),
            // EF Core's FromSqlRaw can only map one result set directly to an entity.
            // For a robust implementation we use ADO.NET directly or Dapper.
            // To stick to EF Core only, we can query VwProductList directly.
            
            var query = _context.VwProductLists.AsQueryable();

            if (!string.IsNullOrEmpty(p.CategorySlug))
                query = query.Where(x => x.CategorySlug == p.CategorySlug);

            if (!string.IsNullOrEmpty(p.SearchText))
                query = query.Where(x => x.Name.Contains(p.SearchText) || x.ShortDesc.Contains(p.SearchText));

            if (p.MinPrice.HasValue)
                query = query.Where(x => x.Price >= p.MinPrice.Value);

            if (p.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= p.MaxPrice.Value);
                
            if (p.IsFeatured.HasValue)
            {
                query = query.Where(x => x.IsFeatured == p.IsFeatured.Value);
            }

            switch (p.SortBy?.ToLower())
            {
                case "price_asc":
                    query = query.OrderBy(x => x.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(x => x.Id);
                    break;
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((p.PageNumber - 1) * p.PageSize).Take(p.PageSize).ToListAsync();

            var mappedItems = items.Select(x => new ProductListItem
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                ShortDesc = x.ShortDesc,
                Price = x.Price,
                OriginalPrice = x.OriginalPrice,
                ThumbnailUrl = x.ThumbnailUrl,
                Badge = x.Badge,
                BadgeStyle = x.BadgeStyle,
                CategoryName = x.CategoryName,
                CategorySlug = x.CategorySlug,
                AvgRating = x.AvgRating,
                ReviewCount = x.TotalReviews,
                IsFeatured = x.IsFeatured,
                IsActive = x.IsActive
            }).ToList();

            return new PagedResult<ProductListItem>
            {
                Items = mappedItems,
                TotalCount = totalCount,
                PageNumber = p.PageNumber,
                PageSize = p.PageSize
            };
        }

        public async Task<List<Product>> GetFeaturedAsync(int take = 4)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Product>> GetRelatedAsync(int categoryId, int excludeId, int take = 4)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.CategoryId == categoryId && p.Id != excludeId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.OrderDetails)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product == null) return false;

            if (product.OrderDetails.Any())
            {
                // Soft delete
                product.IsActive = false;
            }
            else
            {
                // Hard delete
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            return await _context.Products.AnyAsync(p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId.Value));
        }
    }
}
