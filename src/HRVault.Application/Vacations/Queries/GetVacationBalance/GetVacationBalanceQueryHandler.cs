using HRVault.Application.Common.Interfaces;
using HRVault.Application.Vacations.DTOs;
using MediatR;

namespace HRVault.Application.Vacations.Queries.GetVacationBalance;

public class GetVacationBalanceQueryHandler
    : IRequestHandler<GetVacationBalanceQuery, VacationBalanceDto?>
{
    private readonly IVacationBalanceRepository _repository;
    private readonly ICurrentUserService _currentUser;
	private readonly IVacationRequestRepository _vacationRequestRepository;

    public GetVacationBalanceQueryHandler(
		IVacationBalanceRepository repository,
		IVacationRequestRepository vacationRequestRepository,
		ICurrentUserService currentUser)
	{
		_repository = repository;
		_vacationRequestRepository = vacationRequestRepository;
		_currentUser = currentUser;
	}

    public async Task<VacationBalanceDto?> Handle(
        GetVacationBalanceQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var balance =
            await _repository.GetByEmployeeAndYearAsync(
                request.EmployeeId,
                request.Year,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (balance is null)
            return null;

		var approvedDays =
			await _vacationRequestRepository.GetApprovedDaysForYearAsync(
				balance.EmployeeId,
				balance.Year,
				_currentUser.CompanyId.Value,
				cancellationToken: cancellationToken);

		var totalDays =
			balance.EntitledDays +
			balance.CarriedOverDays +
			balance.AdjustmentDays;

		var remainingDays =
			totalDays - approvedDays;

        return new VacationBalanceDto
        {
            Id = balance.Id,
            EmployeeId = balance.EmployeeId,
            Year = balance.Year,
            EntitledDays = balance.EntitledDays,
            CarriedOverDays = balance.CarriedOverDays,
            AdjustmentDays = balance.AdjustmentDays,
            Notes = balance.Notes,
			TotalDays = totalDays,
			ApprovedDays = approvedDays,
			RemainingDays = remainingDays
        };
    }
}