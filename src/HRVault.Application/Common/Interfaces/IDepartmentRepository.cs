using HRVault.Application.Common.Models;
using HRVault.Application.Departments.DTOs;
using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<PagedResult<DepartmentDto>> SearchAsync(
        DepartmentFilterDto filter,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<List<Department>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<Department?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> WouldCreateCycleAsync(
        Guid departmentId,
        Guid parentDepartmentId,
        Guid companyId,
        CancellationToken cancellationToken = default);
}