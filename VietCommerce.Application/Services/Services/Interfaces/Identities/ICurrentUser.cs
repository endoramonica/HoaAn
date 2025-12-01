using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VietCommerce.Application.Services.Services.Interfaces.Identities
{
    public interface ICurrentUser
    {
        /// <summary>
        /// Current user's unique identifier
        /// </summary>
        Guid UserId { get; }
        /// <summary>
        /// Current customer ID (from JWT token)
        /// </summary>
        Guid CustomerId { get; }

        /// <summary>
        /// Current user's name
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Current store ID (if applicable)
        /// </summary>
        Guid? StoreId { get; }

        /// <summary>
        /// Current user's role (from JWT)
        /// </summary>
        string Role { get; }

        /// <summary>
        /// Permissions from JWT claims (cached)
        /// </summary>
        IEnumerable<string> Permissions { get; }

        /// <summary>
        /// Load permissions from database (async)
        /// </summary>
        Task<IEnumerable<string>> GetPermissionsAsync();

        /// <summary>
        /// Check if user is admin
        /// </summary>
        bool IsAdmin { get; }

        /// <summary>
        /// Check permission from JWT claims (sync)
        /// </summary>
        bool HasPermission(string permission);

        /// <summary>
        /// Check permission from database (async)
        /// </summary>
        Task<bool> HasPermissionAsync(string permission);
    }
}