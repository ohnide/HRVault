using HRVault.Application.Common.Interfaces;
using HRVault.Application.Permissions.DTOs;
using MediatR;

namespace HRVault.Application.Permissions.Queries.GetPermissionById;

public class GetPermissionByIdQueryHandler
    : IRequestHandler<GetPermissionByIdQuery, PermissionDto?>
{
    private readonly IPermissionRepository _repository;

    public GetPermissionByIdQueryHandler(
        IPermissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PermissionDto?> Handle(
        GetPermissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var permission = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (permission is null)
            return null;

        return new PermissionDto
        {
            Id = permission.Id,
            Code = permission.Code,
            Name = permission.Name,
            Description = permission.Description,
            IsActive = permission.IsActive
        };
    }
}