using FluentValidation;
using MediatR;
using ApplicationValidationException =
    HRVault.Application.Common.Exceptions.ValidationException;

namespace HRVault.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context =
                new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(
                    validator =>
                        validator.ValidateAsync(
                            context,
                            cancellationToken)));

            var failures = results
                .SelectMany(x => x.Errors)
                .Where(x => x is not null)
                .ToList();

            if (failures.Count > 0)
            {
                var errors = failures
                    .Select(x => x.ErrorMessage)
                    .ToList();

                throw new ApplicationValidationException(
                    errors);
            }
        }

        return await next();
    }
}