using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.ViewModels;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using PhanVietDuy_2380600375.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrManager")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public CategoryController(ICategoryRepository categoryRepo, IFileService fileService, ApplicationDbContext context)
        {
            _categoryRepo = categoryRepo;
            _fileService = fileService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetAllAsync();
            var categoryStats = await _context.Categories
                .Select(c => new
                {
                    Category = c,
                    ProductCount = c.Products.Count()
                })
                .OrderBy(c => c.Category.SortOrder)
                .ToListAsync();

            ViewBag.CategoryStats = categoryStats;
            return View(categories.OrderBy(c => c.SortOrder));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var slug = ProductController.GenerateSlug(vm.Name);
            var existing = await _categoryRepo.GetBySlugAsync(slug);
            if (existing != null) slug += "-" + Guid.NewGuid().ToString().Substring(0, 4);

            var category = new Category
            {
                Name = vm.Name,
                Slug = slug,
                Label = vm.Label,
                Description = vm.Description,
                SortOrder = vm.SortOrder,
                IsActive = vm.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            if (vm.ImageFile != null)
            {
                var result = await _fileService.SaveProductImageAsync(vm.ImageFile, slug, "cat"); // Using same file service logic
                category.ImageUrl = result.Url;
            }

            await _categoryRepo.AddAsync(category);

            TempData["Success"] = "Tạo danh mục thành công";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return NotFound();

            var vm = new CategoryEditViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Label = category.Label,
                Description = category.Description,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive,
                ExistingImageUrl = category.ImageUrl
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.Name = vm.Name;
            category.Label = vm.Label;
            category.Description = vm.Description;
            category.SortOrder = vm.SortOrder;
            category.IsActive = vm.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            if (vm.ImageFile != null)
            {
                _fileService.DeleteFile(category.ImageUrl ?? "");
                var result = await _fileService.SaveProductImageAsync(vm.ImageFile, category.Slug, "cat");
                category.ImageUrl = result.Url;
            }

            await _categoryRepo.UpdateAsync(category);

            TempData["Success"] = "Cập nhật danh mục thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return Json(new { success = false, message = "Danh mục không tồn tại." });

            var hasActiveProducts = await _context.Products.AnyAsync(p => p.CategoryId == id && p.IsActive);
            if (hasActiveProducts)
            {
                return Json(new { success = false, message = "Không thể xóa danh mục đang có sản phẩm." });
            }

            category.IsActive = false;
            await _categoryRepo.UpdateAsync(category);

            return Json(new { success = true, message = "Đã vô hiệu hóa danh mục." });
        }

        [HttpPost]
        public async Task<IActionResult> ReOrder([FromBody] List<int> orderedIds)
        {
            if (orderedIds == null || !orderedIds.Any()) return Json(new { success = false });

            var categories = await _categoryRepo.GetAllAsync();
            for (int i = 0; i < orderedIds.Count; i++)
            {
                var cat = categories.FirstOrDefault(c => c.Id == orderedIds[i]);
                if (cat != null)
                {
                    cat.SortOrder = i;
                    _context.Categories.Update(cat);
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
