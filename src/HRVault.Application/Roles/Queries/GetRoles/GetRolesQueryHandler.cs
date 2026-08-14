using AutoMapper;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Roles.DTOs;
using MediatR;

namespace HRVault.Application.Roles.Queries.GetRoles;

public class GetRolesQueryHandler
    : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IRoleRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetRolesQueryHandler(
        IRoleRepository repository,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<RoleDto>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var roles = await _repository.GetAllByCompanyAsync(
            _currentUser.CompanyId.Value,
            cancellationToken);

        return _mapper.Map<List<RoleDto>>(roles);
    }
}