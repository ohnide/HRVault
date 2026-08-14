using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Companies.Commands.DeleteCompany;

public class DeleteCompanyCommandHandler
    : IRequestHandler<DeleteCompanyCommand>
{
    private readonly ICompanyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteCompanyCommandHandler(
        ICompanyRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task Handle(
        DeleteCompanyCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException();
        }

        if (!_currentUser.IsPlatformAdministrator)
        {
            throw new ForbiddenException(
                "Platform administrator access is required.");
        }

        var company = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (company is null)
        {
            throw new NotFoundException(
                "Company not found.");
        }

        await _repository.DeleteAsync(
            company,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}