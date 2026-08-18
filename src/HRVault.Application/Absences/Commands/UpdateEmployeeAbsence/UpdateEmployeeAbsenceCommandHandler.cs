using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Absences.Commands.UpdateEmployeeAbsence;

public class UpdateEmployeeAbsenceCommandHandler
    : IRequestHandler<UpdateEmployeeAbsenceCommand>
{
    private readonly IEmployeeAbsenceRepository _absenceRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAbsenceTypeRepository _absenceTypeRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeAbsenceCommandHandler(
        IEmployeeAbsenceRepository absenceRepository,
        IEmployeeRepository employeeRepository,
        IAbsenceTypeRepository absenceTypeRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _absenceRepository = absenceRepository;
        _employeeRepository = employeeRepository;
        _absenceTypeRepository = absenceTypeRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateEmployeeAbsenceCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        if (request.EndDateTime <= request.StartDateTime)
        {
            throw new BusinessRuleException(
                "End date must be greater than start date.");
        }

        var absence =
            await _absenceRepository.GetByIdAndCompanyAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (absence is null)
        {
            throw new NotFoundException(
                "Absence not found.");
        }

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        var absenceType =
            await _absenceTypeRepository.GetByIdAndCompanyAsync(
                request.AbsenceTypeId,
                companyId,
                cancellationToken);

        if (absenceType is null)
        {
            throw new NotFoundException(
                "Absence type not found.");
        }

        var hasOverlap =
            await _absenceRepository.HasOverlapAsync(
                request.EmployeeId,
                request.StartDateTime,
                request.EndDateTime,
                companyId,
                request.Id,
                cancellationToken);

        if (hasOverlap)
        {
            throw new ConflictException(
                "The employee already has an overlapping absence.");
        }

        absence.EmployeeId =
            request.EmployeeId;

        absence.AbsenceTypeId =
            request.AbsenceTypeId;

        absence.StartDateTime =
            request.StartDateTime;

        absence.EndDateTime =
            request.EndDateTime;

        absence.Status =
            request.Status;

        absence.Reason =
            string.IsNullOrWhiteSpace(
                request.Reason)
                ? null
                : request.Reason.Trim();

        absence.Notes =
            string.IsNullOrWhiteSpace(
                request.Notes)
                ? null
                : request.Notes.Trim();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}