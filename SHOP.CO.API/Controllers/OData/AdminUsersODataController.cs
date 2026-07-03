using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using SHOP.CO.Application.Services;

namespace SHOP.CO.API.Controllers.OData
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminUsersODataController : ODataController
    {
        private readonly IUserAdminService _service;

        public AdminUsersODataController(IUserAdminService service)
        {
            _service = service;
        }

        [EnableQuery(PageSize = 100)]
        public IActionResult Get()
        {
            return Ok(_service.GetUserODataQuery());
        }

    }
}
