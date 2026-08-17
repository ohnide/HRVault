using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.SearchEmployeeDocuments;

public class SearchEmployeeDocumentsQueryHandler
    : IRequestHandler<
        SearchEmployeeDocumentsQuery,
        PagedResult<EmployeeDocumentDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUser;

    public SearchEmployeeDocumentsQueryHandler(
        IEmployeeRepository employeeRepository,
        IDocumentRepository documentRepository,
        ICurrentUserService currentUser)
    {
        _employeeRepository = employeeRepository;
        _documentRepository = documentRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<EmployeeDocumentDto>> Handle(
        SearchEmployeeDocumentsQuery request,
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

        return await _documentRepository.SearchByEmployeeAsync(
            request.EmployeeId,
            request.Filter,
            cancellationToken);
    }
}