using AutoMapper;
using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var employeeNumberExists =
            await _employeeRepository.EmployeeNumberExistsAsync(
                request.EmployeeNumber,
                companyId,
                cancellationToken: cancellationToken);

        if (employeeNumberExists)
        {
            throw new ConflictException(
                "An employee with this employee number already exists.");
        }

        if (request.DepartmentId.HasValue)
        {
            var department =
                await _departmentRepository.GetByIdAndCompanyAsync(
                    request.DepartmentId.Value,
                    companyId,
                    cancellationToken);

            if (department is null)
            {
                throw new NotFoundException(
                    "Department not found.");
            }
        }

        if (request.PositionId.HasValue)
        {
            var position =
                await _positionRepository.GetByIdAndCompanyAsync(
                    request.PositionId.Value,
                    companyId,
                    cancellationToken);

            if (position is null)
            {
                throw new NotFoundException(
                    "Position not found.");
            }
        }

        var employee = _mapper.Map<Employee>(request);

        employee.CompanyId = companyId;
        employee.DepartmentId = request.DepartmentId;
        employee.PositionId = request.PositionId;

        await _employeeRepository.AddAsync(
            employee,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return employee.Id;
    }
}