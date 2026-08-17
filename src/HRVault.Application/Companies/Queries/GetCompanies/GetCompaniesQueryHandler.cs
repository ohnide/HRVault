using HRVault.Application.Common.Interfaces;
using HRVault.Application.Companies.DTOs;
using HRVault.Application.Common.Exceptions;
using MediatR;

namespace HRVault.Application.Companies.Queries.GetCompanies;

public class GetCompaniesQueryHandler
    : IRequestHandler<GetCompaniesQuery, List<CompanyDto>>
{
    private readonly ICompanyRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetCompaniesQueryHandler(
        ICompanyRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<CompanyDto>> Handle(
        GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        if (!_currentUser.IsPlatformAdministrator)
        {
            throw new ForbiddenException(
                "Platform administrator access is required.");
        }

        var companies = await _repository.GetAllAsync(
            cancellationToken);

        return companies
            .Select(x => new CompanyDto
            {
                Id = x.Id,
                Name = x.Name,
                VatNumber = x.VatNumber,
                Address = x.Address,
                LogoUrl = x.LogoUrl,
				HrNotificationEmail =
					x.HrNotificationEmail
            })
            .ToList();
    }
}