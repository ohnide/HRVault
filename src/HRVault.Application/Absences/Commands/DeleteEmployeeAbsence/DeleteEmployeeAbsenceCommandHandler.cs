using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Absences.Commands.DeleteEmployeeAbsence;

public class DeleteEmployeeAbsenceCommandHandler
    : IRequestHandler<DeleteEmployeeAbsenceCommand>
{
    private readonly IEmployeeAbsenceRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeAbsenceCommandHandler(
        IEmployeeAbsenceRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteEmployeeAbsenceCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var absence =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (absence is null)
        {
            throw new NotFoundException(
                "Absence not found.");
        }

        await _repository.DeleteAsync(
            absence,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}