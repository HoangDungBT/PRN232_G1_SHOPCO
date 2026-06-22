using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers.OData
{
    [ApiExplorerSettings(IgnoreApi = true)] // Ẩn khỏi Swagger
    [Authorize(Roles = "Admin,Staff")]
    public class AdminCategoriesODataController : ODataController
    {
        private readonly ICategoryAdminService _service;

        public AdminCategoriesODataController(ICategoryAdminService service)
        {
            _service = service;
        }

        // GET /odata/AdminCategories
        [EnableQuery(PageSize = 100)]
        public IActionResult Get()
        {
            var query = _service.GetODataQuery();
            return Ok(query);
        }
    }
}
