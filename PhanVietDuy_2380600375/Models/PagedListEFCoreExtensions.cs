using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace X.PagedList.EFCore
{
    public static class PagedListEFCoreExtensions
    {
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> superset, int pageNumber, int pageSize)
        {
            var totalItemCount = await superset.CountAsync();
            var page = pageNumber < 1 ? 1 : pageNumber;
            var subset = await superset.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new StaticPagedList<T>(subset, page, pageSize, totalItemCount);
        }
    }
}
