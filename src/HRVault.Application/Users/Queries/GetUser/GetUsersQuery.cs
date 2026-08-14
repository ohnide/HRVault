using HRVault.Application.Users.DTOs;
using MediatR;

namespace HRVault.Application.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<List<UserDto>>
{
}