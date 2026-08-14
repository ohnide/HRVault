using HRVault.Application.Roles.DTOs;
using MediatR;

namespace HRVault.Application.Roles.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest<RoleDto?>
{
    public Guid Id { get; }

    public GetRoleByIdQuery(Guid id)
    {
        Id = id;
    }
}