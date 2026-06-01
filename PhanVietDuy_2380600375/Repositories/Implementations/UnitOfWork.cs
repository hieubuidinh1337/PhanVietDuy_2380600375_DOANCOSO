using System;
using System.Threading.Tasks;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Repositories.Interfaces;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public IProductColorRepository ProductColor { get; private set; }
        public IProductSizeRepository ProductSize { get; private set; }
        public IReviewRepository Review { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IContactMessageRepository ContactMessage { get; private set; }
        public INewsletterSubscriberRepository NewsletterSubscriber { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public ICartItemRepository CartItem { get; private set; }
        public IApiLogRepository ApiLog { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Product = new ProductRepository(_context);
            ProductImage = new ProductImageRepository(_context);
            ProductColor = new ProductColorRepository(_context);
            ProductSize = new ProductSizeRepository(_context);
            Review = new ReviewRepository(_context);
            Order = new OrderRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            ContactMessage = new ContactMessageRepository(_context);
            NewsletterSubscriber = new NewsletterSubscriberRepository(_context);
            Coupon = new CouponRepository(_context);
            CartItem = new CartItemRepository(_context);
            ApiLog = new ApiLogRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
