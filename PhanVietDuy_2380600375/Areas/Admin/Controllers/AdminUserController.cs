using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhanVietDuy_2380600375.Data;
using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.EFCore;

namespace PhanVietDuy_2380600375.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string? search, string? role, bool? isActive, int page = 1)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Email.Contains(search) || u.FullName.Contains(search));

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            // Filtering by role in EF Core Identity usually requires joining UserRoles
            if (!string.IsNullOrEmpty(role))
            {
                var roleObj = await _roleManager.FindByNameAsync(role);
                if (roleObj != null)
                {
                    var userRoles = _context.UserRoles.Where(ur => ur.RoleId == roleObj.Id).Select(ur => ur.UserId);
                    query = query.Where(u => userRoles.Contains(u.Id));
                }
            }

            var users = await query.OrderByDescending(u => u.CreatedAt).ToPagedListAsync(page, 20);

            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.IsActive = isActive;
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

            return View(users);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            ViewBag.Orders = await _context.Orders.Where(o => o.UserId == id).OrderByDescending(o => o.OrderedAt).Take(10).ToListAsync();
            ViewBag.Reviews = await _context.Reviews.Where(r => r.UserId == id).OrderByDescending(r => r.CreatedAt).Take(10).ToListAsync();

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var vm = new AdminEditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Address = user.Address,
                City = user.City,
                Age = user.Age,
                IsActive = user.IsActive,
                SelectedRoles = userRoles.ToList()
            };

            ViewBag.AllRoles = roles;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, AdminEditUserViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = vm.FullName;
            user.Address = vm.Address;
            user.City = vm.City;
            user.Age = vm.Age;
            user.IsActive = vm.IsActive;

            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            
            if (vm.SelectedRoles != null && vm.SelectedRoles.Any())
            {
                await _userManager.AddToRolesAsync(user, vm.SelectedRoles);
            }

            if (!vm.IsActive)
            {
                await _userManager.UpdateSecurityStampAsync(user); // Force logout
            }

            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                
                if (!user.IsActive)
                {
                    await _userManager.UpdateSecurityStampAsync(user); // Force logout
                }
                
                return Json(new { success = true, isActive = user.IsActive });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại." });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == id)
            {
                return Json(new { success = false, message = "Bạn không thể tự xóa tài khoản của chính mình!" });
            }

            var isUserAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isUserAdmin)
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                {
                    return Json(new { success = false, message = "Không thể xóa quản trị viên duy nhất của hệ thống." });
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Xóa người dùng thành công." });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Json(new { success = false, message = $"Xóa thất bại: {errors}" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                // Send email
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
