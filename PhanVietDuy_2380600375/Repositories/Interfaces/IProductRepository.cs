using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        new Task<Product?> GetByIdAsync(object id);
        Task<Product?> GetBySlugAsync(string slug);
        Task<PagedResult<ProductListItem>> SearchAsync(ProductFilterParams p);
        Task<List<Product>> GetFeaturedAsync(int take = 4);
        Task<List<Product>> GetRelatedAsync(int categoryId, int excludeId, int take = 4);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    }
}
