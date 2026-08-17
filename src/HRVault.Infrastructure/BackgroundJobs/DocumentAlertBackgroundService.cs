using HRVault.Application.Common.Interfaces;
using HRVault.Application.Documents.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HRVault.Infrastructure.BackgroundJobs;

public class DocumentAlertBackgroundService
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DocumentAlertBackgroundService> _logger;
	private readonly DocumentAlertOptions _options;

	
	public DocumentAlertBackgroundService(
		IServiceScopeFactory scopeFactory,
		ILogger<DocumentAlertBackgroundService> logger,
		IOptions<DocumentAlertOptions> options)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;
		_options = options.Value;
	}

    protected override async Task ExecuteAsync(
		CancellationToken stoppingToken)
	{
		_logger.LogInformation(
			"Document alert background service started.");

		if (_options.GenerationHour is < 0 or > 23)
		{
			throw new InvalidOperationException(
				"DocumentAlerts:GenerationHour must be between 0 and 23.");
		}

		if (_options.GenerationMinute is < 0 or > 59)
		{
			throw new InvalidOperationException(
				"DocumentAlerts:GenerationMinute must be between 0 and 59.");
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var now = DateTimeOffset.Now;

				var nextRun = new DateTimeOffset(
					now.Year,
					now.Month,
					now.Day,
					_options.GenerationHour,
					_options.GenerationMinute,
					0,
					now.Offset);

				if (nextRun <= now)
				{
					nextRun = nextRun.AddDays(1);
				}

				var delay = nextRun - now;

				_logger.LogInformation(
					"Next document alert generation scheduled for {NextRun}.",
					nextRun);

				await Task.Delay(
					delay,
					stoppingToken);

				await GenerateAlertsAsync(
					stoppingToken);
			}
			catch (OperationCanceledException)
				when (stoppingToken.IsCancellationRequested)
			{
				break;
			}
			catch (Exception ex)
			{
				_logger.LogError(
					ex,
					"Error while generating document alerts.");

				// Evita um loop rápido caso exista um erro inesperado.
				await Task.Delay(
					TimeSpan.FromMinutes(5),
					stoppingToken);
			}
		}

		_logger.LogInformation(
			"Document alert background service stopped.");
	}
	
	private async Task GenerateAlertsAsync(
		CancellationToken cancellationToken)
	{
		using var scope =
			_scopeFactory.CreateScope();
			
		var emailService =
			scope.ServiceProvider
				.GetRequiredService<IDocumentAlertEmailService>();

		var companyRepository =
			scope.ServiceProvider
				.GetRequiredService<ICompanyRepository>();

		var generator =
			scope.ServiceProvider
				.GetRequiredService<IDocumentAlertGenerator>();

		var companies =
			await companyRepository.GetAllActiveAsync(
				cancellationToken);

		var totalCreated = 0;

		foreach (var company in companies)
		{
			try
			{
				var created =
					await generator.GenerateForCompanyAsync(
						company.Id,
						cancellationToken);
						
				var emailed =
					await emailService.SendForCompanyAsync(
						company.Id,
						cancellationToken);

				_logger.LogInformation(
					"Sent {Count} document alert email items for company {CompanyId}.",
					emailed,
					company.Id);

				totalCreated += created;

				_logger.LogInformation(
					"Generated {Count} document alerts for company {CompanyId}.",
					created,
					company.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(
					ex,
					"Failed to generate document alerts for company {CompanyId}.",
					company.Id);
			}
		}

		_logger.LogInformation(
			"Document alert generation completed. Total created: {TotalCreated}.",
			totalCreated);
	}
}