using HRVault.Domain.Entities;
using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;

namespace HRVault.Application.Common.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAndEmployeeIdAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<List<Document>> GetAllByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Document document,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Document document,
        CancellationToken cancellationToken = default);
		
	Task<PagedResult<EmployeeDocumentDto>> SearchByEmployeeAsync(
		Guid employeeId,
		EmployeeDocumentFilterDto filter,
		CancellationToken cancellationToken = default);
}