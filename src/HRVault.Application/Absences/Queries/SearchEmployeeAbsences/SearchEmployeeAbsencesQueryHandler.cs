using HRVault.Application.Absences.DTOs;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using MediatR;

namespace HRVault.Application.Absences.Queries.SearchEmployeeAbsences;

public class SearchEmployeeAbsencesQueryHandler
    : IRequestHandler<
        SearchEmployeeAbsencesQuery,
        PagedResult<EmployeeAbsenceDto>>
{
    private readonly IEmployeeAbsenceRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public SearchEmployeeAbsencesQueryHandler(
        IEmployeeAbsenceRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<EmployeeAbsenceDto>> Handle(
        SearchEmployeeAbsencesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        return await _repository.SearchAsync(
            request.Filter,
            _currentUser.CompanyId.Value,
            cancellationToken);
    }
}