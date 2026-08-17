using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Absences.Commands.CreateAbsenceType;

public class CreateAbsenceTypeCommandHandler
    : IRequestHandler<CreateAbsenceTypeCommand, Guid>
{
    private readonly IAbsenceTypeRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAbsenceTypeCommandHandler(
        IAbsenceTypeRepository repository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateAbsenceTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId =
            _currentUser.CompanyId.Value;

        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleException(
                "Absence type name is required.");
        }

        var exists =
            await _repository.NameExistsAsync(
                name,
                companyId,
                cancellationToken: cancellationToken);

        if (exists)
        {
            throw new ConflictException(
                "An absence type with this name already exists.");
        }

        var absenceType =
            new AbsenceType
            {
                CompanyId = companyId,
                Name = name,
                Description =
                    string.IsNullOrWhiteSpace(
                        request.Description)
                        ? null
                        : request.Description.Trim(),
                RequiresApproval =
                    request.RequiresApproval,
                RequiresDocument =
                    request.RequiresDocument,
                IsPaid =
                    request.IsPaid
            };

        await _repository.AddAsync(
            absenceType,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return absenceType.Id;
    }
}