using HRVault.Application.Absences.DTOs;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Absences.Queries.GetAbsenceTypes;

public class GetAbsenceTypesQueryHandler
    : IRequestHandler<
        GetAbsenceTypesQuery,
        List<AbsenceTypeDto>>
{
    private readonly IAbsenceTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetAbsenceTypesQueryHandler(
        IAbsenceTypeRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<AbsenceTypeDto>> Handle(
        GetAbsenceTypesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var types =
            await _repository.GetAllByCompanyAsync(
                _currentUser.CompanyId.Value,
                cancellationToken);

        return types
            .Select(x => new AbsenceTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                RequiresApproval =
                    x.RequiresApproval,
                RequiresDocument =
                    x.RequiresDocument,
                IsPaid = x.IsPaid
            })
            .ToList();
    }
}