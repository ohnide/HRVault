using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Absences.Commands.DeleteAbsenceType;

public class DeleteAbsenceTypeCommandHandler
    : IRequestHandler<DeleteAbsenceTypeCommand>
{
    private readonly IAbsenceTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAbsenceTypeCommandHandler(
        IAbsenceTypeRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteAbsenceTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var absenceType =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (absenceType is null)
        {
            throw new NotFoundException(
                "Absence type not found.");
        }

        await _repository.DeleteAsync(
            absenceType,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}