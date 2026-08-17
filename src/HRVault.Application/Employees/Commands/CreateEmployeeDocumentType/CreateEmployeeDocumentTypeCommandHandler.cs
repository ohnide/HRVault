using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployeeDocumentType;

public class CreateEmployeeDocumentTypeCommandHandler
    : IRequestHandler<CreateEmployeeDocumentTypeCommand, Guid>
{
    private readonly IEmployeeDocumentTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeDocumentTypeCommandHandler(
        IEmployeeDocumentTypeRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateEmployeeDocumentTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId =
            _currentUser.CompanyId.Value;

        var exists =
            await _repository.NameExistsAsync(
                request.Name,
                companyId,
                cancellationToken: cancellationToken);

        if (exists)
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

        var documentType =
            new EmployeeDocumentType
            {
                CompanyId = companyId,
                Name = request.Name,
                Description = request.Description,
                HasExpiration = request.HasExpiration,
                ExpirationWarningDays =
                    request.HasExpiration
                        ? request.ExpirationWarningDays
                        : null
            };

        await _repository.AddAsync(
            documentType,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return documentType.Id;
    }
}