using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Positions.Commands.CreatePosition;

public class CreatePositionCommandHandler
    : IRequestHandler<CreatePositionCommand, Guid>
{
    private readonly IPositionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreatePositionCommandHandler(
        IPositionRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreatePositionCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var position = new Position
        {
            CompanyId = _currentUser.CompanyId.Value,
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        };

        await _repository.AddAsync(
            position,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return position.Id;
    }
}