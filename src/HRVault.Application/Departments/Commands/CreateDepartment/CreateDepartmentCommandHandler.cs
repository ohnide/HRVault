using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler
    : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateDepartmentCommandHandler(
        IDepartmentRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        if (request.ParentDepartmentId.HasValue)
        {
            var parentDepartment =
                await _repository.GetByIdAndCompanyAsync(
                    request.ParentDepartmentId.Value,
                    companyId,
                    cancellationToken);

            if (parentDepartment is null)
                throw new NotFoundException(
                    "Parent department not found.");
        }

        var department = new Department
        {
            CompanyId = companyId,
            Name = request.Name,
            Description = request.Description,
            ParentDepartmentId = request.ParentDepartmentId
        };

        await _repository.AddAsync(
            department,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return department.Id;
    }
}