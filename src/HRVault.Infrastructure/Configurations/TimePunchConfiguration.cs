using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class TimePunchConfiguration : IEntityTypeConfiguration<TimePunch>
{
    public void Configure(EntityTypeBuilder<TimePunch> builder)
    {
        builder.ToTable("TimePunches");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AdjustmentReason).HasMaxLength(1000);
        builder.Property(x => x.VoidReason).HasMaxLength(1000);

        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.TimestampUtc });

        builder.HasIndex(x => x.AttendanceEventId)
            .IsUnique()
            .HasFilter("\"AttendanceEventId\" IS NOT NULL");

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AttendanceDevice)
            .WithMany(x => x.TimePunches)
            .HasForeignKey(x => x.AttendanceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AttendanceEvent)
            .WithMany(x => x.TimePunches)
            .HasForeignKey(x => x.AttendanceEventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
