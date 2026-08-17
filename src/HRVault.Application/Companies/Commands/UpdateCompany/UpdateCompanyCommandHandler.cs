using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Companies.Commands.UpdateCompany;

public class UpdateCompanyCommandHandler
    : IRequestHandler<UpdateCompanyCommand>
{
    private readonly ICompanyRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyCommandHandler(
        ICompanyRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        if (!_currentUser.IsPlatformAdministrator &&
            request.Id != _currentUser.CompanyId.Value)
        {
            throw new NotFoundException(
                "Company not found.");
        }

        var company = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (company is null)
        {
            throw new NotFoundException(
                "Company not found.");
        }

        company.Name = request.Name;
        company.VatNumber = request.VatNumber;
        company.Address = request.Address;
        company.LogoUrl = request.LogoUrl;
		company.HrNotificationEmail =
			request.HrNotificationEmail;

        await _repository.UpdateAsync(
            company,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}