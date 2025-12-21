using AutoMapper;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.DTOs.HRM;

namespace VietCommerce.Application.Mappings;

public class HRMMappingProfile : Profile
{
    public HRMMappingProfile()
    {
        // ========== EMPLOYEE MAPPINGS ==========

        // CreateEmployeeRequest → Employee
        CreateMap<CreateEmployeeRequest, Employee>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Manager, opt => opt.Ignore())
            .ForMember(dest => dest.Store, opt => opt.Ignore())
            .ForMember(dest => dest.Subordinates, opt => opt.Ignore())
            .ForMember(dest => dest.Performance, opt => opt.Ignore())
            .ForMember(dest => dest.LeaveRequests, opt => opt.Ignore())
            .ForMember(dest => dest.WorkSchedules, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EmployeeStatus.Active));

        // Employee → EmployeeDto
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.AvatarUrl))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src =>
                src.Manager != null ? src.Manager.User.Name : null))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src =>
                src.Store != null ? src.Store.Name : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt));

        // UpdateEmployeeRequest → Employee (partial update)
        CreateMap<UpdateEmployeeRequest, Employee>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ========== LEAVE REQUEST MAPPINGS ==========

        // LeaveRequest → LeaveRequestDto
        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                src.Employee.User.Name)) // ✅ Sửa: src.Employee.User.Name thay vì src.Employee.Name
            .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src =>
                src.ApprovedByEmployee != null ? src.ApprovedByEmployee.User.Name : null)); // ✅ Sửa tương tự

        // CreateLeaveRequestRequest → LeaveRequest
        CreateMap<CreateLeaveRequestRequest, LeaveRequest>()
            .ForMember(dest => dest.Days, opt => opt.MapFrom(src =>
                (src.EndDate - src.StartDate).Days + 1))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => LeaveRequestStatus.Pending))
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.ApprovedByEmployee, opt => opt.Ignore());

        // ========== WORK SCHEDULE MAPPINGS ==========

        // WorkSchedule → WorkScheduleDto
        CreateMap<WorkSchedule, WorkScheduleDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>
                src.Employee.User.Name)); // ✅ Sửa: src.Employee.User.Name

        // CreateWorkScheduleRequest → WorkSchedule
        CreateMap<CreateWorkScheduleRequest, WorkSchedule>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());

        // UpdateWorkScheduleRequest → WorkSchedule (partial update)
        CreateMap<UpdateWorkScheduleRequest, WorkSchedule>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ========== SHIFT MAPPINGS ==========

        // Shift → ShiftDto
        CreateMap<Shift, ShiftDto>()
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src =>
                src.Staff != null ? src.Staff.Name : null)) // ✅ Handle null Staff
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src =>
                src.Store != null ? src.Store.Name : null)); // ✅ Handle null Store

        // OpenShiftRequest → Shift
        CreateMap<OpenShiftRequest, Shift>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Staff, opt => opt.Ignore())
            .ForMember(dest => dest.Store, opt => opt.Ignore());
    }
}