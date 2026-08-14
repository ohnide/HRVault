using FluentValidation;

namespace HRVault.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandValidator
    : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.EmployeeNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.WorkEmail)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.WorkEmail));

        RuleFor(x => x.PersonalEmail)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.PersonalEmail));

        RuleFor(x => x.MobilePhone)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.MobilePhone));

        RuleFor(x => x.HireDate)
            .NotEmpty();

        RuleFor(x => x.Status)
            .InclusiveBetween(1, 4);

        RuleFor(x => x.TerminationDate)
            .GreaterThanOrEqualTo(x => x.HireDate)
            .When(x => x.TerminationDate.HasValue);
    }
}