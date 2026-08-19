using HRVault.Application.Common.Interfaces;
using HRVault.Application.Vacations.DTOs;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Vacations.Queries.GetVacationBalance;

public class GetVacationBalanceQueryHandler
    : IRequestHandler<GetVacationBalanceQuery, VacationBalanceDto?>
{
    private readonly IVacationBalanceRepository _repository;
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IVacationEntitlementCalculator _vacationEntitlementCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetVacationBalanceQueryHandler(
        IVacationBalanceRepository repository,
        IVacationRequestRepository vacationRequestRepository,
        IEmployeeRepository employeeRepository,
        IVacationEntitlementCalculator vacationEntitlementCalculator,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _vacationRequestRepository = vacationRequestRepository;
        _employeeRepository = employeeRepository;
        _vacationEntitlementCalculator = vacationEntitlementCalculator;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<VacationBalanceDto?> Handle(
        GetVacationBalanceQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var balance =
            await _repository.GetByEmployeeAndYearAsync(
                request.EmployeeId,
                request.Year,
                companyId,
                cancellationToken);

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (employee is null)
            return null;

        // Não criar saldos para anos anteriores à admissão.
        if (request.Year < employee.HireDate.Year)
            return null;

        balance = await EnsureBalanceAsync(
            employee,
            request.Year,
            companyId,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return await BuildDtoAsync(
            balance,
            companyId,
            cancellationToken);
    }

    private async Task<VacationBalance> EnsureBalanceAsync(
        Employee employee,
        int year,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        decimal carriedOverDays = 0;

        if (year > employee.HireDate.Year)
        {
            var previousBalance =
                await EnsureBalanceAsync(
                    employee,
                    year - 1,
                    companyId,
                    cancellationToken);

            var previousApprovedDays =
                await _vacationRequestRepository
                    .GetApprovedDaysForYearAsync(
                        employee.Id,
                        year - 1,
                        companyId,
                        cancellationToken:
                            cancellationToken);

            var previousTotalDays =
                previousBalance.EntitledDays +
                previousBalance.CarriedOverDays +
                previousBalance.AdjustmentDays;

            var previousRemainingDays =
                previousTotalDays -
                previousApprovedDays;

            carriedOverDays =
                Math.Max(0, previousRemainingDays);
        }

        var entitledDays =
            _vacationEntitlementCalculator.Calculate(
                employee.HireDate,
                year);

        var existing =
            await _repository.GetByEmployeeAndYearAsync(
                employee.Id,
                year,
                companyId,
                cancellationToken);

        if (existing is not null)
        {
            var changed = false;

            if (existing.EntitledDays != entitledDays)
            {
                existing.EntitledDays = entitledDays;
                changed = true;
            }

            if (existing.CarriedOverDays != carriedOverDays)
            {
                existing.CarriedOverDays = carriedOverDays;
                changed = true;
            }

            if (changed)
            {
                await _repository.UpdateAsync(
                    existing,
                    cancellationToken);
            }

            return existing;
        }

        var balance = new VacationBalance
        {
            CompanyId = companyId,
            EmployeeId = employee.Id,
            Year = year,
            EntitledDays = entitledDays,
            CarriedOverDays = carriedOverDays,
            AdjustmentDays = 0,
            Notes =
                year == employee.HireDate.Year
                    ? "Saldo criado automaticamente na admissão."
                    : "Saldo anual criado automaticamente."
        };

        await _repository.AddAsync(
            balance,
            cancellationToken);

        return balance;
    }

    private async Task<VacationBalanceDto> BuildDtoAsync(
        VacationBalance balance,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        var approvedDays =
            await _vacationRequestRepository
                .GetApprovedDaysForYearAsync(
                    balance.EmployeeId,
                    balance.Year,
                    companyId,
                    cancellationToken:
                        cancellationToken);

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
