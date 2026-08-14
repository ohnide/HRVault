using HRVault.Domain.Enums;
using MediatR;

namespace HRVault.Application.Employees.Commands.UpsertEmployeeProfile;

public record UpsertEmployeeProfileCommand(
    Guid EmployeeId,
    DateOnly? BirthDate,
    Gender? Gender,
    MaritalStatus? MaritalStatus,
    string? Nationality,
    DocumentType? DocumentType,
    string? DocumentNumber,
    string? TaxNumber,
    string? SocialSecurityNumber,
    string? SnsNumber
) : IRequest;