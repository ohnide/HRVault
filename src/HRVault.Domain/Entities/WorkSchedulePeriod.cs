using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class WorkSchedulePeriod : AuditableEntity
{
    public Guid WorkScheduleDayId { get; set; }
    public WorkScheduleDay WorkScheduleDay { get; set; } = null!;

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
