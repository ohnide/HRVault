using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand, Guid>
{
    private readonly IRoleRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateRoleCommandHandler(
        IRoleRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var role = await _repository.GetByIdAndCompanyAsync(
            request.Id,
            companyId,
            cancellationToken);

        if (role is null)
        {
            throw new NotFoundException(
                "Role not found.");
        }

        var nameExists =
            await _repository.NameExistsAsync(
                request.Name,
                companyId,
                request.Id,
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException(
                "A role with this name already exists.");
        }

        role.Name = request.Name;
        role.Description = request.Description;

        await _repository.UpdateAsync(
            role,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return role.Id;
    }
}