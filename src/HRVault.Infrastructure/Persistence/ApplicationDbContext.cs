using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.SharedKernel.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;

namespace HRVault.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUser;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();

    public DbSet<EmployeeAddress> EmployeeAddresses => Set<EmployeeAddress>();

    public DbSet<EmployeeContact> EmployeeContacts => Set<EmployeeContact>();
	
	public DbSet<EmployeeDocumentType> DocumentTypes =>
		Set<EmployeeDocumentType>();
	
	public DbSet<RefreshToken> RefreshTokens =>
		Set<RefreshToken>();

    public DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts =>
        Set<EmployeeEmergencyContact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    public override int SaveChanges()
    {
        PrepareEntitiesForSave();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        PrepareEntitiesForSave();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void PrepareEntitiesForSave()
	{
		CreateAuditLogs();
		UpdateSoftDeleteEntities();
		UpdateAuditableEntities();
	}

	private void CreateAuditLogs()
{
    var entries = ChangeTracker
        .Entries()
        .Where(x =>
            x.Entity is not AuditLog &&
            x.Entity is not RefreshToken &&
            x.State is EntityState.Added
                or EntityState.Modified
                or EntityState.Deleted)
        .ToList();

    if (entries.Count == 0)
        return;

    var now = DateTime.UtcNow;

    foreach (var entry in entries)
    {
        var sensitiveChangeDetected = false;

        var oldValues =
            new Dictionary<string, object?>();

        var newValues =
            new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            // Nunca guardar valores sensíveis no AuditLog.
            if (IsSensitiveProperty(property.Metadata.Name))
            {
                if (entry.State == EntityState.Modified &&
                    property.IsModified &&
                    !Equals(
                        property.OriginalValue,
                        property.CurrentValue))
                {
                    sensitiveChangeDetected = true;
                }

                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    newValues[property.Metadata.Name] =
                        property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    oldValues[property.Metadata.Name] =
                        property.OriginalValue;
                    break;

                case EntityState.Modified:
                    if (!property.IsModified)
                        continue;

                    if (Equals(
                            property.OriginalValue,
                            property.CurrentValue))
                    {
                        continue;
                    }

                    oldValues[property.Metadata.Name] =
                        property.OriginalValue;

                    newValues[property.Metadata.Name] =
                        property.CurrentValue;

                    break;
            }
        }

        if (entry.State == EntityState.Modified &&
            oldValues.Count == 0 &&
            newValues.Count == 0 &&
            !sensitiveChangeDetected)
        {
            continue;
        }

        if (sensitiveChangeDetected)
        {
            newValues["SensitiveDataChanged"] = true;
        }

        var entityId = GetEntityId(entry);

        var companyId = GetCompanyId(entry);

        var auditLog = new AuditLog
        {
            CompanyId =
                companyId ?? _currentUser.CompanyId,

            UserId = _currentUser.UserId,

            UserName = _currentUser.Name,

            Action = entry.State switch
            {
                EntityState.Added => "Create",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete",
                _ => "Unknown"
            },

            EntityName =
                entry.Metadata.ClrType.Name,

            EntityId = entityId,

            OldValues = oldValues.Count > 0
                ? JsonSerializer.Serialize(oldValues)
                : null,

            NewValues = newValues.Count > 0
                ? JsonSerializer.Serialize(newValues)
                : null,

            CreatedAt = now,
			
			IpAddress = _currentUser.IpAddress
        };

        AuditLogs.Add(auditLog);
    }
}

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker
            .Entries<AuditableEntity>();

        var userId = _currentUser.UserId;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;

                    if (userId.HasValue)
                        entry.Entity.CreatedBy = userId.Value;

                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;

                    if (userId.HasValue)
                        entry.Entity.UpdatedBy = userId.Value;

                    break;
            }
        }
    }

    private void UpdateSoftDeleteEntities()
    {
        var entries = ChangeTracker
            .Entries<SoftDeleteEntity>();

        var userId = _currentUser.UserId;

        foreach (var entry in entries)
        {
            if (entry.State != EntityState.Deleted)
                continue;

            entry.State = EntityState.Modified;

            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;

            if (userId.HasValue)
                entry.Entity.DeletedBy = userId.Value;
        }
    }

    private static void ApplySoftDeleteQueryFilters(
        ModelBuilder modelBuilder)
    {
        var softDeleteEntities = modelBuilder.Model
            .GetEntityTypes()
            .Where(x =>
                typeof(SoftDeleteEntity)
                    .IsAssignableFrom(x.ClrType));

        foreach (var entityType in softDeleteEntities)
        {
            var parameter = Expression.Parameter(
                entityType.ClrType,
                "e");

            var property = Expression.Property(
                parameter,
                nameof(SoftDeleteEntity.IsDeleted));

            var compare = Expression.Equal(
                property,
                Expression.Constant(false));

            var lambda = Expression.Lambda(
                compare,
                parameter);

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(lambda);
        }
    }
	
	private static Guid? GetEntityId(
		Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
	{
		var idProperty =
			entry.Properties.FirstOrDefault(
				x => x.Metadata.Name == "Id");

		if (idProperty?.CurrentValue is Guid id)
			return id;

		if (idProperty?.OriginalValue is Guid originalId)
			return originalId;

		return null;
	}

	private Guid? GetCompanyId(
		Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
	{
		// Entidades que têm CompanyId diretamente.
		var companyProperty =
			entry.Properties.FirstOrDefault(
				x => x.Metadata.Name == "CompanyId");

		if (companyProperty?.CurrentValue is Guid companyId &&
			companyId != Guid.Empty)
		{
			return companyId;
		}

		if (companyProperty?.OriginalValue is Guid originalCompanyId &&
			originalCompanyId != Guid.Empty)
		{
			return originalCompanyId;
		}

		// A própria Company pertence a si mesma para efeitos
		// de auditoria.
		if (entry.Entity is Company company)
		{
			return company.Id;
		}

		// UserRole não tem CompanyId.
		// Tentamos descobrir através do User ou Role
		// que estejam atualmente no ChangeTracker.
		if (entry.Entity is UserRole userRole)
		{
			var trackedUser = ChangeTracker
				.Entries<User>()
				.FirstOrDefault(x =>
					x.Entity.Id == userRole.UserId);

			if (trackedUser is not null)
				return trackedUser.Entity.CompanyId;

			var trackedRole = ChangeTracker
				.Entries<Role>()
				.FirstOrDefault(x =>
					x.Entity.Id == userRole.RoleId);

			if (trackedRole is not null)
				return trackedRole.Entity.CompanyId;
		}

		// RolePermission também não tem CompanyId.
		if (entry.Entity is RolePermission rolePermission)
		{
			var trackedRole = ChangeTracker
				.Entries<Role>()
				.FirstOrDefault(x =>
					x.Entity.Id == rolePermission.RoleId);

			if (trackedRole is not null)
				return trackedRole.Entity.CompanyId;
		}

		return _currentUser.CompanyId;
	}

	private static bool IsSensitiveProperty(
		string propertyName)
	{
		return propertyName.Contains(
				   "Password",
				   StringComparison.OrdinalIgnoreCase) ||
			   propertyName.Contains(
				   "Token",
				   StringComparison.OrdinalIgnoreCase) ||
			   propertyName.Contains(
				   "Secret",
				   StringComparison.OrdinalIgnoreCase);
	}
}