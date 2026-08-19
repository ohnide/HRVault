using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class WorkScheduleDay : AuditableEntity
{
    public Guid WorkScheduleId { get; set; }
    public WorkSchedule WorkSchedule { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    public bool IsWorkingDay { get; set; }

    public ICollection<WorkSchedulePeriod> Periods { get; set; }
        = new List<WorkSchedulePeriod>();
}
