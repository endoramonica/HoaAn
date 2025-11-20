using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        /// <summary>
        /// Get paginated employees with filters
        /// </summary>
        Task<(List<Employee> Items, int TotalCount)> GetEmployeesPagedAsync(
            PaginationParams pagination,
            EmployeeFilters filters);

        /// <summary>
        /// Get employee with all related data (User, Manager, Store)
        /// </summary>
        Task<Employee?> GetEmployeeWithDetailsAsync(Guid userId);

        /// <summary>
        /// Check if employee code exists (excluding specific userId for update scenario)
        /// </summary>
        Task<bool> IsEmployeeCodeExistsAsync(string code, Guid? excludeUserId = null);

        /// <summary>
        /// Check if employee has any leave requests
        /// </summary>
        Task<bool> HasLeaveRequestsAsync(Guid userId);

        /// <summary>
        /// Check if employee has any subordinates
        /// </summary>
        Task<bool> HasSubordinatesAsync(Guid managerId);

        /// <summary>
        /// Get employees by department
        /// </summary>
        Task<List<Employee>> GetEmployeesByDepartmentAsync(string department);

        /// <summary>
        /// Get employees by manager
        /// </summary>
        Task<List<Employee>> GetEmployeesByManagerAsync(Guid managerId);

        /// <summary>
        /// Get employees by store
        /// </summary>
        Task<List<Employee>> GetEmployeesByStoreAsync(Guid storeId);

        /// <summary>
        /// Get employees by status
        /// </summary>
        Task<List<Employee>> GetEmployeesByStatusAsync(EmployeeStatus status);

        /// <summary>
        /// Search employees by keyword (searches in User.Name, User.Email, Code)
        /// </summary>
        Task<List<Employee>> SearchEmployeesAsync(string keyword);
    }
}