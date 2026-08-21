using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.TimePunches.Commands.CreateTimePunch;

public class CreateTimePunchCommandHandler
    : IRequestHandler<CreateTimePunchCommand, Guid>
{
    private static readonly TimeSpan DuplicateTolerance =
        TimeSpan.FromSeconds(30);

    private readonly ITimePunchRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTimePunchCommandHandler(
        ITimePunchRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateTimePunchCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        if (!Enum.IsDefined(
                typeof(AttendanceEventDirection),
                request.Direction))
        {
            throw new BusinessRuleException(
                "A direção da picagem é inválida.");
        }

        var employeeExists =
            await _repository.EmployeeExistsInCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (!employeeExists)
        {
            throw new NotFoundException(
                "Funcionário não encontrado.");
        }

        var nowUtc = DateTime.UtcNow;

        var duplicate =
            await _repository.HasRecentPunchAsync(
                request.EmployeeId,
                companyId,
                nowUtc,
                DuplicateTolerance,
                cancellationToken);

        if (duplicate)
        {
            throw new ConflictException(
                "Já existe uma picagem recente para este funcionário.");
        }

        var punch = new TimePunch
        {
            CompanyId = companyId,
            EmployeeId = request.EmployeeId,
            TimestampUtc = nowUtc,
            Source = TimePunchSource.HRVault,
            Direction =
                (AttendanceEventDirection)request.Direction,
            AttendanceDeviceId = null,
            AttendanceEventId = null,
            AdjustmentReason = null,
            IsVoided = false
        };

        await _repository.AddAsync(
            punch,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return punch.Id;
    }
}
