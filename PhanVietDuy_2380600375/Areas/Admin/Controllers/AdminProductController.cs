using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using PhanVietDuy_2380600375.Services.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using X.PagedList;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrManager")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IFileService _fileService;

        public ProductController(IProductRepository productRepo, ICategoryRepository categoryRepo, IFileService fileService)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(string? category, string? search, bool? featured, int page = 1)
        {
            var filter = new ProductFilterParams
            {
                CategorySlug = category,
                SearchText = search,
                IsFeatured = featured,
                PageNumber = page,
                PageSize = 20
            };

            var result = await _productRepo.SearchAsync(filter);
            var categories = await _categoryRepo.GetAllActiveAsync();

            var vm = new ProductAdminIndexViewModel
            {
                Products = new StaticPagedList<ProductListItem>(result.Items, result.PageNumber, result.PageSize, result.TotalCount),
                Categories = categories,
                Category = category,
                Search = search,
                Featured = featured
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepo.GetAllActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(new ProductCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepo.GetAllActiveAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(vm);
            }

            string slug = GenerateSlug(vm.Name);
            bool slugExists = await _productRepo.SlugExistsAsync(slug);
            if (slugExists) slug += "-" + Guid.NewGuid().ToString().Substring(0, 4);

            var product = new Product
            {
                Name = vm.Name,
                Slug = slug,
                CategoryId = vm.CategoryId,
                ShortDesc = vm.ShortDesc,
                Description = vm.Description,
                Materials = vm.Materials,
                Dimensions = vm.Dimensions,
                ShippingInfo = vm.ShippingInfo,
                Price = vm.Price,
                OriginalPrice = vm.OriginalPrice,
                Badge = vm.Badge,
                BadgeStyle = vm.BadgeStyle ?? "default",
                IsFeatured = vm.IsFeatured,
                IsActive = vm.IsActive,
                StockQuantity = vm.StockQuantity,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            if (vm.ThumbnailFile != null)
            {
                var thumbResult = await _fileService.SaveProductImageAsync(vm.ThumbnailFile, slug, "thumb");
                product.ThumbnailUrl = thumbResult.Url;
                product.ThumbnailPath = thumbResult.FilePath;
            }

            for (int i = 0; i < vm.AdditionalImages?.Count; i++)
            {
                var file = vm.AdditionalImages[i];
                if (file != null && file.Length > 0)
                {
                    var imgResult = await _fileService.SaveProductImageAsync(file, slug, $"img_{i}");
                    product.ProductImages.Add(new ProductImage
                    {
                        Url = imgResult.Url,
                        FilePath = imgResult.FilePath,
                        SortOrder = (byte)i,
                        IsPrimary = i == 0 && product.ThumbnailUrl == null
                    });
                }
            }

            foreach (var color in vm.Colors ?? new())
            {
                product.ProductColors.Add(new ProductColor { ColorName = color.ColorName, HexCode = color.HexCode });
            }

            foreach (var size in vm.Sizes ?? new())
            {
                product.ProductSizes.Add(new ProductSize { SizeName = size });
            }

            await _productRepo.CreateAsync(product);

            TempData["Success"] = "Sản phẩm đã được tạo thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _productRepo.DeleteAsync(id);
                if (deleted)
                    return Json(new { success = true, message = "Xóa/Vô hiệu hóa thành công." });
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product != null)
            {
                product.IsFeatured = !product.IsFeatured;
                await _productRepo.UpdateAsync(product);
                return Json(new { success = true, isFeatured = product.IsFeatured });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product != null)
            {
                product.IsActive = !product.IsActive;
                await _productRepo.UpdateAsync(product);
                return Json(new { success = true, isActive = product.IsActive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, int quantity)
        {
            if (quantity < 0) return Json(new { success = false, message = "Số lượng không hợp lệ." });
            
            var product = await _productRepo.GetByIdAsync(id);
            if (product != null)
            {
                product.StockQuantity = quantity;
                await _productRepo.UpdateAsync(product);
                return Json(new { success = true, newStock = quantity });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> PreviewImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var tempUrl = await _fileService.SaveTempAsync(file);
                return Json(new { tempUrl });
            }
            return Json(new { success = false });
        }

        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            string str = input.ToLower();
            str = Regex.Replace(str, "á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ", "a");
            str = Regex.Replace(str, "é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ", "e");
            str = Regex.Replace(str, "í|ì|ỉ|ĩ|ị", "i");
            str = Regex.Replace(str, "ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ", "o");
            str = Regex.Replace(str, "ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự", "u");
            str = Regex.Replace(str, "ý|ỳ|ỷ|ỹ|ỵ", "y");
            str = Regex.Replace(str, "đ", "d");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"-+", "-");
            return str.Trim('-');
        }
    }
}
