using HRVault.Application.Roles.DTOs;
using MediatR;

namespace HRVault.Application.Roles.Queries.GetRoles;

public class GetRolesQuery : IRequest<List<RoleDto>>
{
}