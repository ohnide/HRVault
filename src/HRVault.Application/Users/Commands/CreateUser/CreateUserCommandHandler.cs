using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var emailExists =
            await _userRepository.EmailExistsAsync(
                request.Email,
                cancellationToken: cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "A user with this email already exists.");
        }

        if (request.EmployeeId.HasValue)
        {
            var employee =
                await _employeeRepository.GetByIdAndCompanyAsync(
                    request.EmployeeId.Value,
                    companyId,
                    cancellationToken);

            if (employee is null)
            {
                throw new NotFoundException(
                    "Employee not found.");
            }
        }

        var user = new User
        {
            CompanyId = companyId,
            EmployeeId = request.EmployeeId,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(
                request.Password),
            IsAdministrator = request.IsAdministrator,
            IsActive = request.IsActive,
            PasswordChangedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return user.Id;
    }
}