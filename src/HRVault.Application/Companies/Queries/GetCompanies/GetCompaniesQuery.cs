using HRVault.Application.Companies.DTOs;
using MediatR;

namespace HRVault.Application.Companies.Queries.GetCompanies;

public record GetCompaniesQuery()
    : IRequest<List<CompanyDto>>;