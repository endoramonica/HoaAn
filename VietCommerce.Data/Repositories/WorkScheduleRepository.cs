using VietCommerce.Core.Entities.HRM;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

// WorkScheduleRepository.cs
public class WorkScheduleRepository : GenericRepository<WorkSchedule>, IWorkScheduleRepository
{
    public WorkScheduleRepository(AppDbContext context) : base(context)
    {
    }
}
