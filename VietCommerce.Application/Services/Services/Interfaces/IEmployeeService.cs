using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Get paginated list of employees with filtering
        /// </summary>
        Task<PaginatedResult<EmployeeDto>> GetEmployeesAsync(
            PaginationParams pagination,
            EmployeeFilters filters);

        /// <summary>
        /// Get employee by ID
        /// </summary>
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid id);

        /// <summary>
        /// Create new employee
        /// </summary>
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);

        /// <summary>
        /// Update existing employee
        /// </summary>
        Task<EmployeeDto> UpdateEmployeeAsync(Guid id, UpdateEmployeeRequest request);

        /// <summary>
        /// Delete employee
        /// </summary>
        Task DeleteEmployeeAsync(Guid id);
    }
}
