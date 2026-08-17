import { useEffect, useState } from "react";
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

interface EmployeeContact {
  id: string;
  type: number;
  value: string;
  isPrimary: boolean;
  notes?: string | null;
}

interface EmployeeEmergencyContact {
  id: string;
  name: string;
  relationship: string;
  phone: string;
  email?: string | null;
  notes?: string | null;
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

interface EmployeeDocument {
  id: string;
  employeeId: string;
  employeeDocumentTypeId: string;
  employeeDocumentTypeName: string;
  issueDate?: string | null;
  expirationDate?: string | null;
  notes?: string | null;
  fileName: string;
  mimeType: string;
  size: number;
  uploadedByUserId: string;
  uploadedAt: string;
  status: string;
}

interface EmployeeDocumentType {
  id: string;
  name: string;
  description?: string | null;
  hasExpiration: boolean;
  expirationWarningDays?: number | null;
}

interface EmployeeDetailsData {
  id: string;
  companyId: string;
  departmentId?: string | null;
  departmentName?: string | null;
  positionId?: string | null;
  positionName?: string | null;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  workEmail?: string | null;
  personalEmail?: string | null;
  mobilePhone?: string | null;
  hireDate: string;
  terminationDate?: string | null;
  status: number;
  profile?: EmployeeProfile | null;

  addresses: EmployeeAddress[];
  contacts: EmployeeContact[];
  emergencyContact?: EmployeeEmergencyContact | null;
}

type EmployeeTab =
  | "overview"
  | "documents";

const tabs: Array<{
  key: EmployeeTab;
  label: string;
}> = [
  {
    key: "overview",
    label: "Visão Geral",
  },
  {
    key: "documents",
    label: "Documentos",
  },
];

export default function EmployeeDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [activeTab, setActiveTab] =
    useState<EmployeeTab>("overview");

  const [employee, setEmployee] =
    useState<Employee | null>(null);

  const [details, setDetails] =
    useState<EmployeeDetailsData | null>(null);

  const [departments, setDepartments] =
    useState<Department[]>([]);

