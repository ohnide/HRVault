using HRVault.Application.Absences.DTOs;
using MediatR;

namespace HRVault.Application.Absences.Queries.GetAbsenceTypes;

public record GetAbsenceTypesQuery
    : IRequest<List<AbsenceTypeDto>>;