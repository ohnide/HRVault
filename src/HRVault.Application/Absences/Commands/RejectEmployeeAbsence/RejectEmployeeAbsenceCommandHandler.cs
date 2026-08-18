using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Absences.Commands.RejectEmployeeAbsence;

public class RejectEmployeeAbsenceCommandHandler
    : IRequestHandler<RejectEmployeeAbsenceCommand>
{
    private readonly IEmployeeAbsenceRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RejectEmployeeAbsenceCommandHandler(
        IEmployeeAbsenceRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RejectEmployeeAbsenceCommand request,
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

        if (absence.Status != AbsenceStatus.Pending)
        {
            throw new BusinessRuleException(
                "Only pending absences can be rejected.");
        }

        absence.Status =
            AbsenceStatus.Rejected;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}