  const [positions, setPositions] =
    useState<Position[]>([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [documents, setDocuments] =
    useState<EmployeeDocument[]>([]);

  const [documentTypes, setDocumentTypes] =
    useState<EmployeeDocumentType[]>([]);

  const [documentsLoading, setDocumentsLoading] =
    useState(false);

  const [documentsLoaded, setDocumentsLoaded] =
    useState(false);

  const [documentError, setDocumentError] =
    useState("");

  const [showUploadForm, setShowUploadForm] =
    useState(false);

  const [uploadingDocument, setUploadingDocument] =
    useState(false);

  const [deletingDocumentId, setDeletingDocumentId] =
    useState<string | null>(null);

  const [selectedDocumentTypeId, setSelectedDocumentTypeId] =
    useState("");

  const [documentIssueDate, setDocumentIssueDate] =
    useState("");

  const [documentExpirationDate, setDocumentExpirationDate] =
    useState("");

  const [documentNotes, setDocumentNotes] =
    useState("");

  const [documentFile, setDocumentFile] =
    useState<File | null>(null);

  useEffect(() => {
    if (!id) {
      setError("Funcionário inválido.");
      setLoading(false);
      return;
    }

    void loadEmployee(id);
  }, [id]);

  useEffect(() => {
    if (
      activeTab === "documents" &&
      id &&
      !documentsLoaded
    ) {
      void loadDocuments(id);
    }
  }, [
    activeTab,
    id,
    documentsLoaded,
  ]);

  async function loadDocuments(
    employeeId: string
  ) {
    try {
      setDocumentsLoading(true);
      setDocumentError("");

      const [
        documentsResponse,
        documentTypesResponse,
      ] = await Promise.all([
        api.get<EmployeeDocument[]>(
          `/Employees/${employeeId}/documents`
        ),

        api.get<EmployeeDocumentType[]>(
          "/Employees/document-types"
        ),
      ]);

      setDocuments(
        documentsResponse.data
      );

      setDocumentTypes(
        documentTypesResponse.data
      );

      setDocumentsLoaded(true);
    } catch (error: any) {
      console.error(
        "Erro ao carregar documentos:",
        error
      );

      setDocumentError(
        error.response?.data?.message ??
          "Não foi possível carregar os documentos."
      );
    } finally {
      setDocumentsLoading(false);
    }
  }

  async function uploadDocument(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!id) {
      return;
    }

    if (!selectedDocumentTypeId) {
      setDocumentError(
        "Selecione o tipo de documento."
      );
      return;
    }

    if (!documentFile) {
      setDocumentError(
        "Selecione um ficheiro."
      );
      return;
    }

    if (
      documentFile.size >
      20_000_000
    ) {
      setDocumentError(
        "O ficheiro não pode ultrapassar 20 MB."
      );
      return;
    }

    try {
      setUploadingDocument(true);
      setDocumentError("");

      const formData =
        new FormData();

      formData.append(
        "EmployeeDocumentTypeId",
        selectedDocumentTypeId
      );

      if (documentIssueDate) {
        formData.append(
          "IssueDate",
          documentIssueDate
        );
      }

      if (documentExpirationDate) {
        formData.append(
          "ExpirationDate",
          documentExpirationDate
        );
      }

      if (documentNotes.trim()) {
        formData.append(
          "Notes",
          documentNotes.trim()
        );
      }

      formData.append(
        "File",
        documentFile
      );

      await api.post(
        `/Employees/${id}/documents`,
        formData
      );

      resetUploadForm();
      setShowUploadForm(false);
      setDocumentsLoaded(false);

      await loadDocuments(id);
    } catch (error: any) {
      console.error(
        "Erro ao carregar documento:",
        error
      );

      setDocumentError(
        error.response?.data?.message ??
          "Não foi possível adicionar o documento."
      );
    } finally {
      setUploadingDocument(false);
    }
  }

  async function downloadDocument(
    document: EmployeeDocument
  ) {
    if (!id) {
      return;
    }

    try {
      setDocumentError("");

      const response =
        await api.get(
          `/Employees/${id}/documents/${document.id}/download`,
          {
            responseType: "blob",
          }
        );

      const url =
        window.URL.createObjectURL(
          response.data
        );

      const link =
        window.document.createElement(
          "a"
        );

      link.href = url;
      link.download =
        document.fileName;
      window.document.body.appendChild(
        link
      );
      link.click();
      link.remove();

      window.URL.revokeObjectURL(
        url
      );
    } catch (error: any) {
      console.error(
        "Erro ao descarregar documento:",
        error
      );

      setDocumentError(
        error.response?.data?.message ??
          "Não foi possível descarregar o documento."
      );
    }
  }

  async function deleteDocument(
    document: EmployeeDocument
  ) {
    if (!id) {
      return;
    }

    const confirmed =
      window.confirm(
        `Tem a certeza de que pretende apagar "${document.fileName}"?`
      );

    if (!confirmed) {
      return;
    }

    try {
      setDeletingDocumentId(
        document.id
      );

      setDocumentError("");

      await api.delete(
        `/Employees/${id}/documents/${document.id}`
      );

      setDocuments((current) =>
        current.filter(
          (item) =>
            item.id !== document.id
        )
      );
    } catch (error: any) {
      console.error(
        "Erro ao apagar documento:",
        error
      );

      setDocumentError(
        error.response?.data?.message ??
          "Não foi possível apagar o documento."
      );
    } finally {
      setDeletingDocumentId(
        null
      );
    }
  }

  function resetUploadForm() {
    setSelectedDocumentTypeId("");
    setDocumentIssueDate("");
    setDocumentExpirationDate("");
    setDocumentNotes("");
    setDocumentFile(null);
  }

  async function loadEmployee(
    employeeId: string
  ) {
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

        api.get<Department[]>(
          "/Departments"
        ),

        api.get<Position[]>(
          "/Positions"
        ),

        api.get<EmployeeDetailsData>(
          `/Employees/${employeeId}/details`
        ),
      ]);

      setEmployee(
        employeeResponse.data
      );

      setDepartments(
        departmentsResponse.data
      );

      setPositions(
        positionsResponse.data
      );

