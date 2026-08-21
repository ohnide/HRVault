using HRVault.Application.WorkSchedules.DTOs;
using MediatR;
namespace HRVault.Application.WorkSchedules.Queries.GetWorkSchedules;
public record GetWorkSchedulesQuery : IRequest<List<WorkScheduleDto>>;
