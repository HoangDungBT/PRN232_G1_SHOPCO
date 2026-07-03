using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers.OData
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminOrdersODataController : ODataController
    {
        private readonly IOrderAdminService _service;

        public AdminOrdersODataController(IOrderAdminService service)
        {
            _service = service;
        }

        [EnableQuery(PageSize = 100)]
        public IActionResult Get()
        {
            return Ok(_service.GetOrdersODataQuery());
        }
    }
}
