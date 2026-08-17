import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
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
  status: number | string;
}

interface Department {
  id: string;
  name: string;
}

interface DocumentSummary {
  total: number;
  valid: number;
  expiring: number;
  expired: number;
}

interface ExpiringDocument {
  documentId: string;
  employeeId: string;
  employeeName: string;
  employeeDocumentTypeId: string;
  documentTypeName: string;
  fileName: string;
  expirationDate?: string | null;
  daysRemaining?: number | null;
  status: string;
}

interface PagedResult<T> {
  items: T[];
  totalCount?: number;
  page?: number;
  pageSize?: number;
  totalPages?: number;
}

export default function Dashboard() {
  const navigate = useNavigate();

  const [employees, setEmployees] =
    useState<Employee[]>([]);
  const [departments, setDepartments] =
    useState<Department[]>([]);
  const [documentSummary, setDocumentSummary] =
    useState<DocumentSummary>({
      total: 0,
      valid: 0,
      expiring: 0,
      expired: 0,
    });
  const [attentionDocuments, setAttentionDocuments] =
    useState<ExpiringDocument[]>([]);
  const [loading, setLoading] =
    useState(true);
  const [error, setError] =
    useState("");

  useEffect(() => {
    void loadDashboard();
  }, []);

  async function loadDashboard() {
    try {
      setLoading(true);
      setError("");

      const [
        employeesResult,
        departmentsResult,
        summaryResult,
        expiringResult,
        expiredResult,
      ] = await Promise.allSettled([
        api.get<Employee[]>("/Employees"),
        api.get<Department[]>("/Departments"),
        api.get<DocumentSummary>(
          "/Documents/summary"
        ),
        api.get<PagedResult<ExpiringDocument>>(
          "/Documents/search",
          {
            params: {
              Status: "Expiring",
              Page: 1,
              PageSize: 5,
            },
          }
        ),
        api.get<PagedResult<ExpiringDocument>>(
          "/Documents/search",
          {
            params: {
              Status: "Expired",
              Page: 1,
              PageSize: 5,
            },
          }
        ),
      ]);

      if (
        employeesResult.status ===
        "fulfilled"
      ) {
        setEmployees(
          employeesResult.value.data
        );
      }

      if (
        departmentsResult.status ===
        "fulfilled"
      ) {
        setDepartments(
          departmentsResult.value.data
        );
      }

      if (
        summaryResult.status ===
        "fulfilled"
      ) {
        setDocumentSummary(
          summaryResult.value.data
        );
      }

      const expiring =
        expiringResult.status ===
        "fulfilled"
          ? getPagedItems(
              expiringResult.value.data
            )
          : [];

      const expired =
        expiredResult.status ===
        "fulfilled"
          ? getPagedItems(
              expiredResult.value.data
            )
          : [];

      setAttentionDocuments(
        [...expired, ...expiring]
          .sort(sortDocumentsByUrgency)
          .slice(0, 8)
      );

      const failedRequests = [
        employeesResult,
        departmentsResult,
        summaryResult,
        expiringResult,
        expiredResult,
      ].filter(
        (result) =>
          result.status === "rejected"
      );

      if (failedRequests.length > 0) {
        console.error(
          "Alguns dados do dashboard não foram carregados:",
          failedRequests
        );

        setError(
          "Alguns dados do dashboard não puderam ser carregados."
        );
      }
    } catch (error) {
      console.error(
        "Erro ao carregar dashboard:",
        error
      );

      setError(
        "Não foi possível carregar o dashboard."
      );
    } finally {
      setLoading(false);
    }
  }

  function getPagedItems(
    data:
      | PagedResult<ExpiringDocument>
      | ExpiringDocument[]
  ) {
    if (Array.isArray(data)) {
      return data;
    }

    return data.items ?? [];
  }

  function sortDocumentsByUrgency(
    first: ExpiringDocument,
    second: ExpiringDocument
  ) {
    const firstDays =
      first.daysRemaining ??
      Number.MAX_SAFE_INTEGER;

    const secondDays =
      second.daysRemaining ??
      Number.MAX_SAFE_INTEGER;

    return firstDays - secondDays;
  }

  function formatDate(
    value?: string | null
  ) {
    if (!value) {
      return "-";
    }

    const parts = value
      .substring(0, 10)
      .split("-");

    if (parts.length !== 3) {
      return value;
    }

    return `${parts[2]}/${parts[1]}/${parts[0]}`;
  }

  function getDocumentStatus(
    document: ExpiringDocument
  ) {
    const normalized =
      document.status
        ?.trim()
        .toLowerCase() ?? "";

    if (
      normalized === "expired" ||
      normalized === "expirado" ||
      (document.daysRemaining != null &&
        document.daysRemaining < 0)
    ) {
      return {
        label: "Expirado",
        className:
          "bg-red-100 text-red-700",
      };
    }

    return {
      label: "A expirar",
      className:
        "bg-amber-100 text-amber-700",
    };
  }

  function getDaysLabel(
    days?: number | null
  ) {
    if (days == null) {
      return "-";
    }

    if (days < 0) {
      const overdue = Math.abs(days);

      return overdue === 1
        ? "1 dia em atraso"
        : `${overdue} dias em atraso`;
    }

    if (days === 0) {
      return "Hoje";
    }

    return days === 1
      ? "1 dia"
      : `${days} dias`;
  }

  const activeEmployees =
    useMemo(
      () =>
        employees.filter(
          (employee) =>
            !employee.terminationDate
        ).length,
      [employees]
    );

  const departmentDistribution =
    useMemo(() => {
      const departmentMap =
        new Map(
          departments.map(
            (department) => [
              department.id,
              department.name,
            ]
          )
        );

      const counts =
        new Map<string, number>();

      for (const employee of employees) {
        if (employee.terminationDate) {
          continue;
        }

        const name =
          employee.departmentId
            ? departmentMap.get(
                employee.departmentId
              ) ?? "Sem departamento"
            : "Sem departamento";

        counts.set(
          name,
          (counts.get(name) ?? 0) + 1
        );
      }

      return Array.from(
        counts.entries()
      )
        .map(([name, count]) => ({
          name,
          count,
        }))
        .sort(
          (a, b) => b.count - a.count
        );
    }, [employees, departments]);

  const maxDepartmentCount =
    Math.max(
      ...departmentDistribution.map(
        (item) => item.count
      ),
      1
    );

  if (loading) {
    return (
      <div className="rounded-xl bg-white p-8 text-center text-slate-500 shadow-sm">
        A carregar dashboard...
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold text-slate-900">
          Dashboard
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Visão geral dos recursos humanos e da documentação da empresa.
        </p>
      </div>

      {error && (
        <div className="rounded-xl border border-amber-200 bg-amber-50 p-4 text-sm text-amber-700">
          {error}
        </div>
      )}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <DashboardCard
          label="Funcionários"
          value={employees.length}
          detail={`${activeEmployees} ativos`}
          onClick={() =>
            navigate("/employees")
          }
        />

        <DashboardCard
          label="Documentos"
          value={documentSummary.total}
          detail={`${documentSummary.valid} válidos`}
        />

        <DashboardCard
          label="A expirar"
          value={documentSummary.expiring}
          detail="Requerem atenção"
          tone="warning"
        />

        <DashboardCard
          label="Expirados"
          value={documentSummary.expired}
          detail="Requerem ação"
          tone="danger"
        />
      </div>

      <section className="overflow-hidden rounded-xl bg-white shadow-sm">
        <div className="flex flex-col gap-3 border-b border-slate-100 px-6 py-5 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h3 className="text-lg font-semibold text-slate-900">
              Documentos que requerem atenção
            </h3>

            <p className="mt-1 text-sm text-slate-500">
              Documentos expirados ou próximos da data de validade.
            </p>
          </div>
        </div>

        {attentionDocuments.length ===
        0 ? (
          <div className="p-8 text-center">
            <p className="text-sm font-medium text-slate-700">
              Nenhum documento requer atenção.
            </p>

            <p className="mt-1 text-sm text-slate-500">
              Não existem documentos expirados ou a expirar nos resultados atuais.
            </p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-slate-200">
              <thead className="bg-slate-50">
                <tr>
                  <TableHeader>
                    Funcionário
                  </TableHeader>
                  <TableHeader>
                    Documento
                  </TableHeader>
                  <TableHeader>
                    Validade
                  </TableHeader>
                  <TableHeader>
                    Dias
                  </TableHeader>
                  <TableHeader>
                    Estado
                  </TableHeader>
                </tr>
              </thead>

              <tbody className="divide-y divide-slate-100">
                {attentionDocuments.map(
                  (document) => {
                    const status =
                      getDocumentStatus(
                        document
                      );

                    return (
                      <tr
                        key={
                          document.documentId
                        }
                        onClick={() =>
                          navigate(
                            `/employees/${document.employeeId}`
                          )
                        }
                        className="cursor-pointer hover:bg-slate-50"
                      >
                        <TableCell>
                          <div>
                            <p className="font-medium text-slate-800">
                              {
                                document.employeeName
                              }
                            </p>
                          </div>
                        </TableCell>

                        <TableCell>
                          <div>
                            <p className="font-medium text-slate-700">
                              {
                                document.documentTypeName
                              }
                            </p>

                            <p className="mt-1 max-w-xs truncate text-xs text-slate-400">
                              {
                                document.fileName
                              }
                            </p>
                          </div>
                        </TableCell>

                        <TableCell>
                          {formatDate(
                            document.expirationDate
                          )}
                        </TableCell>

                        <TableCell>
                          {getDaysLabel(
                            document.daysRemaining
                          )}
                        </TableCell>

                        <TableCell>
                          <span
                            className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${status.className}`}
                          >
                            {status.label}
                          </span>
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

      <div className="grid grid-cols-1 gap-6 xl:grid-cols-3">
        <section className="rounded-xl bg-white p-6 shadow-sm xl:col-span-2">
          <div className="mb-5">
            <h3 className="text-lg font-semibold text-slate-900">
              Distribuição por departamento
            </h3>

            <p className="mt-1 text-sm text-slate-500">
              Funcionários ativos por departamento.
            </p>
          </div>

          {departmentDistribution.length ===
          0 ? (
            <p className="text-sm text-slate-500">
              Não existem dados de departamentos para apresentar.
            </p>
          ) : (
            <div className="space-y-5">
              {departmentDistribution.map(
                (department) => (
                  <div
                    key={department.name}
                  >
                    <div className="mb-2 flex items-center justify-between gap-4">
                      <span className="text-sm font-medium text-slate-700">
                        {department.name}
                      </span>

                      <span className="text-sm font-semibold text-slate-900">
                        {department.count}
                      </span>
                    </div>

                    <div className="h-2 overflow-hidden rounded-full bg-slate-100">
                      <div
                        className="h-full rounded-full bg-blue-600"
                        style={{
                          width: `${Math.max(
                            4,
                            (department.count /
                              maxDepartmentCount) *
                              100
                          )}%`,
                        }}
                      />
                    </div>
                  </div>
                )
              )}
            </div>
          )}
        </section>

        <section className="rounded-xl bg-white p-6 shadow-sm">
          <h3 className="text-lg font-semibold text-slate-900">
            Ações rápidas
          </h3>

          <p className="mt-1 text-sm text-slate-500">
            Acesso às operações mais utilizadas.
          </p>

          <div className="mt-5 space-y-3">
            <QuickAction
              label="+ Novo funcionário"
              description="Registar um novo colaborador"
              onClick={() =>
                navigate(
                  "/employees/new"
                )
              }
              primary
            />

            <QuickAction
              label="Ver funcionários"
              description="Consultar a lista de colaboradores"
              onClick={() =>
                navigate("/employees")
              }
            />

            <QuickAction
              label="Departamentos"
              description="Gerir a estrutura departamental"
              onClick={() =>
                navigate(
                  "/departments"
                )
              }
            />

            <QuickAction
              label="Tipos de documentos"
              description="Configurar documentos e validades"
              onClick={() =>
                navigate(
                  "/settings/document-types"
                )
              }
            />
          </div>
        </section>
      </div>
    </div>
  );
}

interface DashboardCardProps {
  label: string;
  value: number;
  detail: string;
  tone?: "default" | "warning" | "danger";
  onClick?: () => void;
}

function DashboardCard({
  label,
  value,
  detail,
  tone = "default",
  onClick,
}: DashboardCardProps) {
  const valueClass =
    tone === "danger"
      ? "text-red-600"
      : tone === "warning"
        ? "text-amber-600"
        : "text-slate-900";

  return (
    <button
      type="button"
      onClick={onClick}
      disabled={!onClick}
      className={`rounded-xl bg-white p-5 text-left shadow-sm transition ${
        onClick
          ? "hover:-translate-y-0.5 hover:shadow-md"
          : "cursor-default"
      }`}
    >
      <p className="text-xs font-semibold uppercase tracking-wide text-slate-400">
        {label}
      </p>

      <p
        className={`mt-2 text-3xl font-bold ${valueClass}`}
      >
        {value}
      </p>

      <p className="mt-1 text-sm text-slate-500">
        {detail}
      </p>
    </button>
  );
}

interface TableContentProps {
  children: React.ReactNode;
}

function TableHeader({
  children,
}: TableContentProps) {
  return (
    <th className="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
      {children}
    </th>
  );
}

function TableCell({
  children,
}: TableContentProps) {
  return (
    <td className="whitespace-nowrap px-6 py-4 text-sm text-slate-600">
      {children}
    </td>
  );
}

interface QuickActionProps {
  label: string;
  description: string;
  onClick: () => void;
  primary?: boolean;
}

function QuickAction({
  label,
  description,
  onClick,
  primary = false,
}: QuickActionProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`w-full rounded-lg border p-4 text-left transition ${
        primary
          ? "border-blue-200 bg-blue-50 hover:bg-blue-100"
          : "border-slate-200 hover:border-slate-300 hover:bg-slate-50"
      }`}
    >
      <p
        className={`text-sm font-semibold ${
          primary
            ? "text-blue-700"
            : "text-slate-800"
        }`}
      >
        {label}
      </p>

      <p className="mt-1 text-xs text-slate-500">
        {description}
      </p>
    </button>
  );
}
