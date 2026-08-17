using HRVault.Application.Common.Interfaces;
using HRVault.Application.Companies.DTOs;
using MediatR;

namespace HRVault.Application.Companies.Queries.GetCompanyById;

public class GetCompanyByIdQueryHandler
    : IRequestHandler<GetCompanyByIdQuery, CompanyDto?>
{
    private readonly ICompanyRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetCompanyByIdQueryHandler(
        ICompanyRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<CompanyDto?> Handle(
        GetCompanyByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        // Platform admin pode consultar qualquer empresa.
        // Os restantes utilizadores só podem consultar a própria empresa.
        if (!_currentUser.IsPlatformAdministrator &&
            request.Id != _currentUser.CompanyId.Value)
        {
            return null;
        }

        var company = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (company is null)
            return null;

        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            VatNumber = company.VatNumber,
            Address = company.Address,
            LogoUrl = company.LogoUrl,
			HrNotificationEmail =
				company.HrNotificationEmail
        };
    }
}