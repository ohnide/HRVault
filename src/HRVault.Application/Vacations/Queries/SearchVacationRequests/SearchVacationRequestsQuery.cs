using HRVault.Application.Common.Models;
using HRVault.Application.Vacations.DTOs;
using MediatR;

namespace HRVault.Application.Vacations.Queries.SearchVacationRequests;

public record SearchVacationRequestsQuery(
    VacationRequestFilterDto Filter
) : IRequest<PagedResult<VacationRequestDto>>;