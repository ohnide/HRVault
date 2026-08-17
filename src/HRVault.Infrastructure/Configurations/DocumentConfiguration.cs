using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class DocumentConfiguration
    : IEntityTypeConfiguration<Document>
{
    public void Configure(
        EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(x => x.Id);

        // Document -> Employee
        builder.HasOne(x => x.Employee)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Document -> EmployeeDocumentType
        builder.HasOne(x => x.EmployeeDocumentType)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.EmployeeDocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.EmployeeDocumentTypeId)
			.IsRequired();

		builder.HasOne(x => x.EmployeeDocumentType)
			.WithMany(x => x.Documents)
			.HasForeignKey(x => x.EmployeeDocumentTypeId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(x => x.Notes)
			.HasMaxLength(1000);

		builder.Property(x => x.IssueDate);

		builder.Property(x => x.ExpirationDate);

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StorageName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.MimeType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.IssueDate);

        builder.Property(x => x.ExpirationDate);

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.UploadedByUserId)
            .IsRequired();

        builder.Property(x => x.UploadedAt)
            .IsRequired();

        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => x.EmployeeDocumentTypeId);

        builder.HasIndex(x => x.ExpirationDate);
    }
}