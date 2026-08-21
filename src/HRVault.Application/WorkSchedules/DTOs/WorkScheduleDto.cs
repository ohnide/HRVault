namespace HRVault.Application.WorkSchedules.DTOs;

public class WorkScheduleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int Type { get; set; }
    public string TypeName { get; set; } = string.Empty;

    public int? RequiredWorkingDaysPerWeek { get; set; }

    public bool IsActive { get; set; }
    public List<WorkScheduleDayDto> Days { get; set; } = new();
}

public class WorkScheduleDayDto
{
    public Guid Id { get; set; }
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public bool IsWorkingDay { get; set; }
    public TimeOnly? RequiredDailyTime { get; set; }
    public List<WorkSchedulePeriodDto> Periods { get; set; } = new();
}

public class WorkSchedulePeriodDto
{
    public Guid Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
