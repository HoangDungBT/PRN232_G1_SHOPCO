using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Application.Services;
using System.Threading.Tasks;

namespace SHOP.CO.API.Controllers.OData
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductsOdataController : ODataController
    {
        private readonly IProductAdminService _service;

        public AdminProductsOdataController(IProductAdminService service)
        {
            _service = service;
        }

        // 1. API ODATA LẤY DANH SÁCH (Giữ nguyên)
        [EnableQuery(PageSize = 100)]
        public IActionResult Get()
        {
            var query = _service.GetProductODataQuery();
            return Ok(query);
        }

    }
}