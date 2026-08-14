using HRVault.Application.Common.Interfaces;
using HRVault.Application.Departments.DTOs;
using MediatR;

namespace HRVault.Application.Departments.Queries.GetDepartmentById;

public class GetDepartmentByIdQueryHandler
    : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
{
    private readonly IDepartmentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetDepartmentByIdQueryHandler(
        IDepartmentRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<DepartmentDto?> Handle(
        GetDepartmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var department = await _repository.GetByIdAndCompanyAsync(
            request.Id,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (department is null)
            return null;

        return new DepartmentDto
        {
            Id = department.Id,
            CompanyId = department.CompanyId,
            ParentDepartmentId = department.ParentDepartmentId,
            Name = department.Name,
            Description = department.Description
        };
    }
}