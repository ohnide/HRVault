import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";

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

export default function NewEmployee() {
  const navigate = useNavigate();

  // Dados profissionais
  const [employeeNumber, setEmployeeNumber] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [workEmail, setWorkEmail] = useState("");
  const [personalEmail, setPersonalEmail] = useState("");
  const [mobilePhone, setMobilePhone] = useState("");
  const [hireDate, setHireDate] = useState("");
  const [departmentId, setDepartmentId] = useState("");
  const [positionId, setPositionId] = useState("");
  const [contractType, setContractType] = useState(1);

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

  const [departments, setDepartments] = useState<Department[]>([]);
  const [positions, setPositions] = useState<Position[]>([]);

  const [loadingReferences, setLoadingReferences] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    void loadReferenceData();
  }, []);

  async function loadReferenceData() {
    try {
      setLoadingReferences(true);

      const [departmentsResponse, positionsResponse] =
        await Promise.all([
          api.get<Department[]>("/Departments"),
          api.get<Position[]>("/Positions"),
        ]);

      setDepartments(departmentsResponse.data);
      setPositions(positionsResponse.data);
    } catch (error) {
      console.error(
        "Erro ao carregar dados auxiliares:",
        error
      );

      setError(
        "Não foi possível carregar departamentos e cargos."
      );
    } finally {
      setLoadingReferences(false);
    }
  }

  function hasProfileData() {
    return Boolean(
      birthDate ||
        gender ||
        maritalStatus ||
        nationality.trim() ||
        documentType ||
        documentNumber.trim() ||
        taxNumber.trim() ||
        socialSecurityNumber.trim() ||
        snsNumber.trim()
    );
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

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

    try {
      setSaving(true);
      setError("");

      const employeeResponse = await api.post<string>(
        "/Employees",
        {
          departmentId: departmentId || null,
          positionId: positionId || null,
          employeeNumber: employeeNumber.trim(),
          firstName: firstName.trim(),
          lastName: lastName.trim(),
          workEmail: workEmail.trim() || null,
          personalEmail: personalEmail.trim() || null,
          mobilePhone: mobilePhone.trim() || null,
          hireDate,
          contractType,
        }
      );

      const employeeId = employeeResponse.data;

      if (!employeeId) {
        throw new Error(
          "A API não devolveu o identificador do funcionário criado."
        );
      }

      if (hasProfileData()) {
        await api.put(
          `/Employees/${employeeId}/profile`,
          {
            employeeId,
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
      }

      navigate(`/employees/${employeeId}`);
    } catch (error: any) {
      console.error(
        "Erro ao criar funcionário:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível criar o funcionário."
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="space-y-6">
      <div>
        <button
          type="button"
          onClick={() => navigate("/employees")}
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <h2 className="text-3xl font-bold text-slate-900">
          Novo funcionário
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Registar os dados profissionais e pessoais do funcionário.
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
              placeholder="EMP001"
            />
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

          <Field label="Nome" required>
            <input
              type="text"
              value={firstName}
              onChange={(event) =>
                setFirstName(event.target.value)
              }
              required
              className={inputClass}
              placeholder="João"
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
              placeholder="Silva"
            />
          </Field>

          <Field label="Departamento">
            <select
              value={departmentId}
              onChange={(event) =>
                setDepartmentId(event.target.value)
              }
              disabled={loadingReferences}
              className={inputClass}
            >
              <option value="">
                Sem departamento
              </option>

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
              disabled={loadingReferences}
              className={inputClass}
            >
              <option value="">
                Sem cargo
              </option>

              {positions
                .filter((position) => position.isActive)
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

          <Field label="Tipo de contrato" required>
            <select
              value={contractType}
              onChange={(event) =>
                setContractType(
                  Number(event.target.value)
                )
              }
              className={inputClass}
              required
            >
              <option value={1}>
                Sem termo
              </option>
              <option value={2}>
                Termo certo
              </option>
              <option value={3}>
                Termo incerto
              </option>
            </select>
          </Field>

          <Field label="Email profissional">
            <input
              type="email"
              value={workEmail}
              onChange={(event) =>
                setWorkEmail(event.target.value)
              }
              className={inputClass}
              placeholder="joao@empresa.pt"
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
              placeholder="912345678"
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
              placeholder="joao@gmail.com"
            />
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
              <option value="5">
                União de facto
              </option>
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
              placeholder="Portuguesa"
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
              <option value="2">
                Passaporte
              </option>
              <option value="3">
                Título de Residência
              </option>
              <option value="4">
                Outro
              </option>
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
              placeholder="123456789"
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

        {error && (
          <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            {error}
          </div>
        )}

        <div className="flex justify-end gap-3 rounded-xl bg-white p-5 shadow-sm">
          <button
            type="button"
            onClick={() => navigate("/employees")}
            disabled={saving}
            className="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
          >
            Cancelar
          </button>

          <button
            type="submit"
            disabled={saving || loadingReferences}
            className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {saving
              ? "A guardar..."
              : "Criar funcionário"}
          </button>
        </div>
      </form>
    </div>
  );
}

const inputClass =
  "w-full rounded-lg border border-slate-300 px-4 py-3 text-sm text-slate-800 outline-none focus:border-blue-500 disabled:cursor-not-allowed disabled:bg-slate-100";

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
          <span className="ml-1 text-red-500">
            *
          </span>
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
