using MediatR;

namespace HRVault.Application.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? ParentDepartmentId { get; set; }
}