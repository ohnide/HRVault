using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class WorkSchedulePeriodConfiguration
    : IEntityTypeConfiguration<WorkSchedulePeriod>
{
    public void Configure(EntityTypeBuilder<WorkSchedulePeriod> builder)
    {
        builder.ToTable("WorkSchedulePeriods");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartTime)
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.EndTime)
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.WorkScheduleDayId,
            x.StartTime,
            x.EndTime
        }).IsUnique();
    }
}
