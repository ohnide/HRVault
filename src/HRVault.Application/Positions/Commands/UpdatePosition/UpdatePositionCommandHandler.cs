using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Positions.Commands.UpdatePosition;

public class UpdatePositionCommandHandler
    : IRequestHandler<UpdatePositionCommand, Guid>
{
    private readonly IPositionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdatePositionCommandHandler(
        IPositionRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        UpdatePositionCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var position = await _repository.GetByIdAndCompanyAsync(
            request.Id,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (position is null)
            throw new NotFoundException("Position not found.");

        position.Code = request.Code;
        position.Name = request.Name;
        position.Description = request.Description;
        position.IsActive = request.IsActive;

        await _repository.UpdateAsync(
            position,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return position.Id;
    }
}