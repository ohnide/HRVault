using AutoMapper;
using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Roles.DTOs;
using MediatR;

namespace HRVault.Application.Users.Queries.GetUserRoles;

public class GetUserRolesQueryHandler
    : IRequestHandler<GetUserRolesQuery, List<RoleDto>>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetUserRolesQueryHandler(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<RoleDto>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var user = await _userRepository.GetByIdAndCompanyAsync(
            request.UserId,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (user is null)
            throw new NotFoundException(
                "User not found.");

        var roles = await _userRoleRepository.GetRolesByUserIdAsync(
            request.UserId,
            cancellationToken);

        return _mapper.Map<List<RoleDto>>(roles);
    }
}