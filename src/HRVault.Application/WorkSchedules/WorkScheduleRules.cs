using HRVault.Application.Common.Exceptions;
using HRVault.Application.WorkSchedules.DTOs;
using HRVault.Domain.Enums;

namespace HRVault.Application.WorkSchedules;

internal static class WorkScheduleRules
{
    public static void Validate(
        string name,
        string? description,
        int type,
        int? requiredWorkingDaysPerWeek,
        IReadOnlyCollection<WorkScheduleDayInputDto> days)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("O nome do horário é obrigatório.");

        if (name.Trim().Length > 150)
            throw new BusinessRuleException("O nome do horário não pode ultrapassar 150 caracteres.");

        if (description?.Trim().Length > 500)
            throw new BusinessRuleException("A descrição não pode ultrapassar 500 caracteres.");

        if (!Enum.IsDefined(typeof(WorkScheduleType), type))
            throw new BusinessRuleException("O tipo de horário é inválido.");

        if (days.Count != 7)
            throw new BusinessRuleException("O horário deve conter os sete dias da semana.");

        if (days.Any(x => x.DayOfWeek < 0 || x.DayOfWeek > 6))
            throw new BusinessRuleException("Existe um dia da semana inválido.");

        if (days.GroupBy(x => x.DayOfWeek).Any(x => x.Count() > 1))
            throw new BusinessRuleException("Não podem existir dias da semana repetidos.");

        var scheduleType = (WorkScheduleType)type;

        if (scheduleType == WorkScheduleType.WeeklyVariable)
        {
            if (!requiredWorkingDaysPerWeek.HasValue ||
                requiredWorkingDaysPerWeek.Value < 1 ||
                requiredWorkingDaysPerWeek.Value > 7)
                throw new BusinessRuleException("O número de dias de trabalho por semana deve estar entre 1 e 7.");

            if (days.Count(x => x.IsWorkingDay) < requiredWorkingDaysPerWeek.Value)
                throw new BusinessRuleException("Os dias possíveis de trabalho não podem ser inferiores aos dias obrigatórios por semana.");
        }
        else if (requiredWorkingDaysPerWeek.HasValue)
        {
            throw new BusinessRuleException("Os dias obrigatórios por semana só se aplicam ao horário semanal variável.");
        }

        foreach (var day in days)
        {
            if (!day.IsWorkingDay)
            {
                if (day.RequiredDailyTime.HasValue || day.Periods.Count > 0)
                    throw new BusinessRuleException("Um dia não trabalhado não pode ter tempo obrigatório nem períodos.");
                continue;
            }

            switch (scheduleType)
            {
                case WorkScheduleType.Fixed:
                    if (day.RequiredDailyTime.HasValue)
                        throw new BusinessRuleException("Um horário fixo não deve ter tempo diário obrigatório.");
                    if (day.Periods.Count == 0)
                        throw new BusinessRuleException("Um dia de trabalho num horário fixo deve ter pelo menos um período.");
                    if (day.Periods.Any(x => x.StartTime == x.EndTime))
                        throw new BusinessRuleException("A hora de início e fim de um período não podem ser iguais.");
                    break;

                case WorkScheduleType.Flexible:
                case WorkScheduleType.WeeklyVariable:
                    if (!day.RequiredDailyTime.HasValue || day.RequiredDailyTime.Value == TimeOnly.MinValue)
                        throw new BusinessRuleException("O tempo diário obrigatório deve ser superior a 00:00.");
                    if (day.Periods.Count > 0)
                        throw new BusinessRuleException("Este tipo de horário não pode ter períodos fixos.");
                    break;

                case WorkScheduleType.ScheduleExempt:
                    if (day.RequiredDailyTime.HasValue || day.Periods.Count > 0)
                        throw new BusinessRuleException("A isenção de horário não deve ter tempo diário obrigatório nem períodos fixos.");
                    break;
            }
        }
    }
}
