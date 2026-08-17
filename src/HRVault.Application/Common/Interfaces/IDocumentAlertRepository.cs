using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Interfaces;

public interface IDocumentAlertRepository
{
    Task<bool> ExistsAsync(
		Guid documentId,
		CancellationToken cancellationToken = default);

    Task AddAsync(
        DocumentAlert alert,
        CancellationToken cancellationToken = default);

    Task<List<DocumentAlert>> GetPendingByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<DocumentAlert?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default);
		
	Task<List<DocumentAlert>> GetUnsentByCompanyAsync(
		Guid companyId,
		CancellationToken cancellationToken = default);
}