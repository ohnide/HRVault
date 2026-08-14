using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
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
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        if (_currentUser.CompanyId is null)
        {
            throw new UnauthorizedAccessException(
                "User company could not be determined.");
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