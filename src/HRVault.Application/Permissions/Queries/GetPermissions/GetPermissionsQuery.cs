using HRVault.Application.Permissions.DTOs;
using MediatR;

namespace HRVault.Application.Permissions.Queries.GetPermissions;

public record GetPermissionsQuery
    : IRequest<List<PermissionDto>>;