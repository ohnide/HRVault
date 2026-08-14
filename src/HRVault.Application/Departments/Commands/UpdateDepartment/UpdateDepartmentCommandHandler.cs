using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler
    : IRequestHandler<UpdateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateDepartmentCommandHandler(
        IDepartmentRepository repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(
        UpdateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var department = await _repository.GetByIdAndCompanyAsync(
            request.Id,
            companyId,
            cancellationToken);

        if (department is null)
            throw new NotFoundException(
                "Department not found.");

        if (request.ParentDepartmentId.HasValue)
        {
            var parentDepartmentId =
                request.ParentDepartmentId.Value;

            if (parentDepartmentId == department.Id)
            {
                throw new BusinessRuleException(
                    "A department cannot be its own parent.");
            }

            var parentDepartment =
                await _repository.GetByIdAndCompanyAsync(
                    parentDepartmentId,
                    companyId,
                    cancellationToken);

            if (parentDepartment is null)
            {
                throw new NotFoundException(
                    "Parent department not found.");
            }

            var wouldCreateCycle =
                await _repository.WouldCreateCycleAsync(
                    department.Id,
                    parentDepartmentId,
                    companyId,
                    cancellationToken);

            if (wouldCreateCycle)
            {
                throw new BusinessRuleException(
                    "The selected parent department would create a hierarchy cycle.");
            }
        }

        department.Name = request.Name;
        department.Description = request.Description;
        department.ParentDepartmentId =
            request.ParentDepartmentId;

        await _repository.UpdateAsync(
            department,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return department.Id;
    }
}