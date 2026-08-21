using HRVault.Application.WorkSchedules.DTOs;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;

namespace HRVault.Application.WorkSchedules;

internal static class WorkScheduleMapping
{
    public static WorkScheduleDto ToDto(WorkSchedule schedule)
    {
        return new WorkScheduleDto
        {
            Id = schedule.Id,
            Name = schedule.Name,
            Description = schedule.Description,
            Type = (int)schedule.Type,
            TypeName = schedule.Type switch
            {
                WorkScheduleType.Fixed => "Fixo",
                WorkScheduleType.Flexible => "Livre",
                WorkScheduleType.WeeklyVariable => "Semanal variável",
                WorkScheduleType.ScheduleExempt => "Isenção de horário",
                _ => schedule.Type.ToString()
            },
            RequiredWorkingDaysPerWeek = schedule.RequiredWorkingDaysPerWeek,
            IsActive = schedule.IsActive,
            Days = schedule.Days
                .OrderBy(x => x.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)x.DayOfWeek)
                .Select(day => new WorkScheduleDayDto
                {
                    Id = day.Id,
                    DayOfWeek = (int)day.DayOfWeek,
                    DayName = day.DayOfWeek switch
                    {
                        DayOfWeek.Monday => "Segunda-feira",
                        DayOfWeek.Tuesday => "Terça-feira",
                        DayOfWeek.Wednesday => "Quarta-feira",
                        DayOfWeek.Thursday => "Quinta-feira",
                        DayOfWeek.Friday => "Sexta-feira",
                        DayOfWeek.Saturday => "Sábado",
                        DayOfWeek.Sunday => "Domingo",
                        _ => day.DayOfWeek.ToString()
                    },
                    IsWorkingDay = day.IsWorkingDay,
                    RequiredDailyTime = day.RequiredDailyTime,
                    Periods = day.Periods.OrderBy(x => x.StartTime)
                        .Select(p => new WorkSchedulePeriodDto
                        {
                            Id = p.Id,
                            StartTime = p.StartTime,
                            EndTime = p.EndTime
                        }).ToList()
                }).ToList()
        };
    }
}
