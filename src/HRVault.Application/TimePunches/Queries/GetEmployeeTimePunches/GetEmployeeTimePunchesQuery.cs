using HRVault.Application.TimePunches.DTOs;
using MediatR;

namespace HRVault.Application.TimePunches.Queries.GetEmployeeTimePunches;

public record GetEmployeeTimePunchesQuery(
    Guid EmployeeId,
    DateTime FromUtc,
    DateTime ToUtc
) : IRequest<List<TimePunchDto>>;
