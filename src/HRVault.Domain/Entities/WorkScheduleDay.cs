using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class WorkScheduleDay : AuditableEntity
{
    public Guid WorkScheduleId { get; set; }
    public WorkSchedule WorkSchedule { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    // Fixed/Flexible/ScheduleExempt: dia normal de trabalho.
    // WeeklyVariable: dia permitido para integrar a escala semanal.
    public bool IsWorkingDay { get; set; }

    // Flexible e WeeklyVariable. No frontend apresentar em HH:mm.
    public TimeOnly? RequiredDailyTime { get; set; }

    public ICollection<WorkSchedulePeriod> Periods { get; set; }
        = new List<WorkSchedulePeriod>();
}
