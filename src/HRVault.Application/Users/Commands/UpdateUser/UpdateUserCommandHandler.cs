using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var user = await _userRepository.GetByIdAndCompanyAsync(
            request.Id,
            companyId,
            cancellationToken);

        if (user is null)
            throw new NotFoundException(
                "User not found.");

        var emailExists =
            await _userRepository.EmailExistsAsync(
                request.Email,
                request.Id,
                cancellationToken);

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

        user.EmployeeId = request.EmployeeId;
        user.Name = request.Name;
        user.Email = request.Email;
        user.IsAdministrator = request.IsAdministrator;
        user.IsActive = request.IsActive;

        await _userRepository.UpdateAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return user.Id;
    }
}