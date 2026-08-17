using HRVault.Application.Common.Interfaces;
using HRVault.Application.Documents.Services;
using MediatR;

namespace HRVault.Application.Documents.Commands.GenerateDocumentAlerts;

public class GenerateDocumentAlertsCommandHandler
    : IRequestHandler<GenerateDocumentAlertsCommand, int>
{
    private readonly IDocumentAlertGenerator _generator;
    private readonly ICurrentUserService _currentUser;

    public GenerateDocumentAlertsCommandHandler(
        IDocumentAlertGenerator generator,
        ICurrentUserService currentUser)
    {
        _generator = generator;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(
        GenerateDocumentAlertsCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        return await _generator.GenerateForCompanyAsync(
            _currentUser.CompanyId.Value,
            cancellationToken);
    }
}