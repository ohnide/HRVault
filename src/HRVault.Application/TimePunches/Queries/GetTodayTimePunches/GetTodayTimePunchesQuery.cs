using HRVault.Application.TimePunches.DTOs;
using MediatR;

namespace HRVault.Application.TimePunches.Queries.GetTodayTimePunches;

public record GetTodayTimePunchesQuery(
    Guid? EmployeeId
) : IRequest<List<TimePunchDto>>;
