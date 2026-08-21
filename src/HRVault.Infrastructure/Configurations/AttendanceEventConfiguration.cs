using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class AttendanceEventConfiguration : IEntityTypeConfiguration<AttendanceEvent>
{
    public void Configure(EntityTypeBuilder<AttendanceEvent> builder)
    {
        builder.ToTable("AttendanceEvents");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalEventId).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ExternalUserId).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ReaderCode).HasMaxLength(150);
        builder.Property(x => x.RawPayload).HasColumnType("jsonb");
        builder.Property(x => x.ProcessingError).HasMaxLength(1000);

        builder.HasIndex(x => new { x.CompanyId, x.AttendanceDeviceId, x.ExternalEventId }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.TimestampUtc });
        builder.HasIndex(x => new { x.CompanyId, x.IsProcessed });

        builder.HasOne(x => x.AttendanceDevice)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.AttendanceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
