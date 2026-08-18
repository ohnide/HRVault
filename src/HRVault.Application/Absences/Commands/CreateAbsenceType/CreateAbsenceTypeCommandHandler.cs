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

	private static string NormalizeColor(
		string? color)
	{
		if (string.IsNullOrWhiteSpace(color))
		{
			return "#3B82F6";
		}

		var normalized = color.Trim().ToUpperInvariant();

		if (!System.Text.RegularExpressions.Regex.IsMatch(
				normalized,
				"^#[0-9A-F]{6}$"))
		{
			throw new BusinessRuleException(
				"Color must be a valid hexadecimal color in the format #RRGGBB.");
		}

		return normalized;
	}

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

		var color =
			NormalizeColor(request.Color);

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
                    request.IsPaid,
				Color = color
            };

        await _repository.AddAsync(
            absenceType,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return absenceType.Id;
    }
}