using HRVault.Application.Authentication.Interfaces;
using HRVault.Application.Common.Interfaces;
using HRVault.Infrastructure.Authentication.Jwt;
using HRVault.Infrastructure.Persistence;
using HRVault.Infrastructure.Persistence.Repositories;
using HRVault.Infrastructure.Security;
using HRVault.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HRVault.Infrastructure.BackgroundJobs;
using HRVault.Infrastructure.Storage;
using HRVault.Infrastructure.Email;

namespace HRVault.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(
                    "DefaultConnection")));

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
		
		services.Configure<DocumentAlertOptions>(
			configuration.GetSection(
				DocumentAlertOptions.SectionName));
		
		services.Configure<SmtpOptions>(
			configuration.GetSection(
				SmtpOptions.SectionName));

		services.AddScoped<
			IEmailService,
			SmtpEmailService>();
		
		services.AddHostedService<
			DocumentAlertBackgroundService>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.Configure<JwtOptions>(
            configuration.GetSection(
                JwtOptions.SectionName));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddHttpContextAccessor();
		
		services.AddScoped<
			IEmployeeProfileRepository,
			EmployeeProfileRepository>();
		
		services.AddScoped<
			IAuditLogRepository,
			AuditLogRepository>();
		
		services.AddScoped<
			IRefreshTokenRepository,
			RefreshTokenRepository>();
			
		services.AddScoped<
			IEmployeeAddressRepository,
			EmployeeAddressRepository>();
			
		services.AddScoped<
			IEmployeeContactRepository,
			EmployeeContactRepository>();
			
		services.AddScoped<
			IEmployeeEmergencyContactRepository,
			EmployeeEmergencyContactRepository>();
			
		services.AddScoped<
			IDocumentRepository,
			DocumentRepository>();
			
		services.Configure<MinioOptions>(
			configuration.GetSection(
				MinioOptions.SectionName));

		services.AddSingleton<IFileStorageService,
			MinioFileStorageService>();
			
		services.AddSingleton<
			IRefreshTokenService,
			RefreshTokenService>();
			
		services.AddScoped<
			IEmployeeDocumentTypeRepository,
			EmployeeDocumentTypeRepository>();
		
		services.AddScoped<
			IDocumentAlertRepository,
			DocumentAlertRepository>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}