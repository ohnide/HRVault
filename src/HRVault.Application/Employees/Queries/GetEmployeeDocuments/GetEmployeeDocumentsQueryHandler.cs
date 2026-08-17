using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeDocuments;

public class GetEmployeeDocumentsQueryHandler
    : IRequestHandler<
        GetEmployeeDocumentsQuery,
        List<EmployeeDocumentDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeDocumentsQueryHandler(
        IEmployeeRepository employeeRepository,
        IDocumentRepository documentRepository,
        ICurrentUserService currentUser)
    {
        _employeeRepository = employeeRepository;
        _documentRepository = documentRepository;
        _currentUser = currentUser;
    }

    public async Task<List<EmployeeDocumentDto>> Handle(
        GetEmployeeDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

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

        var documents =
            await _documentRepository.GetAllByEmployeeIdAsync(
                request.EmployeeId,
                cancellationToken);

        return documents
            .Select(x => new EmployeeDocumentDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                Category = x.Category,
                FileName = x.FileName,
                MimeType = x.MimeType,
                Size = x.Size,
                UploadedByUserId = x.UploadedByUserId,
                UploadedAt = x.UploadedAt
            })
            .ToList();
    }
}