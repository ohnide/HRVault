using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.TimePunches.Commands.CreateManualTimePunch;

public class CreateManualTimePunchCommandHandler
    : IRequestHandler<CreateManualTimePunchCommand, Guid>
{
    private static readonly TimeSpan DuplicateTolerance = TimeSpan.FromSeconds(30);

    private readonly ITimePunchRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICompanyTimeZoneService _timeZoneService;

    public CreateManualTimePunchCommandHandler(
        ITimePunchRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        ICompanyTimeZoneService timeZoneService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _timeZoneService = timeZoneService;
    }

    public async Task<Guid> Handle(
        CreateManualTimePunchCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        if (!Enum.IsDefined(typeof(AttendanceEventDirection), request.Direction))
            throw new BusinessRuleException("A direção da picagem é inválida.");

        var direction = (AttendanceEventDirection)request.Direction;

        if (direction == AttendanceEventDirection.Unknown)
            throw new BusinessRuleException("Selecione Entrada ou Saída.");

        var reason = request.Reason?.Trim();

        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessRuleException("O motivo da picagem manual é obrigatório.");

        if (reason.Length > 500)
            throw new BusinessRuleException(
                "O motivo da picagem manual não pode exceder 500 caracteres.");

        var employeeExists = await _repository.EmployeeExistsInCompanyAsync(
            request.EmployeeId, companyId, cancellationToken);

        if (!employeeExists)
            throw new NotFoundException("Funcionário não encontrado.");

        var timeZone = await _timeZoneService.GetAsync(
            companyId, cancellationToken);

        var localDateTime = request.LocalDate.ToDateTime(
            request.LocalTime, DateTimeKind.Unspecified);

        if (timeZone.IsInvalidTime(localDateTime))
            throw new BusinessRuleException(
                "A data/hora indicada não existe na timezone da empresa devido à mudança de hora.");

        if (timeZone.IsAmbiguousTime(localDateTime))
            throw new BusinessRuleException(
                "A data/hora indicada é ambígua devido à mudança de hora.");

        var timestampUtc = TimeZoneInfo.ConvertTimeToUtc(
            localDateTime, timeZone);

        var duplicate = await _repository.HasRecentPunchAsync(
            request.EmployeeId,
            companyId,
            timestampUtc,
            DuplicateTolerance,
            cancellationToken);

        if (duplicate)
            throw new ConflictException(
                "Já existe uma picagem para este funcionário muito próxima da data/hora indicada.");

        var punch = new TimePunch
        {
            CompanyId = companyId,
            EmployeeId = request.EmployeeId,
            TimestampUtc = timestampUtc,
            Source = TimePunchSource.ManualAdjustment,
            Direction = direction,
            AttendanceDeviceId = null,
            AttendanceEventId = null,
            AdjustmentReason = reason,
            IsVoided = false
        };

        await _repository.AddAsync(punch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return punch.Id;
    }
}
