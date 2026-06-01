using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetAllActiveAsync()
        {
            var list = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            if (list == null || !list.Any())
            {
                list = await _context.Categories
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync();
            }
            return list;
        }

        public new async Task<Category?> GetByIdAsync(object id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == (int)id);
        }

        public async Task<Category?> GetBySlugAsync(string slug)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
