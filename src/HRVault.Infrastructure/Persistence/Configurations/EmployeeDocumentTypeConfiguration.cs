using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Persistence.Configurations;

public class EmployeeDocumentTypeConfiguration
    : IEntityTypeConfiguration<EmployeeDocumentType>
{
    public void Configure(
        EntityTypeBuilder<EmployeeDocumentType> builder)
    {
        builder.ToTable("EmployeeDocumentTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.HasExpiration)
            .IsRequired();

        builder.Property(x => x.ExpirationWarningDays);

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.Name
        })
        .IsUnique()
        .HasFilter("\"IsDeleted\" = false");

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}