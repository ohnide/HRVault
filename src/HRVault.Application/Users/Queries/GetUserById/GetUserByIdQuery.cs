using HRVault.Application.Users.DTOs;
using MediatR;

namespace HRVault.Application.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; }

    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }
}