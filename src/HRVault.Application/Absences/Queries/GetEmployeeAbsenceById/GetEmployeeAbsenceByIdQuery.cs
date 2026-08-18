using HRVault.Application.Absences.DTOs;
using MediatR;

namespace HRVault.Application.Absences.Queries.GetEmployeeAbsenceById;

public record GetEmployeeAbsenceByIdQuery(
    Guid Id
) : IRequest<EmployeeAbsenceDto?>;