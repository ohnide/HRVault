using HRVault.Application.Absences.DTOs;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Absences.Queries.GetEmployeeAbsenceById;

public class GetEmployeeAbsenceByIdQueryHandler
    : IRequestHandler<
        GetEmployeeAbsenceByIdQuery,
        EmployeeAbsenceDto?>
{
    private readonly IEmployeeAbsenceRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeAbsenceByIdQueryHandler(
        IEmployeeAbsenceRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<EmployeeAbsenceDto?> Handle(
        GetEmployeeAbsenceByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var absence =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (absence is null)
            return null;

        return new EmployeeAbsenceDto
        {
            Id = absence.Id,
            EmployeeId = absence.EmployeeId,
            EmployeeName =
                $"{absence.Employee.FirstName} {absence.Employee.LastName}",
            AbsenceTypeId =
                absence.AbsenceTypeId,
            AbsenceTypeName =
                absence.AbsenceType.Name,
            StartDateTime =
                absence.StartDateTime,
            EndDateTime =
                absence.EndDateTime,
            Status =
                absence.Status.ToString(),
            Reason =
                absence.Reason,
            Notes =
                absence.Notes
        };
    }
}