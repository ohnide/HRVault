using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? PositionId { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? WorkEmail { get; set; }

    public string? PersonalEmail { get; set; }

    public string? MobilePhone { get; set; }

    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public int Status { get; set; }
}