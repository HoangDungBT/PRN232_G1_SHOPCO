using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers.OData
{
    // =================================================================
    // 1. CATEGORIES ODATA
    // URL will be: GET /odata/AdminCategoriesOData
    // =================================================================
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
    [Authorize(Roles = "Admin,Staff")]
    public class AdminCategoriesODataController : ODataController
    {
        private readonly IAdminCategoryService _service;
        public AdminCategoriesODataController(IAdminCategoryService service) => _service = service;

        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetODataQuery());
    }

    // =================================================================
    // 2. INVENTORY ODATA
    // URL will be: GET /odata/AdminInventoryOData
    // =================================================================
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
    [Authorize(Roles = "Admin,Staff")]
    public class AdminInventoryODataController : ODataController
    {
        private readonly IAdminDashboardService _service; // Assuming this is where GetInventoryODataQuery is
        public AdminInventoryODataController(IAdminDashboardService service) => _service = service;

        [EnableQuery(PageSize = 50)]
        public IActionResult Get() => Ok(_service.GetInventoryODataQuery());
    }

    // =================================================================
    // 3. ORDERS ODATA
    // URL will be: GET /odata/AdminOrdersOData
    // =================================================================
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
    [Authorize(Roles = "Admin,Staff")]
    public class AdminOrdersODataController : ODataController
    {
        private readonly IAdminOrderService _service;
        public AdminOrdersODataController(IAdminOrderService service) => _service = service;

        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetOrdersODataQuery());
    }

    // =================================================================
    // 4. PRODUCTS ODATA
    // URL will be: GET /odata/AdminProductsOData
    // =================================================================
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductsODataController : ODataController // Fixed casing to OData
    {
        private readonly IAdminProductService _service;
        public AdminProductsODataController(IAdminProductService service) => _service = service;

        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetProductODataQuery());
    }

    // =================================================================
    // 5. USERS ODATA
    // URL will be: GET /odata/AdminUsersOData
    // =================================================================
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
    [Authorize(Roles = "Admin,Staff")]
    public class AdminUsersODataController : ODataController
    {
        private readonly IAdminUserService _service;
        public AdminUsersODataController(IAdminUserService service) => _service = service;

        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetUserODataQuery());
    }

    [Authorize(Roles = "Admin,Staff")]
    public class AdminLogsODataController : ODataController
    {
        private readonly IAdminLogService _service;

        public AdminLogsODataController(IAdminLogService service)
        {
            _service = service;
        }

        [EnableQuery(PageSize = 100)]
        public IActionResult Get()
        {
            return Ok(_service.GetLogsODataQuery());
        }
    }
}