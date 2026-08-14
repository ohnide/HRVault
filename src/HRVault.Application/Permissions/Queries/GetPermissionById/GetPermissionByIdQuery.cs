using HRVault.Application.Permissions.DTOs;
using MediatR;

namespace HRVault.Application.Permissions.Queries.GetPermissionById;

public record GetPermissionByIdQuery(
    Guid Id)
    : IRequest<PermissionDto?>;