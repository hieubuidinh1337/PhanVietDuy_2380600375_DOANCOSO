using PhanVietDuy_2380600375.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetAllActiveAsync();
        new Task<Category?> GetByIdAsync(object id);
        Task<Category?> GetBySlugAsync(string slug);
        Task UpdateAsync(Category category);
    }
}
