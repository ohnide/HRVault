using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeDetails;

public class GetEmployeeDetailsQueryHandler
    : IRequestHandler<GetEmployeeDetailsQuery, EmployeeDetailsDto?>
{
    private readonly IEmployeeRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeDetailsQueryHandler(
        IEmployeeRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<EmployeeDetailsDto?> Handle(
        GetEmployeeDetailsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var employee =
            await _repository.GetDetailsByIdAndCompanyAsync(
                request.Id,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (employee is null)
            return null;

        return new EmployeeDetailsDto
        {
            Id = employee.Id,
            CompanyId = employee.CompanyId,

            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name,

            PositionId = employee.PositionId,
            PositionName = employee.Position?.Name,

            EmployeeNumber = employee.EmployeeNumber,
            FirstName = employee.FirstName,
            LastName = employee.LastName,

            WorkEmail = employee.WorkEmail,
            PersonalEmail = employee.PersonalEmail,
            MobilePhone = employee.MobilePhone,

            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            Status = employee.Status,

            Profile = employee.Profile is null
                ? null
                : new EmployeeProfileDto
                {
                    BirthDate = employee.Profile.BirthDate,
                    Gender = employee.Profile.Gender,
                    MaritalStatus = employee.Profile.MaritalStatus,
                    Nationality = employee.Profile.Nationality,
                    DocumentType = employee.Profile.DocumentType,
                    DocumentNumber = employee.Profile.DocumentNumber,
                    TaxNumber = employee.Profile.TaxNumber,
                    SocialSecurityNumber =
                        employee.Profile.SocialSecurityNumber,
                    SnsNumber = employee.Profile.SnsNumber
                },

            Addresses = employee.Addresses
                .Select(x => new EmployeeAddressDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Street = x.Street,
                    PostalCode = x.PostalCode,
                    City = x.City,
                    District = x.District,
                    Country = x.Country
                })
                .ToList(),

            Contacts = employee.Contacts
                .Select(x => new EmployeeContactDto
                {
                    Id = x.Id,
                    Type = x.Type,
                    Value = x.Value,
                    IsPrimary = x.IsPrimary,
                    Notes = x.Notes
                })
                .ToList(),

            EmergencyContact =
                employee.EmergencyContact is null
                    ? null
                    : new EmployeeEmergencyContactDto
                    {
                        Id = employee.EmergencyContact.Id,
                        Name = employee.EmergencyContact.Name,
                        Relationship =
                            employee.EmergencyContact.Relationship,
                        Phone = employee.EmergencyContact.Phone,
                        Email = employee.EmergencyContact.Email,
                        Notes = employee.EmergencyContact.Notes
                    }
        };
    }
}