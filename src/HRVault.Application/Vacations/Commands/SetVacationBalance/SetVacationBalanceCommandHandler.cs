using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
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
        {
            throw new UnauthorizedAccessException();
        }

        var companyId = _currentUser.CompanyId.Value;

        if (request.Year < 2000 ||
            request.Year > 2100)
        {
            throw new BusinessRuleException(
                "Invalid vacation year.");
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
            throw new BusinessRuleException(
                "Vacation balance not found for this employee and year.");
        }

        balance.AdjustmentDays =
            request.AdjustmentDays;

        balance.Notes =
            string.IsNullOrWhiteSpace(request.Notes)
                ? null
                : request.Notes.Trim();

        await _repository.UpdateAsync(
            balance,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return balance.Id;
    }
}