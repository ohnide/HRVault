using MediatR;

namespace HRVault.Application.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommand : IRequest
{
    public DeleteDepartmentCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}