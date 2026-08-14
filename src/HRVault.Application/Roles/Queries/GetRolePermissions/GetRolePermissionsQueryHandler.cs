using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Roles.DTOs;
using MediatR;

namespace HRVault.Application.Roles.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler
    : IRequestHandler<
        GetRolePermissionsQuery,
        List<RolePermissionDto>>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUser;

    public GetRolePermissionsQueryHandler(
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUser)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _currentUser = currentUser;
    }

    public async Task<List<RolePermissionDto>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var role = await _roleRepository.GetByIdAndCompanyAsync(
            request.RoleId,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (role is null)
            throw new NotFoundException(
                "Role not found.");

        var rolePermissions =
            await _rolePermissionRepository.GetByRoleIdAsync(
                request.RoleId,
                cancellationToken);

        return rolePermissions
            .Select(x => new RolePermissionDto
            {
                PermissionId = x.PermissionId,
                Code = x.Permission.Code,
                Name = x.Permission.Name,
                Description = x.Permission.Description
            })
            .ToList();
    }
}