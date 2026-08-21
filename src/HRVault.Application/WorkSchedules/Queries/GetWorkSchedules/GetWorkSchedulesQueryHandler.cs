using HRVault.Application.Common.Interfaces;
using HRVault.Application.WorkSchedules.DTOs;
using MediatR;
namespace HRVault.Application.WorkSchedules.Queries.GetWorkSchedules;
public class GetWorkSchedulesQueryHandler : IRequestHandler<GetWorkSchedulesQuery, List<WorkScheduleDto>>
{
    private readonly IWorkScheduleRepository _repository; private readonly ICurrentUserService _currentUser;
    public GetWorkSchedulesQueryHandler(IWorkScheduleRepository repository, ICurrentUserService currentUser)
    { _repository = repository; _currentUser = currentUser; }
    public async Task<List<WorkScheduleDto>> Handle(GetWorkSchedulesQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null) throw new UnauthorizedAccessException();
        var schedules = await _repository.GetAllByCompanyAsync(_currentUser.CompanyId.Value, cancellationToken);
        return schedules.Select(WorkScheduleMapping.ToDto).ToList();
    }
}
