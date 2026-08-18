using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Vacations.Commands.RejectVacationRequest;

public class RejectVacationRequestCommandHandler
    : IRequestHandler<RejectVacationRequestCommand>
{
    private readonly IVacationRequestRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RejectVacationRequestCommandHandler(
        IVacationRequestRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RejectVacationRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var vacation =
            await _repository.GetByIdAsync(
                request.Id,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (vacation is null)
        {
            throw new NotFoundException(
                "Vacation request not found.");
        }

        if (vacation.Status != VacationRequestStatus.Pending)
        {
            throw new BusinessRuleException(
                "Only pending vacation requests can be rejected.");
        }

        vacation.Status =
            VacationRequestStatus.Rejected;

        vacation.ApprovedAt = null;
        vacation.ApprovedBy = null;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}