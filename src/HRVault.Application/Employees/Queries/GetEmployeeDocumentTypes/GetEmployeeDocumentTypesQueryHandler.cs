using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeDocumentTypes;

public class GetEmployeeDocumentTypesQueryHandler
    : IRequestHandler<
        GetEmployeeDocumentTypesQuery,
        List<EmployeeDocumentTypeDto>>
{
    private readonly IEmployeeDocumentTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeDocumentTypesQueryHandler(
        IEmployeeDocumentTypeRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<EmployeeDocumentTypeDto>> Handle(
        GetEmployeeDocumentTypesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var types =
            await _repository.GetAllByCompanyAsync(
                _currentUser.CompanyId.Value,
                cancellationToken);

        return types
            .Select(x => new EmployeeDocumentTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                HasExpiration = x.HasExpiration,
                ExpirationWarningDays =
                    x.ExpirationWarningDays
            })
            .ToList();
    }
}