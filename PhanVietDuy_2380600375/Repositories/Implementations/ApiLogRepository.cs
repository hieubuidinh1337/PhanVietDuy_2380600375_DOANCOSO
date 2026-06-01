using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Repositories.Interfaces;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class ApiLogRepository : Repository<ApiLog>, IApiLogRepository
    {
        public ApiLogRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
