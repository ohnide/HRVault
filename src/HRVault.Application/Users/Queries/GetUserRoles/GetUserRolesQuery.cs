using HRVault.Application.Roles.DTOs;
using MediatR;

namespace HRVault.Application.Users.Queries.GetUserRoles;

public record GetUserRolesQuery(
    Guid UserId
) : IRequest<List<RoleDto>>;