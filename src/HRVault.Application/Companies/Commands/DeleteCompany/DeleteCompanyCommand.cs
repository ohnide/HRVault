using MediatR;

namespace HRVault.Application.Companies.Commands.DeleteCompany;

public record DeleteCompanyCommand(Guid Id) : IRequest;