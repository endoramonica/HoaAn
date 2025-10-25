using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Authorization;
using VietCommerce.Core.DTOs.UserRoles;
namespace VietCommerce.Api.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class UserRoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<UserRoleController> _logger;
    public UserRoleController(IRoleService roleService, IPermissionService permissionService, ILogger<UserRoleController> logger)
    {
        _roleService = roleService;
        _permissionService = permissionService;
        _logger = logger;
    }
}
