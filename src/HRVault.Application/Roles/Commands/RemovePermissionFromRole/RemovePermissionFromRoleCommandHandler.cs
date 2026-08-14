using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Roles.Commands.RemovePermissionFromRole;

public class RemovePermissionFromRoleCommandHandler
    : IRequestHandler<RemovePermissionFromRoleCommand>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public RemovePermissionFromRoleCommandHandler(
        IRolePermissionRepository rolePermissionRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task Handle(
        RemovePermissionFromRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var role = await _roleRepository.GetByIdAndCompanyAsync(
            request.RoleId,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (role is null)
        {
            throw new NotFoundException(
                "Role not found.");
        }

        var rolePermission =
            await _rolePermissionRepository.GetAsync(
                request.RoleId,
                request.PermissionId,
                cancellationToken);

        if (rolePermission is null)
        {
            throw new NotFoundException(
                "Permission is not assigned to this role.");
        }

        await _rolePermissionRepository.DeleteAsync(
            rolePermission,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}