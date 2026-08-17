using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IEmployeeDocumentTypeRepository
{
    Task<List<EmployeeDocumentType>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<EmployeeDocumentType?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        string name,
        Guid companyId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        EmployeeDocumentType documentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        EmployeeDocumentType documentType,
        CancellationToken cancellationToken = default);
}