using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler
    : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateCompanyCommandHandler(
        ICompanyRepository companyRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUserRoleRepository userRoleRepository,
        IPermissionRepository permissionRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _userRoleRepository = userRoleRepository;
        _permissionRepository = permissionRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        if (!_currentUser.IsPlatformAdministrator)
        {
            throw new ForbiddenException(
                "Platform administrator access is required.");
        }

        var emailExists =
            await _userRepository.EmailExistsAsync(
                request.AdministratorEmail,
                cancellationToken: cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "A user with this email already exists.");
        }

        return await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                var company = new Company
                {
                    Name = request.Name,
                    VatNumber = request.VatNumber,
                    Address = request.Address,
                    LogoUrl = request.LogoUrl
                };

                await _companyRepository.AddAsync(
                    company,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                var adminRole = new Role
                {
                    CompanyId = company.Id,
                    Name = "Administrador",
                    Description = "Acesso total ao sistema"
                };

                await _roleRepository.AddAsync(
                    adminRole,
                    cancellationToken);

                var adminUser = new User
                {
                    CompanyId = company.Id,
                    EmployeeId = null,
                    Name = request.AdministratorName,
                    Email = request.AdministratorEmail,
                    PasswordHash =
                        _passwordHasher.Hash(
                            request.AdministratorPassword),
                    IsAdministrator = true,
                    IsPlatformAdministrator = false,
                    IsActive = true,
                    PasswordChangedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(
                    adminUser,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                var permissions =
                    await _permissionRepository.GetAllActiveAsync(
                        cancellationToken);

                foreach (var permission in permissions)
                {
                    await _rolePermissionRepository.AddAsync(
                        new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = permission.Id
                        },
                        cancellationToken);
                }

                await _userRoleRepository.AddAsync(
                    new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id
                    },
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                return company.Id;
            },
            cancellationToken);
    }
}