using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhanVietDuy_2380600375.Models.DTOs;
using PhanVietDuy_2380600375.Repositories.Interfaces;
using PhanVietDuy_2380600375.Services.Interfaces;
using PhanVietDuy_2380600375.Models.ViewModels;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IApiLogService _logService;

        public ProductsApiController(IProductRepository productRepo, IApiLogService logService)
        {
            _productRepo = productRepo;
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterParams filter)
        {
            var result = await _productRepo.SearchAsync(filter);
            
            await _logService.LogAsync(Request.Path, Request.Method, 200, null, null, HttpContext.Connection.RemoteIpAddress?.ToString(), 10);
            
            return Ok(ApiResponse<PagedResult<ProductListItem>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound(ApiResponse<object>.Fail("Sản phẩm không tồn tại", 404));

            return Ok(ApiResponse<object>.Ok(product)); // Note: Usually map to DTO to avoid circular ref
        }
    }

    [ApiController]
    [Route("api/categories")]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoriesApiController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepo.GetAllActiveAsync();
            return Ok(ApiResponse<object>.Ok(categories));
        }
    }

    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound(ApiResponse<object>.Fail("Đơn hàng không tồn tại", 404));
            
            // Verify ownership logic omitted for brevity
            return Ok(ApiResponse<object>.Ok(order));
        }
    }

    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        // Placeholder for Auth Api using JWT if needed
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel vm)
        {
            return Ok(ApiResponse<object>.Ok(new { Token = "mock_jwt_token" }));
        }
    }
}
