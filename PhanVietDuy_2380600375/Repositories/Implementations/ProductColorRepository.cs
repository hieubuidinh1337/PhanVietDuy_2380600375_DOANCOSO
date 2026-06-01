using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Repositories.Interfaces;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class ProductColorRepository : Repository<ProductColor>, IProductColorRepository
    {
        public ProductColorRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
