using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Permissions;
using VietCommerce.Core.DTOs.Roles;
using VietCommerce.Core.DTOs.UserRoles;
namespace VietCommerce.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RoleController> _logger;
    public RoleController(IRoleService roleService, ILogger<RoleController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }
}
