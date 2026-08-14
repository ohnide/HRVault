using AutoMapper;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.DTOs;
using MediatR;

namespace HRVault.Application.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetEmployeeByIdQueryHandler(
        IEmployeeRepository repository,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<EmployeeDto?> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var employee = await _repository.GetByIdAndCompanyAsync(
            request.Id,
            _currentUser.CompanyId.Value,
            cancellationToken);

        if (employee is null)
            return null;

        return _mapper.Map<EmployeeDto>(employee);
    }
}