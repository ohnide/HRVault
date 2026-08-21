using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class EmployeeDeviceIdentityConfiguration : IEntityTypeConfiguration<EmployeeDeviceIdentity>
{
    public void Configure(EntityTypeBuilder<EmployeeDeviceIdentity> builder)
    {
        builder.ToTable("EmployeeDeviceIdentities");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalUserId).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CardNumber).HasMaxLength(150);

        builder.HasIndex(x => new { x.CompanyId, x.AttendanceDeviceId, x.ExternalUserId }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.AttendanceDeviceId }).IsUnique();

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AttendanceDevice)
            .WithMany(x => x.EmployeeIdentities)
            .HasForeignKey(x => x.AttendanceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
