using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class DocumentAlertConfiguration
    : IEntityTypeConfiguration<DocumentAlert>
{
    public void Configure(
        EntityTypeBuilder<DocumentAlert> builder)
    {
        builder.ToTable("DocumentAlerts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyId)
            .IsRequired();

        builder.Property(x => x.DocumentId)
            .IsRequired();

        builder.Property(x => x.EmployeeId)
            .IsRequired();

        builder.Property(x => x.AlertDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.EmailSent)
            .IsRequired();

        builder.HasOne(x => x.Document)
            .WithMany()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CompanyId);

        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => x.DocumentId);

        builder.HasIndex(x => x.AlertDate);

        builder.HasIndex(x => x.DocumentId)
			.IsUnique()
			.HasFilter("\"IsDeleted\" = false");
    }
}