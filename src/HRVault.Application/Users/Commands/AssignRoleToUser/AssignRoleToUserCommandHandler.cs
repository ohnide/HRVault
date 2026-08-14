using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Users.Commands.AssignRoleToUser;

public class AssignRoleToUserCommandHandler
    : IRequestHandler<AssignRoleToUserCommand>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public AssignRoleToUserCommandHandler(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task Handle(
        AssignRoleToUserCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var user = await _userRepository.GetByIdAndCompanyAsync(
            request.UserId,
            companyId,
            cancellationToken);

        if (user is null)
            throw new NotFoundException(
                "User not found.");

        var role = await _roleRepository.GetByIdAndCompanyAsync(
            request.RoleId,
            companyId,
            cancellationToken);

        if (role is null)
            throw new NotFoundException(
                "Role not found.");

        var existing = await _userRoleRepository.GetAsync(
            request.UserId,
            request.RoleId,
            cancellationToken);

        if (existing is not null)
            throw new ConflictException(
                "Role is already assigned to this user.");

        var userRole = new UserRole
        {
            UserId = request.UserId,
            RoleId = request.RoleId
        };

        await _userRoleRepository.AddAsync(
            userRole,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}