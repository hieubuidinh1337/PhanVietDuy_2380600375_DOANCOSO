using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Repositories.Interfaces;

namespace PhanVietDuy_2380600375.Repositories.Implementations
{
    public class NewsletterSubscriberRepository : Repository<NewsletterSubscriber>, INewsletterSubscriberRepository
    {
        public NewsletterSubscriberRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
