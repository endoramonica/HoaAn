using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Tasks;
using VietCommerce.Core.Models;
using VietCommerce.Application.Extensions;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.AdminAPI.Controllers
{
    /// <summary>
    /// Controller quản lý Task
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<TaskDto>>> GetTasks([FromQuery] PaginationParams pagination, [FromQuery] TaskFilters filters)
        {
            var paginatedResult = await _taskService.GetTasksAsync(pagination, filters);
            var response = paginatedResult.ToResponse();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskRequest request)
        {
            var createdTask = await _taskService.CreateTaskAsync(request);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(string id, [FromBody] UpdateTaskRequest request)
        {
            var updatedTask = await _taskService.UpdateTaskAsync(id, request);
            if (updatedTask == null) return NotFound();
            return Ok(updatedTask);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<TaskDto>> UpdateTaskStatus(string id, [FromBody] TaskStatus status)
        {
            var updatedTask = await _taskService.UpdateTaskStatusAsync(id, status);
            if (updatedTask == null) return NotFound();
            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }

        [HttpPost("{taskId}/assign/{assigneeId}")]
        public async Task<IActionResult> AssignTask(string taskId, string assigneeId)
        {
            await _taskService.AssignTaskAsync(taskId, assigneeId);
            return NoContent();
        }

        [HttpPost("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(string taskId, [FromBody] string completionNote)
        {
            await _taskService.CompleteTaskAsync(taskId, completionNote);
            return NoContent();
        }

        [HttpGet("assignee/{assigneeId}")]
        public async Task<ActionResult<PaginatedResponse<TaskDto>>> GetTasksByAssignee(string assigneeId, [FromQuery] PaginationParams pagination)
        {
            var paginatedResult = await _taskService.GetTasksByAssigneeAsync(assigneeId, pagination);
            var response = paginatedResult.ToResponse();
            return Ok(response);
        }
    }
}