      setDetails(
        detailsResponse.data
      );
    } catch (error: any) {
      console.error(
        "Erro ao carregar funcionário:",
        error
      );

      if (error.response?.status === 404) {
        setError(
          "Funcionário não encontrado."
        );
      } else {
        setError(
          error.response?.data?.message ??
            "Não foi possível carregar o funcionário."
        );
      }
    } finally {
      setLoading(false);
    }
  }

  function getStatusInfo(
    status: number
  ) {
    switch (status) {
      case 1:
        return {
          label: "Ativo",
          className:
            "bg-green-100 text-green-700",
        };

      case 2:
        return {
          label: "Inativo",
          className:
            "bg-slate-100 text-slate-600",
        };

      case 3:
        return {
          label: "Suspenso",
          className:
            "bg-yellow-100 text-yellow-700",
        };

      case 4:
        return {
          label: "Terminado",
          className:
            "bg-red-100 text-red-700",
        };

      default:
        return {
          label: "Desconhecido",
          className:
            "bg-slate-100 text-slate-600",
        };
    }
  }

  function getDepartmentName(
    departmentId?: string | null
  ) {
    if (!departmentId) {
      return "-";
    }

    const department =
      departments.find(
        (item) =>
          item.id === departmentId
      );

    return (
      department?.name ??
      "Departamento não encontrado"
    );
  }

  function getPositionName(
    positionId?: string | null
  ) {
    if (!positionId) {
      return "-";
    }

    const position =
      positions.find(
        (item) =>
          item.id === positionId
      );

    if (!position) {
      return "Cargo não encontrado";
    }

    return `${position.code} - ${position.name}`;
  }

  function formatDate(
    value?: string | null
  ) {
    if (!value) {
      return "-";
    }

    return new Date(
      `${value}T00:00:00`
    ).toLocaleDateString("pt-PT");
  }

  function getGenderName(
  value?: number | null
) {
  switch (value) {
    case 1:
      return "Masculino";

    case 2:
      return "Feminino";

    case 3:
      return "Outro";

    case 4:
      return "Prefere não indicar";

    default:
      return "-";
  }
}

function getMaritalStatusName(
  value?: number | null
) {
  switch (value) {
    case 1:
      return "Solteiro(a)";

    case 2:
      return "Casado(a)";

    case 3:
      return "Divorciado(a)";

    case 4:
      return "Viúvo(a)";

    case 5:
      return "União de facto";

    default:
      return "-";
  }
}

function getDocumentTypeName(
  value?: number | null
) {
  switch (value) {
    case 1:
      return "Cartão de Cidadão";

    case 2:
      return "Passaporte";

    case 3:
      return "Título de Residência";

    case 4:
      return "Outro";

    default:
      return "-";
  }
}

