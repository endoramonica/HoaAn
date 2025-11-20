using AutoMapper;
using System;
using VietCommerce.Core.DTOs.Tasks;
using VietCommerce.Core.Entities.Tasks;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for WorkTask entity and DTOs
    /// Handles bidirectional mapping with computed properties
    /// </summary>
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            // ========================================
            // WorkTask → TaskDto (Response mapping)
            // ========================================
            CreateMap<WorkTask, TaskDto>()
                // Basic fields
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))

                // Assignee info (from navigation property)
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.AssignedToName,
                    opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.Name ?? "Unknown" : "Unknown"))
                .ForMember(dest => dest.AssignedToEmail,
                    opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.Email : null))

                // Creator info (from navigation property)
                .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.AssignedBy))
                .ForMember(dest => dest.AssignedByName,
                    opt => opt.MapFrom(src => src.AssignedByUser != null ? src.AssignedByUser.Name ?? "Unknown" : "Unknown"))

                // Task details
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Priority.ToString()))

                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))

                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))

                // Audit fields
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))

                // Computed properties are handled in TaskDto itself (IsOverdue, DaysUntilDue)
                .ForMember(dest => dest.IsOverdue, opt => opt.Ignore())
                .ForMember(dest => dest.DaysUntilDue, opt => opt.Ignore());

            // ========================================
            // CreateTaskRequest → WorkTask
            // ========================================
            CreateMap<CreateTaskRequest, WorkTask>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))

                // Set default values
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TaskStatus.Pending))
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())

                // AssignedBy will be set in service from ICurrentUser
                .ForMember(dest => dest.AssignedBy, opt => opt.Ignore())

                // BaseEntity fields (handled by BaseEntity constructor or service)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())

                // Navigation properties (handled by EF Core)
                .ForMember(dest => dest.AssignedToUser, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUser, opt => opt.Ignore());

            // ========================================
            // UpdateTaskRequest → WorkTask (Partial update)
            // ========================================
            CreateMap<UpdateTaskRequest, WorkTask>()
                .ForMember(dest => dest.Title, opt => opt.Condition(src => src.Title != null))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.AssignedTo, opt => opt.Condition(src => src.AssignedTo.HasValue))
                .ForMember(dest => dest.Priority, opt => opt.Condition(src => src.Priority.HasValue))
                .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue))
                .ForMember(dest => dest.DueDate, opt => opt.Condition(src => src.DueDate.HasValue))

                // Special handling: Set CompletedAt when status changes to Completed
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom((src, dest) =>
                    src.Status == TaskStatus.Completed && dest.CompletedAt == null
                        ? DateTime.UtcNow
                        : dest.CompletedAt))

                // UpdatedAt will be handled by service
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())

                // AssignedBy cannot be changed
                .ForMember(dest => dest.AssignedBy, opt => opt.Ignore())

                // BaseEntity fields
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())

                // Navigation properties
                .ForMember(dest => dest.AssignedToUser, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUser, opt => opt.Ignore());
        }
    }
}