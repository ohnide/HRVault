using AutoMapper;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Users.DTOs;
using MediatR;

namespace HRVault.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetUsersQueryHandler(
        IUserRepository repository,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<UserDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var users = await _repository.GetAllByCompanyAsync(
            _currentUser.CompanyId.Value,
            cancellationToken);

        return _mapper.Map<List<UserDto>>(users);
    }
}