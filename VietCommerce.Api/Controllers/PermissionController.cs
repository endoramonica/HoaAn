using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Permissions;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class PermissionController : ControllerBase
{
   
}
