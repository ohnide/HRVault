using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Users.Commands.RemoveRoleFromUser;

public class RemoveRoleFromUserCommandHandler
    : IRequestHandler<RemoveRoleFromUserCommand>
{
    private readonly IUserRoleRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleFromUserCommandHandler(
        IUserRoleRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RemoveRoleFromUserCommand request,
        CancellationToken cancellationToken)
    {
        var userRole = await _repository.GetAsync(
            request.UserId,
            request.RoleId,
            cancellationToken);

        if (userRole is null)
            throw new Exception(
                "Role is not assigned to this user.");

        await _repository.DeleteAsync(
            userRole,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}