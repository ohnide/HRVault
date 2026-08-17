using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IAbsenceTypeRepository
{
    Task<List<AbsenceType>> GetAllByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<AbsenceType?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        string name,
        Guid companyId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AbsenceType absenceType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        AbsenceType absenceType,
        CancellationToken cancellationToken = default);
}