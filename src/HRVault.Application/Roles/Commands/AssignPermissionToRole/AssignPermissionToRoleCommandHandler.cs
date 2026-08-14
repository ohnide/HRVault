using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Roles.Commands.AssignPermissionToRole;

public class AssignPermissionToRoleCommandHandler
    : IRequestHandler<AssignPermissionToRoleCommand>
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AssignPermissionToRoleCommandHandler(
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
        AssignPermissionToRoleCommand request,
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

        var exists = await _rolePermissionRepository.ExistsAsync(
            request.RoleId,
            request.PermissionId,
            cancellationToken);

        if (exists)
        {
            throw new ConflictException(
                "Permission is already assigned to this role.");
        }

        var rolePermission = new RolePermission
        {
            RoleId = request.RoleId,
            PermissionId = request.PermissionId
        };

        await _rolePermissionRepository.AddAsync(
            rolePermission,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}