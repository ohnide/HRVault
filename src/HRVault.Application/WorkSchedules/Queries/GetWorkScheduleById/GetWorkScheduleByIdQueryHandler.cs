using HRVault.Application.Common.Interfaces;
using HRVault.Application.WorkSchedules.DTOs;
using MediatR;
namespace HRVault.Application.WorkSchedules.Queries.GetWorkScheduleById;
public class GetWorkScheduleByIdQueryHandler : IRequestHandler<GetWorkScheduleByIdQuery, WorkScheduleDto?>
{
    private readonly IWorkScheduleRepository _repository; private readonly ICurrentUserService _currentUser;
    public GetWorkScheduleByIdQueryHandler(IWorkScheduleRepository repository, ICurrentUserService currentUser)
    { _repository = repository; _currentUser = currentUser; }
    public async Task<WorkScheduleDto?> Handle(GetWorkScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null) throw new UnauthorizedAccessException();
        var schedule = await _repository.GetByIdAndCompanyAsync(request.Id, _currentUser.CompanyId.Value, cancellationToken);
        return schedule is null ? null : WorkScheduleMapping.ToDto(schedule);
    }
}
