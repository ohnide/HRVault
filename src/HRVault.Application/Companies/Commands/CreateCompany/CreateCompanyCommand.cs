using MediatR;

namespace HRVault.Application.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(
    string Name,
    string VatNumber,
    string? Address,
    string? LogoUrl,
    string AdministratorName,
    string AdministratorEmail,
    string AdministratorPassword
) : IRequest<Guid>;