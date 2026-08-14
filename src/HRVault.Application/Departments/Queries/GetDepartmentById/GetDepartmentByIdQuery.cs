using HRVault.Application.Departments.DTOs;
using MediatR;

namespace HRVault.Application.Departments.Queries.GetDepartmentById;

public class GetDepartmentByIdQuery : IRequest<DepartmentDto?>
{
    public Guid Id { get; }

    public GetDepartmentByIdQuery(Guid id)
    {
        Id = id;
    }
}