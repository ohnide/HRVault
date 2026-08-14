using AutoMapper;
using HRVault.Application.Departments.DTOs;
using HRVault.Domain.Entities;

namespace HRVault.Application.Departments.Mapping;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<Department, DepartmentDto>();

        CreateMap<DepartmentDto, Department>();
    }
}