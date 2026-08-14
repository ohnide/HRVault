using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    Guid? DepartmentId,
    Guid? PositionId,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string? WorkEmail,
    string? PersonalEmail,
    string? MobilePhone,
    DateOnly HireDate
) : IRequest<Guid>;