using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Vacations.Commands.CreateVacationRequest;

public class CreateVacationRequestCommandHandler
    : IRequestHandler<CreateVacationRequestCommand, Guid>
{
    private readonly IVacationRequestRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVacationRequestCommandHandler(
        IVacationRequestRepository repository,
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
        CreateVacationRequestCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var companyId = _currentUser.CompanyId.Value;

        if (request.EndDate.Date < request.StartDate.Date)
        {
            throw new BusinessRuleException(
                "End date must be equal to or greater than start date.");
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

        var days = CalculateWeekDays(
            request.StartDate.Date,
            request.EndDate.Date);

        if (days <= 0)
        {
            throw new BusinessRuleException(
                "The selected period does not contain working days.");
        }

		var hasOverlap =
			await _repository.HasOverlapAsync(
				request.EmployeeId,
				request.StartDate.Date,
				request.EndDate.Date,
				companyId,
				cancellationToken);

		if (hasOverlap)
		{
			throw new BusinessRuleException(
				"The employee already has a vacation request for the selected period.");
		}

        var vacationRequest =
            new VacationRequest
            {
                CompanyId = companyId,
                EmployeeId = request.EmployeeId,
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                Days = days,
                Status = VacationRequestStatus.Pending,
                Notes =
                    string.IsNullOrWhiteSpace(request.Notes)
                        ? null
                        : request.Notes.Trim()
            };

        await _repository.AddAsync(
            vacationRequest,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return vacationRequest.Id;
    }

    private static decimal CalculateWeekDays(
        DateTime startDate,
        DateTime endDate)
    {
        decimal days = 0;

        for (
            var date = startDate;
            date <= endDate;
            date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday &&
                date.DayOfWeek != DayOfWeek.Sunday)
            {
                days++;
            }
        }

        return days;
    }
}