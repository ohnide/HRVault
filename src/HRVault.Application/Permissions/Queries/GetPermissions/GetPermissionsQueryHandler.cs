using HRVault.Application.Common.Interfaces;
using HRVault.Application.Permissions.DTOs;
using MediatR;

namespace HRVault.Application.Permissions.Queries.GetPermissions;

public class GetPermissionsQueryHandler
    : IRequestHandler<GetPermissionsQuery, List<PermissionDto>>
{
    private readonly IPermissionRepository _repository;

    public GetPermissionsQueryHandler(
        IPermissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PermissionDto>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await _repository.GetAllActiveAsync(
            cancellationToken);

        return permissions
            .Select(x => new PermissionDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            })
            .ToList();
    }
}