using MediatR;

namespace HRVault.Application.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? ParentDepartmentId { get; set; }
}