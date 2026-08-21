using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.WorkSchedules.Commands.DeleteWorkSchedule;

public class DeleteWorkScheduleCommandHandler
    : IRequestHandler<DeleteWorkScheduleCommand>
{
    private readonly IWorkScheduleRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWorkScheduleCommandHandler(
        IWorkScheduleRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var schedule =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (schedule is null)
        {
            throw new NotFoundException(
                "Horário não encontrado.");
        }

        var isAssigned =
            await _repository.IsAssignedAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (isAssigned)
        {
            throw new ConflictException(
                "Não é possível apagar este horário porque está atribuído a pelo menos um funcionário.");
        }

        await _repository.DeleteAsync(
            schedule,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