function getContactTypeName(
  value: number
) {
  switch (value) {
    case 1:
      return "Telemóvel";

    case 2:
      return "Telefone";

    case 3:
      return "Email";

    case 4:
      return "WhatsApp";

    case 5:
      return "Outro";

    default:
      return "Desconhecido";
  }
}

  function getDocumentStatusInfo(
    status: string
  ) {
    const normalized =
      status.trim().toLowerCase();

    if (
      normalized === "valid" ||
      normalized === "válido" ||
      normalized === "valido"
    ) {
      return {
        label: "Válido",
        className:
          "bg-green-100 text-green-700",
        category: "valid",
      };
    }

    if (
      normalized === "expiring" ||
      normalized === "a expirar" ||
      normalized === "expiringsoon"
    ) {
      return {
        label: "A expirar",
        className:
          "bg-yellow-100 text-yellow-700",
        category: "expiring",
      };
    }

    if (
      normalized === "expired" ||
      normalized === "expirado"
    ) {
      return {
        label: "Expirado",
        className:
          "bg-red-100 text-red-700",
        category: "expired",
      };
    }

    return {
      label: status || "Sem estado",
      className:
        "bg-slate-100 text-slate-600",
      category: "other",
    };
  }

  function formatFileSize(
    size: number
  ) {
    if (size < 1024) {
      return `${size} B`;
    }

    if (size < 1024 * 1024) {
      return `${(
        size / 1024
      ).toFixed(1)} KB`;
    }

    return `${(
      size /
      (1024 * 1024)
    ).toFixed(1)} MB`;
  }

  function getInitials() {
    const first =
      employee?.firstName?.trim().charAt(0) ?? "";

    const last =
      employee?.lastName?.trim().charAt(0) ?? "";

    return `${first}${last}`.toUpperCase();
  }

  function formatAddress(
    address: EmployeeAddress
  ) {
    const locality = [
      address.postalCode,
      address.city,
    ]
      .filter(Boolean)
      .join(" ");

    return [
      address.street,
      locality,
      address.district,
      address.country,
    ]
      .filter(Boolean)
      .join(", ");
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

  if (error) {
    return (
      <div>
        <button
          onClick={() =>
            navigate("/employees")
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <div className="rounded-xl border border-red-200 bg-red-50 p-5 text-red-700">
          {error}
        </div>
      </div>
    );
  }

  if (!employee) {
    return null;
  }

  const status =
    getStatusInfo(employee.status);

  const profile =
    details?.profile ?? null;

  const contacts =
    details?.contacts ?? [];

  const addresses =
    details?.addresses ?? [];

  const emergencyContact =
    details?.emergencyContact ?? null;

  const validDocuments =
    documents.filter(
      (document) =>
        getDocumentStatusInfo(
          document.status
        ).category === "valid"
    ).length;

  const expiringDocuments =
    documents.filter(
      (document) =>
        getDocumentStatusInfo(
          document.status
        ).category === "expiring"
    ).length;

  const expiredDocuments =
    documents.filter(
      (document) =>
        getDocumentStatusInfo(
          document.status
        ).category === "expired"
    ).length;

  const selectedDocumentType =
    documentTypes.find(
      (type) =>
        type.id ===
        selectedDocumentTypeId
    );

  return (
    <div className="space-y-6">
      {/* HEADER */}
      <section className="rounded-2xl bg-white p-6 shadow-sm">
        <button
          type="button"
          onClick={() =>
            navigate("/employees")
          }
          className="mb-5 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <div className="flex flex-col gap-6 lg:flex-row lg:items-center lg:justify-between">
          <div className="flex min-w-0 items-start gap-4">
            <div className="flex h-16 w-16 shrink-0 items-center justify-center rounded-2xl bg-slate-100 text-xl font-bold text-slate-700">
              {getInitials()}
            </div>

            <div className="min-w-0">
              <div className="flex flex-wrap items-center gap-3">
                <h2 className="text-3xl font-bold text-slate-900">
                  {employee.firstName}{" "}
                  {employee.lastName}
                </h2>

                <span
                  className={`rounded-full px-3 py-1 text-xs font-medium ${status.className}`}
                >
                  {status.label}
                </span>
              </div>

              <p className="mt-1 text-sm text-slate-500">
                Funcionário{" "}
                {employee.employeeNumber}
              </p>

              <div className="mt-3 flex flex-wrap gap-x-6 gap-y-2 text-sm text-slate-600">
                <span>
                  <strong className="font-medium text-slate-800">
                    Departamento:
                  </strong>{" "}
                  {getDepartmentName(
                    employee.departmentId
                  )}
                </span>

                <span>
                  <strong className="font-medium text-slate-800">
                    Cargo:
                  </strong>{" "}
                  {getPositionName(
                    employee.positionId
                  )}
                </span>
              </div>

              <div className="mt-2 flex flex-wrap gap-x-6 gap-y-2 text-sm text-slate-500">
                {employee.workEmail && (
                  <span>
                    {employee.workEmail}
                  </span>
                )}

                {employee.mobilePhone && (
                  <span>
                    {employee.mobilePhone}
                  </span>
                )}
              </div>
            </div>
          </div>

          <button
            type="button"
            onClick={() =>
              navigate(
                `/employees/${employee.id}/edit`
              )
            }
            className="self-start rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 lg:self-center"
          >
            Editar
          </button>
        </div>
      </section>

      {/* NAVEGAÇÃO PRINCIPAL */}
      <div className="overflow-x-auto border-b border-slate-200">
        <nav className="flex min-w-max gap-6">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              type="button"
              onClick={() =>
                setActiveTab(tab.key)
              }
              className={`border-b-2 px-1 py-3 text-sm font-medium transition-colors ${
                activeTab === tab.key
                  ? "border-blue-600 text-blue-600"
                  : "border-transparent text-slate-500 hover:border-slate-300 hover:text-slate-700"
              }`}
            >
              {tab.label}
            </button>
          ))}
        </nav>
      </div>

      {/* VISÃO GERAL */}
      {activeTab === "overview" && (
        <div className="space-y-6">
          {/* Indicadores rápidos */}
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-5">
            <StatCard
              label="Data de entrada"
              value={formatDate(
                employee.hireDate
              )}
            />

            <StatCard
              label="NIF"
              value={
                profile?.taxNumber ?? "-"
              }
            />

            <StatCard
              label="NISS"
              value={
                profile?.socialSecurityNumber ??
                "-"
              }
            />

            <StatCard
              label="Nascimento"
              value={formatDate(
                profile?.birthDate
              )}
            />

            <StatCard
              label="Nacionalidade"
              value={
                profile?.nationality ?? "-"
              }
            />
          </div>

          <div className="grid grid-cols-1 gap-6 xl:grid-cols-2">
            <Card title="Dados profissionais">
              <InfoField
                label="Número de funcionário"
                value={
                  employee.employeeNumber
                }
              />

              <InfoField
                label="Departamento"
                value={getDepartmentName(
                  employee.departmentId
                )}
              />

              <InfoField
                label="Cargo"
                value={getPositionName(
                  employee.positionId
                )}
              />

              <InfoField
                label="Email profissional"
                value={
                  employee.workEmail ?? "-"
                }
              />

              <InfoField
                label="Telemóvel"
                value={
                  employee.mobilePhone ?? "-"
                }
              />

              <InfoField
                label="Data de saída"
                value={formatDate(
                  employee.terminationDate
                )}
              />
            </Card>

            <Card title="Dados pessoais">
              <InfoField
                label="Nome completo"
                value={`${employee.firstName} ${employee.lastName}`}
              />

              <InfoField
                label="Data de nascimento"
                value={formatDate(
                  profile?.birthDate
                )}
              />

              <InfoField
                label="Género"
                value={getGenderName(
                  profile?.gender
                )}
              />

              <InfoField
                label="Estado civil"
                value={getMaritalStatusName(
                  profile?.maritalStatus
                )}
              />

              <InfoField
                label="Nacionalidade"
                value={
                  profile?.nationality ?? "-"
                }
              />

              <InfoField
                label="Email pessoal"
                value={
                  employee.personalEmail ??
                  "-"
                }
              />
            </Card>

            <Card title="Identificação">
              <InfoField
                label="Tipo de documento"
                value={getDocumentTypeName(
                  profile?.documentType
                )}
              />

              <InfoField
                label="Número do documento"
                value={
                  profile?.documentNumber ??
                  "-"
                }
              />

              <InfoField
                label="NIF"
                value={
                  profile?.taxNumber ?? "-"
                }
              />

              <InfoField
                label="NISS"
                value={
                  profile?.socialSecurityNumber ??
                  "-"
                }
              />

              <InfoField
                label="Número de utente SNS"
                value={
                  profile?.snsNumber ?? "-"
                }
              />
            </Card>

            <Card title="Contacto de emergência">
              {!emergencyContact ? (
                <EmptyState
                  title="Sem contacto de emergência"
                  description="Ainda não foi definido um contacto de emergência para este funcionário."
                />
              ) : (
                <>
                  <InfoField
                    label="Nome"
                    value={
                      emergencyContact.name
                    }
                  />

                  <InfoField
                    label="Relação"
                    value={
                      emergencyContact.relationship ||
                      "-"
                    }
                  />

                  <InfoField
                    label="Telefone"
                    value={
                      emergencyContact.phone ||
                      "-"
                    }
                  />

                  <InfoField
                    label="Email"
                    value={
                      emergencyContact.email ??
                      "-"
                    }
                  />

                  {emergencyContact.notes && (
                    <InfoField
                      label="Notas"
                      value={
                        emergencyContact.notes
                      }
                    />
                  )}
                </>
              )}
            </Card>
          </div>

          {/* CONTACTOS */}
          <section className="rounded-xl bg-white p-6 shadow-sm">
            <div className="mb-5 flex flex-wrap items-center justify-between gap-3">
              <div>
                <h3 className="text-lg font-semibold text-slate-900">
                  Contactos adicionais
                </h3>

                <p className="mt-1 text-sm text-slate-500">
                  Outros contactos associados ao funcionário.
                </p>
              </div>

              {contacts.length > 0 && (
                <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-600">
                  {contacts.length}{" "}
                  {contacts.length === 1
                    ? "contacto"
                    : "contactos"}
                </span>
              )}
            </div>

            {contacts.length === 0 ? (
              <EmptyState
                title="Sem contactos adicionais"
                description="Este funcionário ainda não tem contactos adicionais registados."
              />
            ) : (
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
                {contacts.map(
                  (contact) => (
                    <div
                      key={contact.id}
                      className="rounded-lg border border-slate-200 p-5"
                    >
                      <div className="flex items-start justify-between gap-4">
                        <div className="min-w-0">
                          <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                            {getContactTypeName(
                              contact.type
                            )}
                          </p>

                          <p className="mt-1 break-all text-sm font-medium text-slate-800">
                            {contact.value}
                          </p>
                        </div>

                        {contact.isPrimary && (
                          <span className="shrink-0 rounded-full bg-blue-100 px-2.5 py-1 text-xs font-medium text-blue-700">
                            Principal
                          </span>
                        )}
                      </div>

                      {contact.notes && (
                        <p className="mt-3 border-t border-slate-100 pt-3 text-sm text-slate-500">
                          {contact.notes}
                        </p>
                      )}
                    </div>
                  )
                )}
              </div>
            )}
          </section>

          {/* MORADAS */}
          <section className="rounded-xl bg-white p-6 shadow-sm">
            <div className="mb-5 flex flex-wrap items-center justify-between gap-3">
              <div>
                <h3 className="text-lg font-semibold text-slate-900">
                  Moradas
                </h3>

                <p className="mt-1 text-sm text-slate-500">
                  Moradas registadas para o funcionário.
                </p>
              </div>

              {addresses.length > 0 && (
                <span className="rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-600">
                  {addresses.length}{" "}
                  {addresses.length === 1
                    ? "morada"
                    : "moradas"}
                </span>
              )}
            </div>

            {addresses.length === 0 ? (
              <EmptyState
                title="Sem moradas"
                description="Este funcionário ainda não tem moradas registadas."
              />
            ) : (
              <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
                {addresses.map(
                  (address) => (
                    <div
                      key={address.id}
                      className="rounded-lg border border-slate-200 p-5"
                    >
                      <div className="mb-3 flex items-center justify-between gap-3">
                        <span className="rounded-full bg-slate-100 px-2.5 py-1 text-xs font-medium text-slate-600">
                          {address.type ||
                            "Morada"}
                        </span>
                      </div>

                      <p className="text-sm font-medium text-slate-800">
                        {address.street || "-"}
                      </p>

                      <p className="mt-1 text-sm text-slate-600">
                        {[
                          address.postalCode,
                          address.city,
                        ]
                          .filter(Boolean)
                          .join(" ")}
                      </p>

                      {address.district && (
                        <p className="mt-1 text-sm text-slate-500">
                          {address.district}
                        </p>
                      )}

                      <p className="mt-1 text-sm text-slate-500">
                        {address.country || "-"}
                      </p>

                      <p className="mt-3 text-xs text-slate-400">
                        {formatAddress(
                          address
                        )}
                      </p>
                    </div>
                  )
                )}
              </div>
            )}
          </section>
        </div>
      )}

      {/* DOCUMENTOS */}
      {activeTab === "documents" && (
        <div className="space-y-6">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <h3 className="text-xl font-semibold text-slate-900">
                Documentos do funcionário
              </h3>

              <p className="mt-1 text-sm text-slate-500">
                Consulte e gira a documentação associada a este funcionário.
              </p>
            </div>

            <button
              type="button"
              onClick={() => {
                setDocumentError("");
                setShowUploadForm(
                  (current) => !current
                );
              }}
              className="self-start rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
            >
              {showUploadForm
                ? "Cancelar"
                : "+ Adicionar documento"}
            </button>
          </div>

          {documentError && (
            <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
              {documentError}
            </div>
          )}

          {showUploadForm && (
            <section className="rounded-xl bg-white p-6 shadow-sm">
              <h4 className="text-lg font-semibold text-slate-900">
                Adicionar documento
              </h4>

              <form
                onSubmit={
                  uploadDocument
                }
                className="mt-5 space-y-5"
              >
                <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
                  <label className="block">
                    <span className="text-sm font-medium text-slate-700">
                      Tipo de documento *
                    </span>

                    <select
                      value={
                        selectedDocumentTypeId
                      }
                      onChange={(event) => {
                        const value =
                          event.target.value;

                        setSelectedDocumentTypeId(
                          value
                        );

                        const selectedType =
                          documentTypes.find(
                            (type) =>
                              type.id === value
                          );

                        if (
                          !selectedType?.hasExpiration
                        ) {
                          setDocumentExpirationDate(
                            ""
                          );
                        }
                      }}
                      required
                      className="mt-1 w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500"
                    >
                      <option value="">
                        Selecionar...
                      </option>

                      {documentTypes.map(
                        (type) => (
                          <option
                            key={type.id}
                            value={type.id}
                          >
                            {type.name}
                          </option>
                        )
                      )}
                    </select>

                    {selectedDocumentType?.description && (
                      <p className="mt-1 text-xs text-slate-500">
                        {
                          selectedDocumentType.description
                        }
                      </p>
                    )}
                  </label>

                  <label className="block">
                    <span className="text-sm font-medium text-slate-700">
                      Ficheiro *
                    </span>

                    <input
                      type="file"
                      required
                      onChange={(event) =>
                        setDocumentFile(
                          event.target
                            .files?.[0] ??
                            null
                        )
                      }
                      className="mt-1 block w-full text-sm text-slate-600 file:mr-4 file:rounded-lg file:border-0 file:bg-slate-100 file:px-4 file:py-2.5 file:text-sm file:font-medium file:text-slate-700 hover:file:bg-slate-200"
                    />

                    <p className="mt-1 text-xs text-slate-500">
                      Tamanho máximo: 20 MB.
                    </p>
                  </label>

                  <label className="block">
                    <span className="text-sm font-medium text-slate-700">
                      Data de emissão
                    </span>

                    <input
                      type="date"
                      value={
                        documentIssueDate
                      }
                      onChange={(event) =>
                        setDocumentIssueDate(
                          event.target.value
                        )
                      }
                      className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500"
                    />
                  </label>

                  <label className="block">
                    <span className="text-sm font-medium text-slate-700">
                      Data de validade
                      {selectedDocumentType?.hasExpiration
                        ? " *"
                        : ""}
                    </span>

                    <input
                      type="date"
                      value={
                        documentExpirationDate
                      }
                      onChange={(event) =>
                        setDocumentExpirationDate(
                          event.target.value
                        )
                      }
                      required={
                        selectedDocumentType?.hasExpiration ===
                        true
                      }
                      disabled={
                        !selectedDocumentType?.hasExpiration
                      }
                      className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500 disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400"
                    />

                    {selectedDocumentType?.hasExpiration &&
                      selectedDocumentType.expirationWarningDays !=
                        null && (
                        <p className="mt-1 text-xs text-slate-500">
                          Alerta configurado para{" "}
                          {
                            selectedDocumentType.expirationWarningDays
                          }{" "}
                          dias antes da validade.
                        </p>
                      )}
                  </label>
                </div>

                <label className="block">
                  <span className="text-sm font-medium text-slate-700">
                    Notas
                  </span>

                  <textarea
                    value={
                      documentNotes
                    }
                    onChange={(event) =>
                      setDocumentNotes(
                        event.target.value
                      )
                    }
                    rows={3}
                    className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500"
                  />
                </label>

                <div className="flex flex-wrap justify-end gap-3">
                  <button
                    type="button"
                    onClick={() => {
                      resetUploadForm();
                      setShowUploadForm(
                        false
                      );
                    }}
                    className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
                  >
                    Cancelar
                  </button>

                  <button
                    type="submit"
                    disabled={
                      uploadingDocument
                    }
                    className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
                  >
                    {uploadingDocument
                      ? "A enviar..."
                      : "Guardar documento"}
                  </button>
                </div>
              </form>
            </section>
          )}

          <div className="grid grid-cols-2 gap-4 xl:grid-cols-4">
            <DocumentStatCard
              label="Total"
              value={documents.length}
            />

            <DocumentStatCard
              label="Válidos"
              value={validDocuments}
            />

            <DocumentStatCard
              label="A expirar"
              value={expiringDocuments}
            />

            <DocumentStatCard
              label="Expirados"
              value={expiredDocuments}
            />
          </div>

          <section className="overflow-hidden rounded-xl bg-white shadow-sm">
            {documentsLoading ? (
              <div className="p-8 text-center text-sm text-slate-500">
                A carregar documentos...
              </div>
            ) : documents.length === 0 ? (
              <div className="p-6">
                <EmptyState
                  title="Sem documentos"
                  description="Este funcionário ainda não tem documentos registados."
                />
              </div>
            ) : (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                  <thead className="bg-slate-50">
                    <tr>
                      <TableHeader>
                        Tipo
                      </TableHeader>

                      <TableHeader>
                        Ficheiro
                      </TableHeader>

                      <TableHeader>
                        Emissão
                      </TableHeader>

                      <TableHeader>
                        Validade
                      </TableHeader>

                      <TableHeader>
                        Estado
                      </TableHeader>

                      <TableHeader>
                        Ações
                      </TableHeader>
                    </tr>
                  </thead>

                  <tbody className="divide-y divide-slate-100 bg-white">
                    {documents.map(
                      (document) => {
                        const documentStatus =
                          getDocumentStatusInfo(
                            document.status
                          );

                        return (
                          <tr
                            key={
                              document.id
                            }
                            className="hover:bg-slate-50"
                          >
                            <TableCell>
                              <div>
                                <p className="font-medium text-slate-800">
                                  {
                                    document.employeeDocumentTypeName
                                  }
                                </p>

                                {document.notes && (
                                  <p className="mt-1 max-w-xs truncate text-xs text-slate-500">
                                    {
                                      document.notes
                                    }
                                  </p>
                                )}
                              </div>
                            </TableCell>

                            <TableCell>
                              <div>
                                <p className="max-w-xs truncate text-slate-700">
                                  {
                                    document.fileName
                                  }
                                </p>

                                <p className="mt-1 text-xs text-slate-400">
                                  {formatFileSize(
                                    document.size
                                  )}
                                </p>
                              </div>
                            </TableCell>

                            <TableCell>
                              {formatDate(
                                document.issueDate
                              )}
                            </TableCell>

                            <TableCell>
                              {formatDate(
                                document.expirationDate
                              )}
                            </TableCell>

                            <TableCell>
                              <span
                                className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${documentStatus.className}`}
                              >
                                {
                                  documentStatus.label
                                }
                              </span>
                            </TableCell>

                            <TableCell>
                              <div className="flex items-center gap-3">
                                <button
                                  type="button"
                                  onClick={() =>
                                    void downloadDocument(
                                      document
                                    )
                                  }
                                  className="text-sm font-medium text-blue-600 hover:text-blue-700"
                                >
                                  Download
                                </button>

                                <button
                                  type="button"
                                  disabled={
                                    deletingDocumentId ===
                                    document.id
                                  }
                                  onClick={() =>
                                    void deleteDocument(
                                      document
                                    )
                                  }
                                  className="text-sm font-medium text-red-600 hover:text-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                                >
                                  {deletingDocumentId ===
                                  document.id
                                    ? "A apagar..."
                                    : "Apagar"}
                                </button>
                              </div>
                            </TableCell>
                          </tr>
                        );
                      }
                    )}
                  </tbody>
                </table>
              </div>
            )}
          </section>
        </div>
      )}
    </div>
  );
}

interface CardProps {
  title: string;
  children: React.ReactNode;
}

function Card({
  title,
  children,
}: CardProps) {
  return (
    <section className="rounded-xl bg-white p-6 shadow-sm">
      <h3 className="mb-5 text-lg font-semibold text-slate-900">
        {title}
      </h3>

      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
        {children}
      </div>
    </section>
  );
}

interface StatCardProps {
  label: string;
  value: string;
}

function StatCard({
  label,
  value,
}: StatCardProps) {
  return (
    <div className="rounded-xl bg-white p-5 shadow-sm">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
        {label}
      </p>

      <p className="mt-2 text-base font-semibold text-slate-800">
        {value}
      </p>
    </div>
  );
}

interface DocumentStatCardProps {
  label: string;
  value: number;
}

function DocumentStatCard({
  label,
  value,
}: DocumentStatCardProps) {
  return (
    <div className="rounded-xl bg-white p-5 shadow-sm">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
        {label}
      </p>

      <p className="mt-2 text-2xl font-bold text-slate-900">
        {value}
      </p>
    </div>
  );
}

interface TableContentProps {
  children: React.ReactNode;
}

function TableHeader({
  children,
}: TableContentProps) {
  return (
    <th className="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
      {children}
    </th>
  );
}

function TableCell({
  children,
}: TableContentProps) {
  return (
    <td className="whitespace-nowrap px-5 py-4 text-sm text-slate-600">
      {children}
    </td>
  );
}

interface InfoFieldProps {
  label: string;
  value: string;
}

function InfoField({
  label,
  value,
}: InfoFieldProps) {
  return (
    <div>
      <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
        {label}
      </p>

      <p className="mt-1 whitespace-pre-wrap text-sm text-slate-800">
        {value}
      </p>
    </div>
  );
}

interface EmptyStateProps {
  title: string;
  description: string;
}

function EmptyState({
  title,
  description,
}: EmptyStateProps) {
  return (
    <div className="col-span-full rounded-lg border border-dashed border-slate-300 p-6 text-center">
      <p className="text-sm font-medium text-slate-700">
        {title}
      </p>

      <p className="mt-1 text-sm text-slate-500">
        {description}
      </p>
    </div>
  );
}
