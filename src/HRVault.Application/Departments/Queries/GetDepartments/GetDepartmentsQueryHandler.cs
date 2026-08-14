using HRVault.Application.Common.Interfaces;
using HRVault.Application.Departments.DTOs;
using MediatR;

namespace HRVault.Application.Departments.Queries.GetDepartments;

public class GetDepartmentsQueryHandler
    : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
{
    private readonly IDepartmentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetDepartmentsQueryHandler(
        IDepartmentRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<List<DepartmentDto>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var departments = await _repository.GetAllByCompanyAsync(
            _currentUser.CompanyId.Value,
            cancellationToken);

        return departments
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                CompanyId = d.CompanyId,
                ParentDepartmentId = d.ParentDepartmentId,
                Name = d.Name,
                Description = d.Description
            })
            .ToList();
    }
}