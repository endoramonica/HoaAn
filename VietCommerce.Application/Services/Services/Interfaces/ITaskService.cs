using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Tasks;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface ITaskService
    {
        Task<PaginatedResponse<TaskDto>> GetTasksAsync(PaginationParams pagination, TaskFilters filters);
        Task<TaskDto> GetTaskByIdAsync(string id);
        Task<TaskDto> CreateTaskAsync(CreateTaskRequest request);
        Task<TaskDto> UpdateTaskAsync(string id, UpdateTaskRequest request);
        Task<TaskDto> UpdateTaskStatusAsync(string id, TaskStatus status);
        Task DeleteTaskAsync(string id);
        Task AssignTaskAsync(string taskId, string assigneeId);
        Task CompleteTaskAsync(string taskId, string completionNote);
        Task<PaginatedResponse<TaskDto>> GetTasksByAssigneeAsync(string assigneeId, PaginationParams pagination);
    }
}
