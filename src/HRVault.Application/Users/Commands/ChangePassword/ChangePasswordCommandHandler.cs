using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository repository,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        if (_currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException(
                "User identity could not be determined.");
        }

        if (_currentUser.CompanyId is null)
        {
            throw new UnauthorizedAccessException(
                "User company could not be determined.");
        }

        // O utilizador só pode alterar a própria password.
        if (_currentUser.UserId.Value != request.UserId)
        {
            throw new UnauthorizedAccessException(
                "You can only change your own password.");
        }

        var user = await _repository.GetByIdAndCompanyAsync(
            request.UserId,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                "User not found.");
        }

        if (!_passwordHasher.Verify(
                request.CurrentPassword,
                user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Current password is incorrect.");
        }

        user.PasswordHash =
            _passwordHasher.Hash(request.NewPassword);

        user.PasswordChangedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}