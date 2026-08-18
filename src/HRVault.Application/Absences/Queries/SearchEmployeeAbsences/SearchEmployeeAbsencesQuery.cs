using HRVault.Application.Absences.DTOs;
using HRVault.Application.Common.Models;
using MediatR;

namespace HRVault.Application.Absences.Queries.SearchEmployeeAbsences;

public record SearchEmployeeAbsencesQuery(
    EmployeeAbsenceFilterDto Filter
) : IRequest<PagedResult<EmployeeAbsenceDto>>;