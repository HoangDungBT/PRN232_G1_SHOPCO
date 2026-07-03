using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers.OData
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminInventoryODataController : ODataController
    {
        private readonly IAdminService _service;
        public AdminInventoryODataController(IAdminService service) => _service = service;

        [EnableQuery(PageSize = 50)]
        public IActionResult Get() => Ok(_service.GetInventoryODataQuery());
    }
}