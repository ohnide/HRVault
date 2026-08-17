using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.DownloadEmployeeDocument;

public class DownloadEmployeeDocumentQueryHandler
    : IRequestHandler<
        DownloadEmployeeDocumentQuery,
        EmployeeDocumentDownloadDto>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUser;

    public DownloadEmployeeDocumentQueryHandler(
        IEmployeeRepository employeeRepository,
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUser)
    {
        _employeeRepository = employeeRepository;
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _currentUser = currentUser;
    }

    public async Task<EmployeeDocumentDownloadDto> Handle(
        DownloadEmployeeDocumentQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        // Garante que o funcionário pertence
        // à empresa do utilizador atual.
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

        // Também garante que o documento pertence
        // ao funcionário indicado.
        var document =
            await _documentRepository.GetByIdAndEmployeeIdAsync(
                request.DocumentId,
                request.EmployeeId,
                cancellationToken);

        if (document is null)
        {
            throw new NotFoundException(
                "Document not found.");
        }

        var stream =
            await _fileStorageService.DownloadAsync(
                document.StorageName,
                cancellationToken);

        return new EmployeeDocumentDownloadDto
        {
            Content = stream,
            FileName = document.FileName,
            MimeType = document.MimeType
        };
    }
}