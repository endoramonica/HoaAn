using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Customers;

namespace VietCommerce.Api.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class DefaultController : ControllerBase
    {
        public DefaultController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This is a default controller.");
        }
    }
}
                