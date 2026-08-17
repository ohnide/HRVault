using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpdateEmployeeDocumentType;

public class UpdateEmployeeDocumentTypeCommandHandler
    : IRequestHandler<UpdateEmployeeDocumentTypeCommand>
{
    private readonly IEmployeeDocumentTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeDocumentTypeCommandHandler(
        IEmployeeDocumentTypeRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateEmployeeDocumentTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var documentType =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (documentType is null)
        {
            throw new NotFoundException(
                "Document type not found.");
        }

        var nameExists =
            await _repository.NameExistsAsync(
                request.Name,
                companyId,
                request.Id,
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException(
                "A document type with this name already exists.");
        }

        if (!request.HasExpiration &&
            request.ExpirationWarningDays.HasValue)
        {
            throw new BusinessRuleException(
                "Expiration warning days can only be defined for document types with expiration.");
        }

        if (request.ExpirationWarningDays < 0)
        {
            throw new BusinessRuleException(
                "Expiration warning days cannot be negative.");
        }

        documentType.Name = request.Name;
        documentType.Description = request.Description;
        documentType.HasExpiration = request.HasExpiration;
        documentType.ExpirationWarningDays =
            request.HasExpiration
                ? request.ExpirationWarningDays
                : null;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}