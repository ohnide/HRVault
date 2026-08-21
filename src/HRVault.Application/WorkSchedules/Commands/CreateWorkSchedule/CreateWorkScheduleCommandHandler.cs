using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.WorkSchedules.DTOs;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.WorkSchedules.Commands.CreateWorkSchedule;

public class CreateWorkScheduleCommandHandler
    : IRequestHandler<CreateWorkScheduleCommand, Guid>
{
    private readonly IWorkScheduleRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkScheduleCommandHandler(
        IWorkScheduleRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateWorkScheduleCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        WorkScheduleRules.Validate(
            request.Name,
            request.Description,
            request.Type,
            request.RequiredWorkingDaysPerWeek,
            request.Days);

        var companyId = _currentUser.CompanyId.Value;
        var name = request.Name.Trim();

        if (await _repository.NameExistsAsync(name, companyId, cancellationToken: cancellationToken))
            throw new ConflictException("Já existe um horário com este nome.");

        var scheduleType = (WorkScheduleType)request.Type;

        var schedule = new WorkSchedule
        {
            CompanyId = companyId,
            Name = name,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            Type = scheduleType,
            RequiredWorkingDaysPerWeek =
                scheduleType == WorkScheduleType.WeeklyVariable
                    ? request.RequiredWorkingDaysPerWeek
                    : null,
            IsActive = true,
            Days = request.Days.Select(day => MapDay(scheduleType, day)).ToList()
        };

        await _repository.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return schedule.Id;
    }

    private static WorkScheduleDay MapDay(
        WorkScheduleType scheduleType,
        WorkScheduleDayInputDto day)
    {
        return new WorkScheduleDay
        {
            DayOfWeek = (DayOfWeek)day.DayOfWeek,
            IsWorkingDay = day.IsWorkingDay,
            RequiredDailyTime =
                day.IsWorkingDay &&
                (scheduleType == WorkScheduleType.Flexible ||
                 scheduleType == WorkScheduleType.WeeklyVariable)
                    ? day.RequiredDailyTime
                    : null,
            Periods =
                day.IsWorkingDay &&
                scheduleType == WorkScheduleType.Fixed
                    ? day.Periods.Select(p => new WorkSchedulePeriod
                    {
                        StartTime = p.StartTime,
                        EndTime = p.EndTime
                    }).ToList()
                    : new List<WorkSchedulePeriod>()
        };
    }
}
