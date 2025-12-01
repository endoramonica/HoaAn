using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services.Identity
{
    public class CurrentUserService : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private IEnumerable<string>? _cachedPermissions;
        private bool _permissionsLoaded = false;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            IUserRoleRepository userRoleRepository,
            IRolePermissionRepository rolePermissionRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRoleRepository = userRoleRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public Guid UserId =>
            Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                ? id
                : Guid.Empty;
        /// <summary>
        /// ✅ CustomerId từ JWT token claim
        /// </summary>
        public Guid CustomerId =>
            Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue("customerId"), out var id)
                ? id
                : Guid.Empty;
        public string UserName =>
            _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Unknown";

        public Guid? StoreId =>
            Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue("storeId"), out var id)
                ? id
                : null;

        public string Role =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        /// <summary>
        /// ✅ Load permissions từ database + JWT claims
        /// </summary>
        public IEnumerable<string> Permissions
        {
            get
            {
                // Trả về cached permissions nếu đã load
                if (_permissionsLoaded && _cachedPermissions != null)
                    return _cachedPermissions;

                // Nếu không load async được, trả về permissions từ JWT claims
                return _httpContextAccessor.HttpContext?.User?
                    .FindAll("permission")
                    .Select(c => c.Value)
                    ?? Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// ✅ Load permissions từ database (async method)
        /// </summary>
        public async Task<IEnumerable<string>> GetPermissionsAsync()
        {
            if (_permissionsLoaded && _cachedPermissions != null)
                return _cachedPermissions;

            try
            {
                var userId = UserId;
                if (userId == Guid.Empty)
                    return Enumerable.Empty<string>();

                // Lấy tất cả roles của user
                var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
                if (userRoles == null || !userRoles.Any())
                {
                    _cachedPermissions = Enumerable.Empty<string>();
                    _permissionsLoaded = true;
                    return _cachedPermissions;
                }

                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

                // Lấy tất cả permissions của các roles này
                var permissions = new List<string>();
                foreach (var roleId in roleIds)
                {
                    var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(roleId);
                    permissions.AddRange(rolePermissions.Select(rp => rp.Permission.Name));
                }

                _cachedPermissions = permissions.Distinct().ToList();
                _permissionsLoaded = true;

                return _cachedPermissions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading permissions: {ex.Message}");
                _cachedPermissions = Enumerable.Empty<string>();
                _permissionsLoaded = true;
                return _cachedPermissions;
            }
        }

        public bool IsAdmin => Role == "Admin" || Role == "SuperAdmin" || Role == "Administrator";

        public bool HasPermission(string permission)
        {
            // Check JWT claims first (fast)
            var jwtPermissions = _httpContextAccessor.HttpContext?.User?
                .FindAll("permission")
                .Select(c => c.Value)
                ?? Enumerable.Empty<string>();

            return jwtPermissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// ✅ Async permission check (từ database)
        /// </summary>
        public async Task<bool> HasPermissionAsync(string permission)
        {
            var permissions = await GetPermissionsAsync();
            return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }
}