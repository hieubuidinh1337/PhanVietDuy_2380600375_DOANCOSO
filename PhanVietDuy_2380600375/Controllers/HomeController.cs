using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Models;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using PhanVietDuy_2380600375.Models.DTOs;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IReviewRepository _reviewRepo;

        public HomeController(IProductRepository productRepo, ICategoryRepository categoryRepo, IReviewRepository reviewRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _reviewRepo = reviewRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productRepo.GetFeaturedAsync(4);
            var categories = await _categoryRepo.GetAllActiveAsync();
            var reviews = (await _reviewRepo.GetAllAsync())
                .Where(r => r.IsHomepage && r.IsApproved)
                .ToList();

            var featuredProductItems = featuredProducts.Select(p => new ProductListItem
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

            var vm = new HomeViewModel
            {
                FeaturedProducts = featuredProductItems,
                Categories = categories,
                HomepageReviews = reviews
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryRepo.GetAllActiveAsync();
            ViewData["Title"] = "Danh mục";
            ViewData["Description"] = "Khám phá toàn bộ danh mục sản phẩm thủ công tinh xảo của Vietduy Atelier — Túi xách, Đồng hồ, Lụa & Cashmere và nhiều hơn nữa.";
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q, int page = 1)
        {
            var filter = new ProductFilterParams
            {
                SearchText = q,
                PageNumber = page,
                PageSize = 12
            };

            var result = await _productRepo.SearchAsync(filter);
            
            // To render Product/Index view properly, we need to assemble ProductIndexViewModel
            var categories = await _categoryRepo.GetAllActiveAsync();

            var vm = new ProductIndexViewModel
            {
                Products = new X.PagedList.StaticPagedList<ProductListItem>(result.Items, result.PageNumber, result.PageSize, result.TotalCount),
                Categories = categories,
                TotalCount = result.TotalCount
            };

            return View("~/Views/Product/Index.cshtml", vm);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public new IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }
    }
}