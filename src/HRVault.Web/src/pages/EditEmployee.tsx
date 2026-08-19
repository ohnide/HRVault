import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";

interface Employee {
  id: string;
  companyId: string;
  departmentId?: string | null;
  positionId?: string | null;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  workEmail?: string | null;
  personalEmail?: string | null;
  mobilePhone?: string | null;
  hireDate: string;
  terminationDate?: string | null;
  contractType: number;
  status: number;
}

interface Department {
  id: string;
  companyId: string;
  name: string;
  description?: string | null;
  parentDepartmentId?: string | null;
}

interface Position {
  id: string;
  companyId: string;
  code: string;
  name: string;
  description?: string | null;
  isActive: boolean;
}

interface EmployeeProfile {
  birthDate?: string | null;
  gender?: number | null;
  maritalStatus?: number | null;
  nationality?: string | null;
  documentType?: number | null;
  documentNumber?: string | null;
  taxNumber?: string | null;
  socialSecurityNumber?: string | null;
  snsNumber?: string | null;
}

interface EmployeeAddress {
  id: string;
  type: string;
  street: string;
  postalCode: string;
  city: string;
  district?: string | null;
  country: string;
}

interface EmployeeEmergencyContact {
  id: string;
  name: string;
  relationship: string;
  phone: string;
  email?: string | null;
  notes?: string | null;
}

interface EmployeeDetailsData {
  id: string;
  profile?: EmployeeProfile | null;
  addresses: EmployeeAddress[];
  emergencyContact?: EmployeeEmergencyContact | null;
}

