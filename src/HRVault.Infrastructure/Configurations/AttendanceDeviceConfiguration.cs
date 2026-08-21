using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class AttendanceDeviceConfiguration : IEntityTypeConfiguration<AttendanceDevice>
{
    public void Configure(EntityTypeBuilder<AttendanceDevice> builder)
    {
        builder.ToTable("AttendanceDevices");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Manufacturer).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Model).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SerialNumber).HasMaxLength(150);
        builder.Property(x => x.IpAddress).HasMaxLength(100);
        builder.Property(x => x.SettingsJson).HasColumnType("jsonb");

        builder.HasIndex(x => new { x.CompanyId, x.Name })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(x => new { x.CompanyId, x.SerialNumber })
            .IsUnique()
            .HasFilter("\"SerialNumber\" IS NOT NULL AND \"IsDeleted\" = false");

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
