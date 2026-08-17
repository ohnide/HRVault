using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Absences.Commands.UpdateAbsenceType;

public class UpdateAbsenceTypeCommandHandler
    : IRequestHandler<UpdateAbsenceTypeCommand>
{
    private readonly IAbsenceTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAbsenceTypeCommandHandler(
        IAbsenceTypeRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateAbsenceTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId =
            _currentUser.CompanyId.Value;

        var absenceType =
            await _repository.GetByIdAndCompanyAsync(
                request.Id,
                companyId,
                cancellationToken);

        if (absenceType is null)
        {
            throw new NotFoundException(
                "Absence type not found.");
        }

        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException(
                "Absence type name is required.");
        }

        var nameExists =
            await _repository.NameExistsAsync(
                name,
                companyId,
                request.Id,
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException(
                "An absence type with this name already exists.");
        }

        absenceType.Name = name;

        absenceType.Description =
            string.IsNullOrWhiteSpace(
                request.Description)
                ? null
                : request.Description.Trim();

        absenceType.RequiresApproval =
            request.RequiresApproval;

        absenceType.RequiresDocument =
            request.RequiresDocument;

        absenceType.IsPaid =
            request.IsPaid;

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}