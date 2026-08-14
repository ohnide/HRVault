using FluentValidation;

namespace HRVault.Application.Employees.Queries.SearchEmployees;

public class SearchEmployeesQueryValidator
    : AbstractValidator<SearchEmployeesQuery>
{
    public SearchEmployeesQueryValidator()
    {
        RuleFor(x => x.Filter)
            .NotNull();

        When(x => x.Filter is not null, () =>
        {
            RuleFor(x => x.Filter.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Filter.PageSize)
                .InclusiveBetween(1, 100);
        });
    }
}