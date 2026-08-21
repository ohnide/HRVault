using HRVault.Domain.Enums;
using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class WorkSchedule : SoftDeleteEntity
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public WorkScheduleType Type { get; set; } = WorkScheduleType.Fixed;

    public int? RequiredWorkingDaysPerWeek { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<WorkScheduleDay> Days { get; set; } = new List<WorkScheduleDay>();

    public ICollection<EmployeeWorkSchedule> EmployeeAssignments { get; set; }
        = new List<EmployeeWorkSchedule>();
}
