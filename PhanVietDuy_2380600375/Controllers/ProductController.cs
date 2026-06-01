using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace PhanVietDuy_2380600375.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(
            IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            IReviewRepository reviewRepo,
            UserManager<ApplicationUser> userManager)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _reviewRepo = reviewRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? category, string? sort, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            var filter = new ProductFilterParams
            {
                CategorySlug = category,
                SortBy = sort,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                PageNumber = page,
                PageSize = 12
            };

            var result = await _productRepo.SearchAsync(filter);
            var categories = await _categoryRepo.GetAllActiveAsync();

            var vm = new ProductIndexViewModel
            {
                Products = new StaticPagedList<ProductListItem>(result.Items, result.PageNumber, result.PageSize, result.TotalCount),
                Categories = categories,
                CurrentCategory = category,
                CurrentSort = sort,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                TotalCount = result.TotalCount,
                PriceRange = (Min: 0, Max: 10000) // Dummy range for slider
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string slug)
        {
            var product = await _productRepo.GetBySlugAsync(slug);
            if (product == null)
                return NotFound();

            var reviews = await _reviewRepo.GetByProductIdAsync(product.Id, approvedOnly: true);
            var recentReviews = reviews.Take(10).ToList();
            var ratingBreakdown = await _reviewRepo.GetRatingBreakdownAsync(product.Id);
            var avgRating = reviews.Any() ? (decimal)reviews.Average(r => r.Rating) : 0m;
            
            var related = await _productRepo.GetRelatedAsync(product.CategoryId, product.Id, 4);

            var relatedProductItems = related.Select(p => new ProductListItem
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDesc = p.ShortDesc,
                Price = p.Price,
                OriginalPrice = p.OriginalPrice,
                ThumbnailUrl = p.ThumbnailUrl,
                Badge = p.Badge,
                BadgeStyle = p.BadgeStyle,
                CategoryName = p.Category?.Name,
                CategorySlug = p.Category?.Slug,
                IsFeatured = p.IsFeatured,
                IsActive = p.IsActive,
                AvgRating = p.Reviews.Any() ? (decimal)p.Reviews.Average(r => r.Rating) : 0m,
                ReviewCount = p.Reviews.Count
            }).ToList();

            var vm = new ProductDetailViewModel
            {
                Product = product,
                Images = product.ProductImages.OrderBy(i => i.Id).ToList(), // SortOrder missing, using Id
                Colors = product.ProductColors.ToList(),
                Sizes = product.ProductSizes.ToList(),
                Reviews = recentReviews,
                RelatedProducts = relatedProductItems,
                AvgRating = avgRating,
                TotalReviews = reviews.Count,
                RatingBreakdown = ratingBreakdown
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SubmitReview(SubmitReviewViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ReviewError"] = "Vui lòng điền đầy đủ thông tin.";
                return RedirectToAction("Detail", new { slug = vm.Slug });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var hasPurchased = await _reviewRepo.UserHasPurchasedAsync(user.Id, vm.ProductId);
            if (!hasPurchased)
            {
                TempData["ReviewError"] = "Bạn cần mua sản phẩm này trước khi đánh giá.";
                return RedirectToAction("Detail", new { slug = vm.Slug });
            }

            var review = new Review
            {
                ProductId = vm.ProductId,
                UserId = user.Id,
                AuthorName = user.FullName ?? user.UserName,
                Rating = (byte)vm.Rating,
                Content = vm.Content,
                IsApproved = false,
                CreatedAt = System.DateTime.UtcNow
            };

            await _reviewRepo.AddAsync(review);

            TempData["ReviewSubmitted"] = true;
            return RedirectToAction("Detail", new { slug = vm.Slug });
        }

        [HttpGet("product/quick-view/{id}")]
        public async Task<IActionResult> GetQuickView(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            var colors = product.ProductColors.Select(c => c.ColorName).ToList();
            var sizes = product.ProductSizes.Select(s => s.SizeName).ToList();
            var images = product.ProductImages.OrderBy(i => i.Id).Select(i => i.Url).ToList();

            if (images.Count == 0 && !string.IsNullOrEmpty(product.ThumbnailUrl))
            {
                images.Add(product.ThumbnailUrl);
            }

            return Json(new
            {
                id = product.Id,
                name = product.Name,
                price = product.Price,
                originalPrice = product.OriginalPrice,
                shortDesc = product.ShortDesc,
                thumbnailUrl = product.ThumbnailUrl,
                slug = product.Slug,
                badge = product.Badge,
                badgeStyle = product.BadgeStyle,
                colors = colors,
                sizes = sizes,
                images = images,
                isActive = product.IsActive
            });
        }
    }
}
