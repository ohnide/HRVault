using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Vacations.Commands.ApproveVacationRequest;

public class ApproveVacationRequestCommandHandler
    : IRequestHandler<ApproveVacationRequestCommand>
{
    private readonly IVacationRequestRepository _repository;
    private readonly IVacationBalanceRepository _balanceRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveVacationRequestCommandHandler(
        IVacationRequestRepository repository,
        IVacationBalanceRepository balanceRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _balanceRepository = balanceRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ApproveVacationRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var companyId =
            _currentUser.CompanyId.Value;

        var vacation =
            await _repository.GetByIdAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (vacation is null)
        {
            throw new NotFoundException(
                "Vacation request not found.");
        }

        if (vacation.Status != VacationRequestStatus.Pending)
        {
            throw new BusinessRuleException(
                "Only pending vacation requests can be approved.");
        }

        var year =
            vacation.StartDate.Year;

        var balance =
            await _balanceRepository.GetByEmployeeAndYearAsync(
                vacation.EmployeeId,
                year,
                companyId,
                cancellationToken);

        if (balance is null)
        {
            throw new BusinessRuleException(
                "Vacation balance not found for this employee and year.");
        }

        var totalAvailable =
            balance.EntitledDays +
            balance.CarriedOverDays +
            balance.AdjustmentDays;

        var approvedDays =
            await _repository.GetApprovedDaysForYearAsync(
                vacation.EmployeeId,
                year,
                companyId,
                vacation.Id,
                cancellationToken);

        var remaining =
            totalAvailable - approvedDays;

        if (vacation.Days > remaining)
        {
            throw new BusinessRuleException(
                $"Insufficient vacation balance. Available: {remaining} days.");
        }

        vacation.Status =
            VacationRequestStatus.Approved;

        vacation.ApprovedAt =
            DateTime.UtcNow;

        vacation.ApprovedBy =
            _currentUser.UserId;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}