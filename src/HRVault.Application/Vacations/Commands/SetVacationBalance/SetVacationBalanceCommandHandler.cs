using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Vacations.Commands.SetVacationBalance;

public class SetVacationBalanceCommandHandler
    : IRequestHandler<SetVacationBalanceCommand, Guid>
{
    private readonly IVacationBalanceRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public SetVacationBalanceCommandHandler(
        IVacationBalanceRepository repository,
        IEmployeeRepository employeeRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        SetVacationBalanceCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId =
            _currentUser.CompanyId.Value;

        if (request.Year < 2000 ||
            request.Year > 2100)
        {
            throw new BusinessRuleException(
                "Invalid vacation year.");
        }

        if (request.EntitledDays < 0 ||
            request.CarriedOverDays < 0)
        {
            throw new BusinessRuleException(
                "Vacation days cannot be negative.");
        }

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        var balance =
            await _repository.GetByEmployeeAndYearAsync(
                request.EmployeeId,
                request.Year,
                companyId,
                cancellationToken);

        if (balance is null)
        {
            balance = new VacationBalance
            {
                CompanyId = companyId,
                EmployeeId = request.EmployeeId,
                Year = request.Year,
                EntitledDays = request.EntitledDays,
                CarriedOverDays = request.CarriedOverDays,
                AdjustmentDays = request.AdjustmentDays,
                Notes =
                    string.IsNullOrWhiteSpace(request.Notes)
                        ? null
                        : request.Notes.Trim()
            };

            await _repository.AddAsync(
                balance,
                cancellationToken);
        }
        else
        {
            balance.EntitledDays =
                request.EntitledDays;

            balance.CarriedOverDays =
                request.CarriedOverDays;

            balance.AdjustmentDays =
                request.AdjustmentDays;

            balance.Notes =
                string.IsNullOrWhiteSpace(request.Notes)
                    ? null
                    : request.Notes.Trim();
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return balance.Id;
    }
}