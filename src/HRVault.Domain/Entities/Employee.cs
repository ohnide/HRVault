using HRVault.Domain.Enums;
using HRVault.SharedKernel.Common;

namespace HRVault.Domain.Entities;

public class Employee : SoftDeleteEntity
{
    // Empresa
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    // Organização
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public Guid? PositionId { get; set; }
    public Position? Position { get; set; }

    // Identificação
    public string EmployeeNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Contactos principais
    public string? WorkEmail { get; set; }

    public string? PersonalEmail { get; set; }

    public string? MobilePhone { get; set; }

    // Datas
    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }
	
	// Contrato
	public ContractType ContractType { get; set; } = ContractType.Permanent;

    // Estado
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    // Navegação
    public EmployeeProfile? Profile { get; set; }

    public ICollection<EmployeeAddress> Addresses { get; set; } = new List<EmployeeAddress>();

    public ICollection<EmployeeContact> Contacts { get; set; } = new List<EmployeeContact>();

    public EmployeeEmergencyContact? EmergencyContact { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
	
	public ICollection<EmployeeAbsence> Absences { get; set; }
		= new List<EmployeeAbsence>();

	public ICollection<EmployeeWorkSchedule> WorkSchedules { get; set; }
		= new List<EmployeeWorkSchedule>();
		
}