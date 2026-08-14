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
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UploadEmployeeDocumentCommandHandler(
        IEmployeeRepository employeeRepository,
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _documentRepository = documentRepository;
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

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        if (request.Size <= 0)
        {
            throw new BusinessRuleException(
                "The file is empty.");
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
                Category = request.Category,
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
            // Se a gravação na BD falhar,
            // não deixamos um ficheiro órfão no MinIO.
            await _fileStorageService.DeleteAsync(
                storageName,
                cancellationToken);

            throw;
        }
    }
}