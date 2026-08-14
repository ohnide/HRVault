using MediatR;

namespace HRVault.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    Guid? EmployeeId,
    string Name,
    string Email,
    string Password,
    bool IsAdministrator,
    bool IsActive
) : IRequest<Guid>;