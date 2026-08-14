using MediatR;

namespace HRVault.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    Guid? EmployeeId,
    string Name,
    string Email,
    bool IsAdministrator,
    bool IsActive
) : IRequest<Guid>;