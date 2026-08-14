using HRVault.Application.Companies.DTOs;
using MediatR;

namespace HRVault.Application.Companies.Queries.GetCompanyById;

public record GetCompanyByIdQuery(Guid Id)
    : IRequest<CompanyDto?>;