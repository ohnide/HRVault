using HRVault.Application.Common.Interfaces;
using HRVault.Application.Documents.DTOs;
using MediatR;

namespace HRVault.Application.Documents.Queries.GetDocumentAlerts;

public class GetDocumentAlertsQueryHandler
    : IRequestHandler<GetDocumentAlertsQuery, List<DocumentAlertDto>>
{
    private readonly IDocumentAlertRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetDocumentAlertsQueryHandler(
        IDocumentAlertRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<DocumentAlertDto>> Handle(
        GetDocumentAlertsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var alerts =
            await _repository.GetPendingByCompanyAsync(
                _currentUser.CompanyId.Value,
                cancellationToken);

        return alerts
            .Select(x =>
            {
                var expirationDate =
                    x.Document.ExpirationDate;

                int? daysRemaining = null;

                if (expirationDate.HasValue)
                {
                    daysRemaining =
                        expirationDate.Value.DayNumber -
                        today.DayNumber;
                }

                return new DocumentAlertDto
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    EmployeeId = x.EmployeeId,
                    EmployeeName =
                        $"{x.Employee.FirstName} {x.Employee.LastName}",
                    DocumentTypeName =
                        x.Document.EmployeeDocumentType.Name,
                    FileName = x.Document.FileName,
                    ExpirationDate = expirationDate,
                    DaysRemaining = daysRemaining,
                    AlertDate = x.AlertDate,
                    Status = x.Status.ToString(),
                    EmailSent = x.EmailSent,
                    EmailSentAt = x.EmailSentAt
                };
            })
            .ToList();
    }
}