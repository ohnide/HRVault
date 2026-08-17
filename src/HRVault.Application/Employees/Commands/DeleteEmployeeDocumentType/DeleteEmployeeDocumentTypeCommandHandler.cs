using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeDocumentType;

public class DeleteEmployeeDocumentTypeCommandHandler
    : IRequestHandler<DeleteEmployeeDocumentTypeCommand>
{
    private readonly IEmployeeDocumentTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeDocumentTypeCommandHandler(
        IEmployeeDocumentTypeRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteEmployeeDocumentTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var documentType =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (documentType is null)
        {
            throw new NotFoundException(
                "Document type not found.");
        }

        await _repository.DeleteAsync(
            documentType,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}