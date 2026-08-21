using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;
namespace HRVault.Application.WorkSchedules.Commands.SetWorkScheduleActive;
public class SetWorkScheduleActiveCommandHandler : IRequestHandler<SetWorkScheduleActiveCommand>
{
    private readonly IWorkScheduleRepository _repository; private readonly ICurrentUserService _currentUser; private readonly IUnitOfWork _unitOfWork;
    public SetWorkScheduleActiveCommandHandler(IWorkScheduleRepository repository, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    { _repository = repository; _currentUser = currentUser; _unitOfWork = unitOfWork; }
    public async Task Handle(SetWorkScheduleActiveCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null) throw new UnauthorizedAccessException();
        var schedule = await _repository.GetByIdAndCompanyAsync(request.Id, _currentUser.CompanyId.Value, cancellationToken);
        if (schedule is null) throw new NotFoundException("Horário não encontrado.");
        schedule.IsActive = request.IsActive;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
