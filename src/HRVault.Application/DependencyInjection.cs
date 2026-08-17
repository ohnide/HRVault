using FluentValidation;
using HRVault.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using HRVault.Application.Documents.Services;

namespace HRVault.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
		this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
				typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(
			typeof(DependencyInjection).Assembly);

		services.AddAutoMapper(
			cfg => { },
			typeof(DependencyInjection).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
			
		services.AddScoped<
			IDocumentAlertGenerator,
			DocumentAlertGenerator>();

		services.AddScoped<
			IDocumentAlertEmailService,
			DocumentAlertEmailService>();

        return services;
    }
}