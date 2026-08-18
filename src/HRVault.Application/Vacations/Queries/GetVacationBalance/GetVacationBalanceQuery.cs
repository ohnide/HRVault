using HRVault.Application.Vacations.DTOs;
using MediatR;

namespace HRVault.Application.Vacations.Queries.GetVacationBalance;

public record GetVacationBalanceQuery(
    Guid EmployeeId,
    int Year
) : IRequest<VacationBalanceDto?>;