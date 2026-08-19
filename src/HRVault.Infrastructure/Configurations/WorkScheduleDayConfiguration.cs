using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class WorkScheduleDayConfiguration
    : IEntityTypeConfiguration<WorkScheduleDay>
{
    public void Configure(EntityTypeBuilder<WorkScheduleDay> builder)
    {
        builder.ToTable("WorkScheduleDays");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.WorkScheduleId,
            x.DayOfWeek
        }).IsUnique();

        builder.HasMany(x => x.Periods)
            .WithOne(x => x.WorkScheduleDay)
            .HasForeignKey(x => x.WorkScheduleDayId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
