using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Repositories.Interfaces;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
