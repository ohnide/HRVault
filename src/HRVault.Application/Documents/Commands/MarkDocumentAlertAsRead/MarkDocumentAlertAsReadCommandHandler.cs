using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Documents.Commands.MarkDocumentAlertAsRead;

public class MarkDocumentAlertAsReadCommandHandler
    : IRequestHandler<MarkDocumentAlertAsReadCommand>
{
    private readonly IDocumentAlertRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public MarkDocumentAlertAsReadCommandHandler(
        IDocumentAlertRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        MarkDocumentAlertAsReadCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var alert =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (alert is null)
        {
            throw new NotFoundException(
                "Document alert not found.");
        }

        alert.Status = DocumentAlertStatus.Read;
        alert.ReadAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}