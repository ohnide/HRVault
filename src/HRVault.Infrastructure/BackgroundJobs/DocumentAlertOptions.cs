namespace HRVault.Infrastructure.BackgroundJobs;

public class DocumentAlertOptions
{
    public const string SectionName = "DocumentAlerts";

    public int GenerationHour { get; set; } = 7;

    public int GenerationMinute { get; set; } = 0;
}