export default function EditEmployee() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [employee, setEmployee] =
    useState<Employee | null>(null);

  // Dados profissionais
  const [employeeNumber, setEmployeeNumber] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [workEmail, setWorkEmail] = useState("");
  const [personalEmail, setPersonalEmail] = useState("");
  const [mobilePhone, setMobilePhone] = useState("");
  const [hireDate, setHireDate] = useState("");
  const [terminationDate, setTerminationDate] = useState("");
  const [departmentId, setDepartmentId] = useState("");
  const [positionId, setPositionId] = useState("");
  const [contractType, setContractType] = useState(1);
  const [status, setStatus] = useState(1);

  // Perfil
  const [birthDate, setBirthDate] = useState("");
  const [gender, setGender] = useState("");
  const [maritalStatus, setMaritalStatus] = useState("");
  const [nationality, setNationality] = useState("");
  const [documentType, setDocumentType] = useState("");
  const [documentNumber, setDocumentNumber] = useState("");
  const [taxNumber, setTaxNumber] = useState("");
  const [socialSecurityNumber, setSocialSecurityNumber] = useState("");
  const [snsNumber, setSnsNumber] = useState("");

  // Morada principal
  const [addressId, setAddressId] = useState("");
  const [street, setStreet] = useState("");
  const [postalCode, setPostalCode] = useState("");
  const [city, setCity] = useState("");
  const [district, setDistrict] = useState("");
  const [country, setCountry] = useState("Portugal");

  // Contacto de emergência
  const [emergencyName, setEmergencyName] = useState("");
  const [emergencyRelationship, setEmergencyRelationship] = useState("");
  const [emergencyPhone, setEmergencyPhone] = useState("");
  const [emergencyEmail, setEmergencyEmail] = useState("");
  const [emergencyNotes, setEmergencyNotes] = useState("");

  const [departments, setDepartments] =
    useState<Department[]>([]);
  const [positions, setPositions] =
    useState<Position[]>([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!id) {
      setError("Funcionário inválido.");
      setLoading(false);
      return;
    }

    void loadEmployee(id);
  }, [id]);

  async function loadEmployee(employeeId: string) {
    try {
      setLoading(true);
      setError("");

      const [
        employeeResponse,
        departmentsResponse,
        positionsResponse,
        detailsResponse,
      ] = await Promise.all([
        api.get<Employee>(
          `/Employees/${employeeId}`
        ),
        api.get<Department[]>("/Departments"),
        api.get<Position[]>("/Positions"),
        api.get<EmployeeDetailsData>(
          `/Employees/${employeeId}/details`
        ),
      ]);

      const data = employeeResponse.data;
      const profile = detailsResponse.data.profile;
      const primaryAddress =
        detailsResponse.data.addresses?.[0] ?? null;
      const emergencyContact =
        detailsResponse.data.emergencyContact ?? null;

      setEmployee(data);
      setDepartments(departmentsResponse.data);
      setPositions(positionsResponse.data);

      setEmployeeNumber(data.employeeNumber);
      setFirstName(data.firstName);
      setLastName(data.lastName);
      setWorkEmail(data.workEmail ?? "");
      setPersonalEmail(data.personalEmail ?? "");
      setMobilePhone(data.mobilePhone ?? "");
      setHireDate(data.hireDate);
      setTerminationDate(data.terminationDate ?? "");
      setDepartmentId(data.departmentId ?? "");
      setPositionId(data.positionId ?? "");
      setContractType(data.contractType ?? 1);
      setStatus(data.status);

      setBirthDate(profile?.birthDate ?? "");
      setGender(
        profile?.gender != null
          ? String(profile.gender)
          : ""
      );
      setMaritalStatus(
        profile?.maritalStatus != null
          ? String(profile.maritalStatus)
          : ""
      );
      setNationality(profile?.nationality ?? "");
      setDocumentType(
        profile?.documentType != null
          ? String(profile.documentType)
          : ""
      );
      setDocumentNumber(profile?.documentNumber ?? "");
      setTaxNumber(profile?.taxNumber ?? "");
      setSocialSecurityNumber(
        profile?.socialSecurityNumber ?? ""
      );
      setSnsNumber(profile?.snsNumber ?? "");

      setAddressId(primaryAddress?.id ?? "");
      setStreet(primaryAddress?.street ?? "");
      setPostalCode(primaryAddress?.postalCode ?? "");
      setCity(primaryAddress?.city ?? "");
      setDistrict(primaryAddress?.district ?? "");
      setCountry(primaryAddress?.country ?? "Portugal");

      setEmergencyName(emergencyContact?.name ?? "");
      setEmergencyRelationship(
        emergencyContact?.relationship ?? ""
      );
      setEmergencyPhone(emergencyContact?.phone ?? "");
      setEmergencyEmail(emergencyContact?.email ?? "");
      setEmergencyNotes(emergencyContact?.notes ?? "");
    } catch (error: any) {
      console.error(
        "Erro ao carregar funcionário:",
        error
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar o funcionário."
      );
    } finally {
      setLoading(false);
    }
  }

  function hasAddressData() {
    return Boolean(
      street.trim() ||
        postalCode.trim() ||
        city.trim() ||
        district.trim()
    );
  }

  function hasEmergencyContactData() {
    return Boolean(
      emergencyName.trim() ||
        emergencyRelationship.trim() ||
        emergencyPhone.trim() ||
        emergencyEmail.trim() ||
        emergencyNotes.trim()
    );
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

    if (!employee) {
      return;
    }

    if (!employeeNumber.trim()) {
      setError("O número de funcionário é obrigatório.");
      return;
    }

    if (!firstName.trim() || !lastName.trim()) {
      setError("O nome e o apelido são obrigatórios.");
      return;
    }

    if (!hireDate) {
      setError("A data de entrada é obrigatória.");
      return;
    }

    if (
      hasAddressData() &&
      (!street.trim() || !postalCode.trim() || !city.trim())
    ) {
      setError(
        "Para guardar a morada, preencha Rua, Código postal e Localidade."
      );
      return;
    }

    if (
      hasEmergencyContactData() &&
      (!emergencyName.trim() ||
        !emergencyRelationship.trim() ||
        !emergencyPhone.trim())
    ) {
      setError(
        "Para guardar o contacto de emergência, preencha Nome, Relação e Telefone."
      );
      return;
    }

    try {
      setSaving(true);
      setError("");

      await api.put(
        `/Employees/${employee.id}`,
        {
          id: employee.id,
          companyId: employee.companyId,
          departmentId: departmentId || null,
          positionId: positionId || null,
          employeeNumber: employeeNumber.trim(),
          firstName: firstName.trim(),
          lastName: lastName.trim(),
          workEmail: workEmail.trim() || null,
          personalEmail: personalEmail.trim() || null,
          mobilePhone: mobilePhone.trim() || null,
          hireDate,
          terminationDate:
            terminationDate || null,
          contractType,
          status,
        }
      );

      await api.put(
        `/Employees/${employee.id}/profile`,
        {
          employeeId: employee.id,
          birthDate: birthDate || null,
          gender: gender ? Number(gender) : null,
          maritalStatus: maritalStatus
            ? Number(maritalStatus)
            : null,
          nationality: nationality.trim() || null,
          documentType: documentType
            ? Number(documentType)
            : null,
          documentNumber:
            documentNumber.trim() || null,
          taxNumber: taxNumber.trim() || null,
          socialSecurityNumber:
            socialSecurityNumber.trim() || null,
          snsNumber: snsNumber.trim() || null,
        }
      );

      if (hasAddressData()) {
        const addressPayload = {
          employeeId: employee.id,
          type: "Residência",
          street: street.trim(),
          postalCode: postalCode.trim(),
          city: city.trim(),
          district: district.trim() || null,
          country: country.trim() || "Portugal",
        };

        if (addressId) {
          await api.put(
            `/Employees/${employee.id}/addresses/${addressId}`,
            {
              ...addressPayload,
              addressId,
            }
          );
        } else {
          await api.post(
            `/Employees/${employee.id}/addresses`,
            addressPayload
          );
        }
      }

      if (hasEmergencyContactData()) {
        await api.put(
          `/Employees/${employee.id}/emergency-contact`,
          {
            employeeId: employee.id,
            name: emergencyName.trim(),
            relationship: emergencyRelationship.trim(),
            phone: emergencyPhone.trim(),
            email: emergencyEmail.trim() || null,
            notes: emergencyNotes.trim() || null,
          }
        );
      }

      navigate(`/employees/${employee.id}`);
    } catch (error: any) {
      console.error(
        "Erro ao atualizar funcionário:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível atualizar o funcionário."
      );
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="rounded-xl bg-white p-8 text-center shadow-sm">
        <p className="text-slate-500">
          A carregar funcionário...
        </p>
      </div>
    );
  }

  if (!employee) {
    return (
      <div>
        <button
          type="button"
          onClick={() => navigate("/employees")}
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <div className="rounded-xl border border-red-200 bg-red-50 p-5 text-red-700">
          {error || "Funcionário não encontrado."}
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <button
          type="button"
          onClick={() =>
            navigate(`/employees/${employee.id}`)
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionário
        </button>

        <h2 className="text-3xl font-bold text-slate-900">
          Editar funcionário
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          {employee.firstName} {employee.lastName}
        </p>
      </div>

      <form
        onSubmit={handleSubmit}
        className="max-w-6xl space-y-6"
      >
        <FormSection
          title="Dados profissionais"
          description="Informação principal do vínculo e enquadramento na empresa."
        >
          <Field label="Número de funcionário" required>
            <input
              type="text"
              value={employeeNumber}
              onChange={(event) =>
                setEmployeeNumber(event.target.value)
              }
              required
              className={inputClass}
            />
          </Field>

          <Field label="Email profissional">
            <input
              type="email"
              value={workEmail}
              onChange={(event) =>
                setWorkEmail(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Nome" required>
            <input
              type="text"
              value={firstName}
              onChange={(event) =>
                setFirstName(event.target.value)
              }
              required
              className={inputClass}
            />
          </Field>

          <Field label="Apelido" required>
            <input
              type="text"
              value={lastName}
              onChange={(event) =>
                setLastName(event.target.value)
              }
              required
              className={inputClass}
            />
          </Field>

          <Field label="Telemóvel">
            <input
              type="tel"
              value={mobilePhone}
              onChange={(event) =>
                setMobilePhone(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Tipo de contrato" required>
            <select
              value={contractType}
              onChange={(event) =>
                setContractType(
                  Number(event.target.value)
                )
              }
              className={inputClass}
            >
              <option value={1}>Sem termo</option>
              <option value={2}>Termo certo</option>
              <option value={3}>Termo incerto</option>
            </select>
          </Field>

          <Field label="Data de entrada" required>
            <input
              type="date"
              value={hireDate}
              onChange={(event) =>
                setHireDate(event.target.value)
              }
              required
              className={inputClass}
            />
          </Field>

          <Field label="Data de saída">
            <input
              type="date"
              value={terminationDate}
              onChange={(event) =>
                setTerminationDate(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Departamento">
            <select
              value={departmentId}
              onChange={(event) =>
                setDepartmentId(event.target.value)
              }
              className={inputClass}
            >
              <option value="">Sem departamento</option>

              {departments.map((department) => (
                <option
                  key={department.id}
                  value={department.id}
                >
                  {department.name}
                </option>
              ))}
            </select>
          </Field>

          <Field label="Cargo">
            <select
              value={positionId}
              onChange={(event) =>
                setPositionId(event.target.value)
              }
              className={inputClass}
            >
              <option value="">Sem cargo</option>

              {positions
                .filter(
                  (position) =>
                    position.isActive ||
                    position.id === positionId
                )
                .map((position) => (
                  <option
                    key={position.id}
                    value={position.id}
                  >
                    {position.code} - {position.name}
                  </option>
                ))}
            </select>
          </Field>
          
          <Field label="Estado" required>
            <select
              value={status}
              onChange={(event) =>
                setStatus(Number(event.target.value))
              }
              className={inputClass}
            >
              <option value={1}>Ativo</option>
              <option value={2}>Inativo</option>
              <option value={3}>Suspenso</option>
              <option value={4}>Terminado</option>
            </select>
          </Field>
        </FormSection>

        <FormSection
          title="Dados pessoais"
          description="Informação pessoal complementar do funcionário."
        >
          <Field label="Data de nascimento">
            <input
              type="date"
              value={birthDate}
              onChange={(event) =>
                setBirthDate(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Género">
            <select
              value={gender}
              onChange={(event) =>
                setGender(event.target.value)
              }
              className={inputClass}
            >
              <option value="">Não indicado</option>
              <option value="1">Masculino</option>
              <option value="2">Feminino</option>
              <option value="3">Outro</option>
              <option value="4">
                Prefere não indicar
              </option>
            </select>
          </Field>

          <Field label="Estado civil">
            <select
              value={maritalStatus}
              onChange={(event) =>
                setMaritalStatus(event.target.value)
              }
              className={inputClass}
            >
              <option value="">Não indicado</option>
              <option value="1">Solteiro(a)</option>
              <option value="2">Casado(a)</option>
              <option value="3">Divorciado(a)</option>
              <option value="4">Viúvo(a)</option>
              <option value="5">União de facto</option>
            </select>
          </Field>

          <Field label="Nacionalidade">
            <input
              type="text"
              value={nationality}
              onChange={(event) =>
                setNationality(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Email pessoal">
            <input
              type="email"
              value={personalEmail}
              onChange={(event) =>
                setPersonalEmail(event.target.value)
              }
              className={inputClass}
            />
          </Field>
        </FormSection>

        <FormSection
          title="Identificação"
          description="Documentos e números de identificação do funcionário."
        >
          <Field label="Tipo de documento">
            <select
              value={documentType}
              onChange={(event) =>
                setDocumentType(event.target.value)
              }
              className={inputClass}
            >
              <option value="">Não indicado</option>
              <option value="1">
                Cartão de Cidadão
              </option>
              <option value="2">Passaporte</option>
              <option value="3">
                Título de Residência
              </option>
              <option value="4">Outro</option>
            </select>
          </Field>

          <Field label="Número do documento">
            <input
              type="text"
              value={documentNumber}
              onChange={(event) =>
                setDocumentNumber(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="NIF">
            <input
              type="text"
              inputMode="numeric"
              value={taxNumber}
              onChange={(event) =>
                setTaxNumber(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="NISS">
            <input
              type="text"
              inputMode="numeric"
              value={socialSecurityNumber}
              onChange={(event) =>
                setSocialSecurityNumber(
                  event.target.value
                )
              }
              className={inputClass}
            />
          </Field>

          <Field label="Número SNS">
            <input
              type="text"
              inputMode="numeric"
              value={snsNumber}
              onChange={(event) =>
                setSnsNumber(event.target.value)
              }
              className={inputClass}
            />
          </Field>
        </FormSection>

        <FormSection
          title="Morada"
          description="Morada principal do funcionário."
        >
          <Field label="Rua">
            <input
              type="text"
              value={street}
              onChange={(event) =>
                setStreet(event.target.value)
              }
              className={inputClass}
              placeholder="Rua, número, fração"
            />
          </Field>

          <Field label="Código postal">
            <input
              type="text"
              value={postalCode}
              onChange={(event) =>
                setPostalCode(event.target.value)
              }
              className={inputClass}
              placeholder="0000-000"
            />
          </Field>

          <Field label="Localidade">
            <input
              type="text"
              value={city}
              onChange={(event) =>
                setCity(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Distrito">
            <input
              type="text"
              value={district}
              onChange={(event) =>
                setDistrict(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="País">
            <input
              type="text"
              value={country}
              onChange={(event) =>
                setCountry(event.target.value)
              }
              className={inputClass}
            />
          </Field>
        </FormSection>

        <FormSection
          title="Contacto de emergência"
          description="Pessoa a contactar em caso de emergência."
        >
          <Field label="Nome">
            <input
              type="text"
              value={emergencyName}
              onChange={(event) =>
                setEmergencyName(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Relação">
            <input
              type="text"
              value={emergencyRelationship}
              onChange={(event) =>
                setEmergencyRelationship(event.target.value)
              }
              className={inputClass}
              placeholder="Ex.: Cônjuge, Pai, Mãe"
            />
          </Field>

          <Field label="Telefone">
            <input
              type="tel"
              value={emergencyPhone}
              onChange={(event) =>
                setEmergencyPhone(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <Field label="Email">
            <input
              type="email"
              value={emergencyEmail}
              onChange={(event) =>
                setEmergencyEmail(event.target.value)
              }
              className={inputClass}
            />
          </Field>

          <div className="md:col-span-2">
            <Field label="Notas">
              <textarea
                value={emergencyNotes}
                onChange={(event) =>
                  setEmergencyNotes(event.target.value)
                }
                rows={3}
                className={inputClass}
              />
            </Field>
          </div>
        </FormSection>

        {error && (
          <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            {error}
          </div>
        )}

        <div className="flex justify-end gap-3 rounded-xl bg-white p-5 shadow-sm">
          <button
            type="button"
            onClick={() =>
              navigate(`/employees/${employee.id}`)
            }
            disabled={saving}
            className="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
          >
            Cancelar
          </button>

          <button
            type="submit"
            disabled={saving}
            className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {saving
              ? "A guardar..."
              : "Guardar alterações"}
          </button>
        </div>
      </form>
    </div>
  );
}

const inputClass =
  "w-full rounded-lg border border-slate-300 px-4 py-3 text-sm text-slate-800 outline-none focus:border-blue-500";

function Field({
  label,
  required = false,
  children,
}: {
  label: string;
  required?: boolean;
  children: React.ReactNode;
}) {
  return (
    <label className="block">
      <span className="mb-1 block text-sm font-medium text-slate-700">
        {label}
        {required && (
          <span className="ml-1 text-red-500">*</span>
        )}
      </span>

      {children}
    </label>
  );
}

function FormSection({
  title,
  description,
  children,
}: {
  title: string;
  description: string;
  children: React.ReactNode;
}) {
  return (
    <section className="rounded-xl bg-white p-6 shadow-sm">
      <div className="mb-6 border-b border-slate-100 pb-4">
        <h3 className="text-lg font-semibold text-slate-900">
          {title}
        </h3>

        <p className="mt-1 text-sm text-slate-500">
          {description}
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        {children}
      </div>
    </section>
  );
}
