using HRVault.Application.Common.Interfaces;
using HRVault.Application.Documents.DTOs;
using HRVault.Domain.Entities;

namespace HRVault.Application.Documents.Services;

public class DocumentAlertGenerator
    : IDocumentAlertGenerator
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentAlertRepository _alertRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentAlertGenerator(
        IDocumentRepository documentRepository,
        IDocumentAlertRepository alertRepository,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _alertRepository = alertRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> GenerateForCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var filter = new ExpiringDocumentFilterDto
        {
            Status = "Expiring",
            Page = 1,
            PageSize = 1000
        };

        var result =
            await _documentRepository.SearchByCompanyAsync(
                companyId,
                filter,
                cancellationToken);

        var createdCount = 0;

        foreach (var document in result.Items)
        {
            var exists =
				await _alertRepository.ExistsAsync(
					document.DocumentId,
					cancellationToken);

            if (exists)
                continue;

            var alert = new DocumentAlert
            {
                CompanyId = companyId,
                DocumentId = document.DocumentId,
                EmployeeId = document.EmployeeId,
                AlertDate = today,
                EmailSent = false
            };

            await _alertRepository.AddAsync(
                alert,
                cancellationToken);

            createdCount++;
        }

        if (createdCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        return createdCount;
    }
}