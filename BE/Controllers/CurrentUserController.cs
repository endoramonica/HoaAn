using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces.Identities;

namespace VietCommerce.AdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // ✅ yêu cầu đăng nhập
    public class CurrentUserController : ControllerBase
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<CurrentUserController> _logger;

        public CurrentUserController(
            ICurrentUser currentUser,
            ILogger<CurrentUserController> logger)
        {
            _currentUser = currentUser;
            _logger = logger;
        }

        /// <summary>
        /// Lấy thông tin current user kèm permissions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            try
            {
                // ✅ Load permissions từ database
                var permissions = await _currentUser.GetPermissionsAsync();

                var info = new
                {
                    userId = _currentUser.UserId,
                    userName = _currentUser.UserName,
                    storeId = _currentUser.StoreId,
                    role = _currentUser.Role,
                    isAdmin = _currentUser.IsAdmin,
                    permissions = permissions.ToList() // ✅ Now from database!
                };

                return Ok(new
                {
                    success = true,
                    data = info
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user info");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        [HttpPost("has-permission/{permission}")]
        public async Task<IActionResult> CheckPermission(string permission)
        {
            try
            {
                // ✅ Check từ database
                var hasPermission = await _currentUser.HasPermissionAsync(permission);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        permission,
                        hasPermission
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission: {Permission}", permission);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error"
                });
            }
        }

        /// <summary>
        /// Get all user's detailed info
        /// </summary>
        [HttpGet("details")]
        public async Task<IActionResult> GetUserDetails()
        {
            try
            {
                var permissions = await _currentUser.GetPermissionsAsync();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        userId = _currentUser.UserId,
                        userName = _currentUser.UserName,
                        storeId = _currentUser.StoreId,
                        role = _currentUser.Role,
                        isAdmin = _currentUser.IsAdmin,
                        permissions = permissions.ToList(),
                        isAuthenticated = _currentUser.UserId != Guid.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user details");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}