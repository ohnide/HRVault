using HRVault.Application.WorkSchedules.DTOs;
using MediatR;
namespace HRVault.Application.WorkSchedules.Queries.GetWorkScheduleById;
public record GetWorkScheduleByIdQuery(Guid Id) : IRequest<WorkScheduleDto?>;
