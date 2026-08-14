using AutoMapper;
using HRVault.Application.Companies.Commands.CreateCompany;
using HRVault.Application.Companies.DTOs;
using HRVault.Application.Employees.Commands.CreateEmployee;
using HRVault.Application.Employees.Commands.UpdateEmployee;
using HRVault.Application.Employees.DTOs;
using HRVault.Application.Roles.DTOs;
using HRVault.Application.Users.DTOs;
using HRVault.Domain.Entities;

namespace HRVault.Application.Common.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entities -> DTOs
        CreateMap<Employee, EmployeeDto>();
        CreateMap<Company, CompanyDto>();
        CreateMap<Role, RoleDto>();
        CreateMap<User, UserDto>();

        // Commands -> Entities
        CreateMap<CreateEmployeeCommand, Employee>()
            .ForMember(
                destination => destination.CompanyId,
                options => options.Ignore());

        CreateMap<UpdateEmployeeCommand, Employee>()
            .ForMember(
                destination => destination.Id,
                options => options.Ignore())
            .ForMember(
                destination => destination.CompanyId,
                options => options.Ignore())
            .ForMember(
                destination => destination.Status,
                options => options.Ignore());

        CreateMap<CreateCompanyCommand, Company>();
    }
}