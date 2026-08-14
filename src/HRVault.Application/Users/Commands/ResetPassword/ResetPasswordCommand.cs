using MediatR;

namespace HRVault.Application.Users.Commands.ResetPassword;

public record ResetPasswordCommand(
    Guid UserId,
    string NewPassword
) : IRequest;