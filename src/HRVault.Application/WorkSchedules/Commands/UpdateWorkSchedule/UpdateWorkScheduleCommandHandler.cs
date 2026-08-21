using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.WorkSchedules.DTOs;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.WorkSchedules.Commands.UpdateWorkSchedule;

public class UpdateWorkScheduleCommandHandler
    : IRequestHandler<UpdateWorkScheduleCommand>
{
    private readonly IWorkScheduleRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkScheduleCommandHandler(
        IWorkScheduleRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        WorkScheduleRules.Validate(
            request.Name,
            request.Description,
            request.Type,
            request.RequiredWorkingDaysPerWeek,
            request.Days);

        var companyId =
            _currentUser.CompanyId.Value;

        var schedule =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (schedule is null)
            throw new NotFoundException(
                "Horário não encontrado.");

        var name = request.Name.Trim();

        if (await _repository.NameExistsAsync(
                name,
                companyId,
                request.Id,
                cancellationToken))
        {
            throw new ConflictException(
                "Já existe um horário com este nome.");
        }

        var scheduleType =
            (WorkScheduleType)request.Type;

        schedule.Name = name;
        schedule.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

        schedule.Type = scheduleType;

        schedule.RequiredWorkingDaysPerWeek =
            scheduleType == WorkScheduleType.WeeklyVariable
                ? request.RequiredWorkingDaysPerWeek
                : null;

        foreach (var requestDay in request.Days)
        {
            var dayOfWeek =
                (DayOfWeek)requestDay.DayOfWeek;

            var existingDay =
                schedule.Days.FirstOrDefault(
                    x => x.DayOfWeek == dayOfWeek);

            if (existingDay is null)
            {
                existingDay = new WorkScheduleDay
                {
                    DayOfWeek = dayOfWeek,
                    WorkScheduleId = schedule.Id,
                    WorkSchedule = schedule
                };

                schedule.Days.Add(existingDay);
            }

            existingDay.IsWorkingDay =
                requestDay.IsWorkingDay;

            existingDay.RequiredDailyTime =
                requestDay.IsWorkingDay &&
                (scheduleType == WorkScheduleType.Flexible ||
                 scheduleType == WorkScheduleType.WeeklyVariable)
                    ? requestDay.RequiredDailyTime
                    : null;

            if (scheduleType != WorkScheduleType.Fixed ||
                !requestDay.IsWorkingDay)
            {
                if (existingDay.Periods.Count > 0)
                {
                    await _repository.DeletePeriodsAsync(
                        existingDay.Periods.ToList(),
                        cancellationToken);

                    existingDay.Periods.Clear();
                }

                continue;
            }

            var existingPeriods =
                existingDay.Periods
                    .OrderBy(x => x.StartTime)
                    .ThenBy(x => x.EndTime)
                    .ToList();

            var requestedPeriods =
                requestDay.Periods
                    .OrderBy(x => x.StartTime)
                    .ThenBy(x => x.EndTime)
                    .ToList();

            var commonCount =
                Math.Min(
                    existingPeriods.Count,
                    requestedPeriods.Count);

            // Atualiza apenas períodos já existentes.
            for (var i = 0; i < commonCount; i++)
            {
                existingPeriods[i].StartTime =
                    requestedPeriods[i].StartTime;

                existingPeriods[i].EndTime =
                    requestedPeriods[i].EndTime;
            }

            // Remove períodos excedentes.
            if (existingPeriods.Count >
                requestedPeriods.Count)
            {
                var periodsToDelete =
                    existingPeriods
                        .Skip(requestedPeriods.Count)
                        .ToList();

                await _repository.DeletePeriodsAsync(
                    periodsToDelete,
                    cancellationToken);

                foreach (var period in periodsToDelete)
                {
                    existingDay.Periods.Remove(period);
                }
            }

            // Cria períodos novos EXPLICITAMENTE através do repository.
            // Assim o EF recebe EntityState.Added e gera INSERT.
            if (requestedPeriods.Count >
                existingPeriods.Count)
            {
                foreach (var requestedPeriod
                         in requestedPeriods
                             .Skip(existingPeriods.Count))
                {
                    var newPeriod =
                        new WorkSchedulePeriod
                        {
                            WorkScheduleDayId =
                                existingDay.Id,

                            WorkScheduleDay =
                                existingDay,

                            StartTime =
                                requestedPeriod.StartTime,

                            EndTime =
                                requestedPeriod.EndTime
                        };

                    await _repository.AddPeriodAsync(
                        newPeriod,
                        cancellationToken);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
