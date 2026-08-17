namespace HRVault.Application.Documents.Services;

public interface IDocumentAlertEmailService
{
    Task<int> SendForCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);
}