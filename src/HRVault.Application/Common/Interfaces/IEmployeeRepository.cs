using HRVault.Application.Common.Models;
using HRVault.Application.Employees.DTOs;
using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeRepository
    : IRepository<Employee>
{
    Task<List<Employee>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<Employee?> GetDetailsByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid companyId,
        Guid? excludeEmployeeId = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<EmployeeListDto>> SearchAsync(
        EmployeeFilterDto filter,
        Guid companyId,
        CancellationToken cancellationToken = default);
}