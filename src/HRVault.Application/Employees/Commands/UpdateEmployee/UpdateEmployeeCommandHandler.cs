using AutoMapper;
using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler
    : IRequestHandler<UpdateEmployeeCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IVacationBalanceRepository _vacationBalanceRepository;
    private readonly IVacationEntitlementCalculator _vacationEntitlementCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        IVacationBalanceRepository vacationBalanceRepository,
        IVacationEntitlementCalculator vacationEntitlementCalculator,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _vacationBalanceRepository = vacationBalanceRepository;
        _vacationEntitlementCalculator = vacationEntitlementCalculator;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        var oldHireDate = employee.HireDate;

        var employeeNumberExists =
            await _employeeRepository.EmployeeNumberExistsAsync(
                request.EmployeeNumber,
                companyId,
                request.Id,
                cancellationToken);

        if (employeeNumberExists)
        {
            throw new ConflictException(
                "An employee with this employee number already exists.");
        }

        if (request.DepartmentId.HasValue)
        {
            var department =
                await _departmentRepository.GetByIdAndCompanyAsync(
                    request.DepartmentId.Value,
                    companyId,
                    cancellationToken);

            if (department is null)
            {
                throw new NotFoundException(
                    "Department not found.");
            }
        }

        if (request.PositionId.HasValue)
        {
            var position =
                await _positionRepository.GetByIdAndCompanyAsync(
                    request.PositionId.Value,
                    companyId,
                    cancellationToken);

            if (position is null)
            {
                throw new NotFoundException(
                    "Position not found.");
            }
        }

        _mapper.Map(request, employee);

        employee.Status =
            (EmployeeStatus)request.Status;

        await _employeeRepository.UpdateAsync(
            employee,
            cancellationToken);

        if (oldHireDate != employee.HireDate)
        {
            var vacationYear =
                employee.HireDate.Year;

            var entitledDays =
                _vacationEntitlementCalculator.Calculate(
                    employee.HireDate,
                    vacationYear);

            var balance =
                await _vacationBalanceRepository
                    .GetByEmployeeAndYearAsync(
                        employee.Id,
                        vacationYear,
                        companyId,
                        cancellationToken);

            if (balance is null)
            {
                balance = new VacationBalance
                {
                    CompanyId = companyId,
                    EmployeeId = employee.Id,
                    Year = vacationYear,
                    EntitledDays = entitledDays,
                    CarriedOverDays = 0,
                    AdjustmentDays = 0,
                    Notes =
                        "Saldo criado automaticamente após alteração da data de entrada."
                };

                await _vacationBalanceRepository.AddAsync(
                    balance,
                    cancellationToken);
            }
            else
            {
                balance.EntitledDays =
                    entitledDays;

                await _vacationBalanceRepository.UpdateAsync(
                    balance,
                    cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return employee.Id;
    }
}