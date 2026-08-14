using AutoMapper;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler
    : IRequestHandler<GetEmployeesQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeesQueryHandler(
        IEmployeeRepository repository,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<EmployeeDto>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var employees = await _repository.GetAllByCompanyAsync(
            _currentUser.CompanyId.Value,
            cancellationToken);

        return _mapper.Map<List<EmployeeDto>>(employees);
    }
}