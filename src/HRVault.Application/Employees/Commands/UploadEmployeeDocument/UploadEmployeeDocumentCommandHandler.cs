using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.Commands.UploadEmployeeDocument;

public class UploadEmployeeDocumentCommandHandler
    : IRequestHandler<UploadEmployeeDocumentCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IEmployeeDocumentTypeRepository _documentTypeRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UploadEmployeeDocumentCommandHandler(
        IEmployeeRepository employeeRepository,
        IDocumentRepository documentRepository,
        IEmployeeDocumentTypeRepository documentTypeRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _documentRepository = documentRepository;
        _documentTypeRepository = documentTypeRepository;
        _fileStorageService = fileStorageService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        UploadEmployeeDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null ||
            _currentUser.UserId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var companyId =
            _currentUser.CompanyId.Value;

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        var documentType =
            await _documentTypeRepository.GetByIdAndCompanyAsync(
                request.EmployeeDocumentTypeId,
                companyId,
                cancellationToken);

        if (documentType is null)
        {
            throw new NotFoundException(
                "Document type not found.");
        }

        if (request.Size <= 0)
        {
            throw new BusinessRuleException(
                "The file is empty.");
        }

        if (!documentType.HasExpiration &&
            request.ExpirationDate.HasValue)
        {
            throw new BusinessRuleException(
                "This document type does not support an expiration date.");
        }

        if (documentType.HasExpiration &&
            !request.ExpirationDate.HasValue)
        {
            throw new BusinessRuleException(
                "Expiration date is required for this document type.");
        }

        if (request.IssueDate.HasValue &&
            request.ExpirationDate.HasValue &&
            request.ExpirationDate.Value <
            request.IssueDate.Value)
        {
            throw new BusinessRuleException(
                "Expiration date cannot be earlier than issue date.");
        }

        var storageName =
            await _fileStorageService.UploadAsync(
                request.Content,
                request.FileName,
                request.ContentType,
                cancellationToken);

        try
        {
            var document = new Document
            {
                EmployeeId = request.EmployeeId,

                EmployeeDocumentTypeId =
                    request.EmployeeDocumentTypeId,

                IssueDate = request.IssueDate,

                ExpirationDate =
                    request.ExpirationDate,

                Notes = request.Notes,

                FileName = request.FileName,

                StorageName = storageName,

                MimeType = request.ContentType,

                Size = request.Size,

                UploadedByUserId =
                    _currentUser.UserId.Value,

                UploadedAt = DateTime.UtcNow
            };

            await _documentRepository.AddAsync(
                document,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return document.Id;
        }
        catch
        {
            await _fileStorageService.DeleteAsync(
                storageName,
                cancellationToken);

            throw;
        }
    }
}