namespace HRVault.Application.Documents.Services;

public interface IDocumentAlertGenerator
{
    Task<int> GenerateForCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);
}