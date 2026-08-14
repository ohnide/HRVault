using MediatR;

namespace HRVault.Application.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string VatNumber,
    string? Address,
    string? LogoUrl
) : IRequest;