using HRVault.Application.Roles.DTOs;
using MediatR;

namespace HRVault.Application.Roles.Queries.GetRolePermissions;

public record GetRolePermissionsQuery(
    Guid RoleId) : IRequest<List<RolePermissionDto>>;