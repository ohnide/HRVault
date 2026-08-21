namespace HRVault.Application.WorkSchedules.DTOs;

public class WorkScheduleDayInputDto
{
    public int DayOfWeek { get; set; }
    public bool IsWorkingDay { get; set; }
    public TimeOnly? RequiredDailyTime { get; set; }
    public List<WorkSchedulePeriodInputDto> Periods { get; set; } = new();
}

public class WorkSchedulePeriodInputDto
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
