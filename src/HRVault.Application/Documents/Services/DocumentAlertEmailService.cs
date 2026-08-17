using System.Net;
using System.Text;
using HRVault.Application.Common.Interfaces;

namespace HRVault.Application.Documents.Services;

public class DocumentAlertEmailService
    : IDocumentAlertEmailService
{
    private readonly IDocumentAlertRepository _alertRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentAlertEmailService(
        IDocumentAlertRepository alertRepository,
        ICompanyRepository companyRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _alertRepository = alertRepository;
        _companyRepository = companyRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> SendForCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var company =
            await _companyRepository.GetByIdAsync(
                companyId,
                cancellationToken);

        if (company is null ||
            string.IsNullOrWhiteSpace(
                company.HrNotificationEmail))
        {
            return 0;
        }

        var alerts =
            await _alertRepository.GetUnsentByCompanyAsync(
                companyId,
                cancellationToken);

        if (alerts.Count == 0)
            return 0;

        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        var body = new StringBuilder();

        body.Append("""
            <h2>HRVault - Alertas de documentos</h2>
            <p>Existem documentos de funcionários que requerem atenção.</p>
            <table style="border-collapse:collapse;width:100%">
            <thead>
            <tr>
                <th style="text-align:left;padding:8px">Funcionário</th>
                <th style="text-align:left;padding:8px">Documento</th>
                <th style="text-align:left;padding:8px">Validade</th>
                <th style="text-align:left;padding:8px">Estado</th>
            </tr>
            </thead>
            <tbody>
            """);

        foreach (var alert in alerts)
        {
            var expirationDate =
                alert.Document.ExpirationDate;

            var status = "Sem validade";

            if (expirationDate.HasValue)
            {
                var days =
                    expirationDate.Value.DayNumber -
                    today.DayNumber;

                status = days switch
                {
                    < 0 => $"Expirado há {Math.Abs(days)} dias",
                    0 => "Expira hoje",
                    1 => "Expira amanhã",
                    _ => $"Expira em {days} dias"
                };
            }

            var employeeName =
                $"{alert.Employee.FirstName} {alert.Employee.LastName}";

            body.Append($"""
                <tr>
                    <td style="padding:8px">
                        {WebUtility.HtmlEncode(employeeName)}
                    </td>
                    <td style="padding:8px">
                        {WebUtility.HtmlEncode(
                            alert.Document.EmployeeDocumentType.Name)}
                    </td>
                    <td style="padding:8px">
                        {expirationDate?.ToString("dd/MM/yyyy") ?? "-"}
                    </td>
                    <td style="padding:8px">
                        {WebUtility.HtmlEncode(status)}
                    </td>
                </tr>
                """);
        }

        body.Append("""
            </tbody>
            </table>
            <p>Consulte o HRVault para mais informações.</p>
            """);

        var subject =
            $"HRVault - {alerts.Count} documento(s) requerem atenção";

        await _emailService.SendAsync(
            company.HrNotificationEmail,
            subject,
            body.ToString(),
            cancellationToken);

        var sentAt = DateTime.UtcNow;

        foreach (var alert in alerts)
        {
            alert.EmailSent = true;
            alert.EmailSentAt = sentAt;
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return alerts.Count;
    }
}