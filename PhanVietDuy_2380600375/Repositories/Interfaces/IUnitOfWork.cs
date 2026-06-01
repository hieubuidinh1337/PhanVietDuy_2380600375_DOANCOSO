using System;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IProductImageRepository ProductImage { get; }
        IProductColorRepository ProductColor { get; }
        IProductSizeRepository ProductSize { get; }
        IReviewRepository Review { get; }
        IOrderRepository Order { get; }
        IOrderDetailRepository OrderDetail { get; }
        IContactMessageRepository ContactMessage { get; }
        INewsletterSubscriberRepository NewsletterSubscriber { get; }
        ICouponRepository Coupon { get; }
        ICartItemRepository CartItem { get; }
        IApiLogRepository ApiLog { get; }
        
        Task<int> CompleteAsync();
    }
}
