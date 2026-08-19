import { useEffect, useState } from "react";
import { api } from "../api/client";

interface Employee {
  id: string;
  employeeNumber: string;
  firstName: string;
  lastName: string;
}

interface Department {
  id: string;
  name: string;
}

interface VacationRequest {
  id: string;
  employeeId: string;
  employeeName: string;
  startDate: string;
  endDate: string;
  days: number;
  status: string;
  notes?: string | null;
  approvedAt?: string | null;
  approvedBy?: string | null;
}

interface VacationBalance {
  id: string;
  employeeId: string;
  year: number;
  entitledDays: number;
  carriedOverDays: number;
  adjustmentDays: number;
  totalDays: number;
  approvedDays: number;
  remainingDays: number;
  notes?: string | null;
}

interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

interface VacationForm {
  employeeId: string;
  startDate: string;
  endDate: string;
  notes: string;
}

const emptyForm: VacationForm = {
  employeeId: "",
  startDate: "",
  endDate: "",
  notes: "",
};

const statusOptions = [
  { value: "", label: "Todos os estados" },
  { value: "Pending", label: "Pendente" },
  { value: "Approved", label: "Aprovado" },
  { value: "Rejected", label: "Rejeitado" },
  { value: "Cancelled", label: "Cancelado" },
];

export default function Vacations() {
  const [requests, setRequests] = useState<VacationRequest[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [statusAction, setStatusAction] = useState<{
    id: string;
    action: "approve" | "reject";
  } | null>(null);

  const [error, setError] = useState("");

  const [employeeId, setEmployeeId] = useState("");
  const [departmentId, setDepartmentId] = useState("");
  const [status, setStatus] = useState("");
  const [year, setYear] = useState(new Date().getFullYear());

  const [page, setPage] = useState(1);
  const pageSize = 20;
  const [totalCount, setTotalCount] = useState(0);

  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<VacationForm>(emptyForm);

  const [balanceEmployeeId, setBalanceEmployeeId] = useState("");
  const [balanceYear, setBalanceYear] = useState(
    new Date().getFullYear()
  );
  const [balance, setBalance] = useState<VacationBalance | null>(null);
  const [balanceLoading, setBalanceLoading] = useState(false);
  const [balanceMessage, setBalanceMessage] = useState("");

  useEffect(() => {
    void loadReferenceData();
  }, []);

  useEffect(() => {
    void loadRequests();
  }, [page]);

  useEffect(() => {
    void loadBalance();
  }, [balanceEmployeeId, balanceYear]);

  async function loadReferenceData() {
    try {
      const [employeesResponse, departmentsResponse] =
        await Promise.all([
          api.get<Employee[]>("/Employees"),
          api.get<Department[]>("/Departments"),
        ]);

      setEmployees(employeesResponse.data);
      setDepartments(departmentsResponse.data);
    } catch (error) {
      console.error("Erro ao carregar dados auxiliares:", error);
    }
  }

  async function loadRequests(targetPage = page) {
    try {
      setLoading(true);
      setError("");

      const params: Record<string, string | number> = {
        page: targetPage,
        pageSize,
        year,
      };

      if (employeeId) {
        params.employeeId = employeeId;
      }

      if (departmentId) {
        params.departmentId = departmentId;
      }

      if (status) {
        params.status = status;
      }

      const response =
        await api.get<PagedResult<VacationRequest>>(
          "/VacationRequests/search",
          { params }
        );

      setRequests(response.data.items);
      setTotalCount(response.data.totalCount);
    } catch (error: any) {
      console.error("Erro ao carregar férias:", error);

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar os pedidos de férias."
      );
    } finally {
      setLoading(false);
    }
  }

  async function loadBalance() {
    if (!balanceEmployeeId) {
      setBalance(null);
      setBalanceMessage(
        "Selecione um funcionário para consultar o saldo."
      );
      return;
    }

    try {
      setBalanceLoading(true);
      setBalanceMessage("");

      const response = await api.get<VacationBalance>(
        `/VacationBalances/${balanceEmployeeId}/${balanceYear}`
      );

      setBalance(response.data);
    } catch (error: any) {
      setBalance(null);

      if (error.response?.status === 404) {
        setBalanceMessage(
          `Ainda não existe saldo de férias definido para ${balanceYear}.`
        );
      } else {
        console.error("Erro ao carregar saldo de férias:", error);
        setBalanceMessage(
          "Não foi possível carregar o saldo de férias."
        );
      }
    } finally {
      setBalanceLoading(false);
    }
  }

  function openCreateForm() {
    setForm({
      ...emptyForm,
      employeeId: employeeId || balanceEmployeeId,
    });
    setError("");
    setShowForm(true);
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  function closeForm() {
    setForm(emptyForm);
    setShowForm(false);
    setError("");
  }

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!form.employeeId) {
      setError("O funcionário é obrigatório.");
      return;
    }

    if (!form.startDate || !form.endDate) {
      setError("As datas de início e fim são obrigatórias.");
      return;
    }

    if (form.endDate < form.startDate) {
      setError(
        "A data de fim tem de ser igual ou posterior à data de início."
      );
      return;
    }

    try {
      setSaving(true);
      setError("");

      await api.post("/VacationRequests", {
        employeeId: form.employeeId,
        startDate: `${form.startDate}T00:00:00Z`,
        endDate: `${form.endDate}T00:00:00Z`,
        notes: form.notes.trim() || null,
      });

      const createdEmployeeId = form.employeeId;

      closeForm();

      if (page !== 1) {
        setPage(1);
      } else {
        await loadRequests(1);
      }

      if (balanceEmployeeId === createdEmployeeId) {
        await loadBalance();
      }
    } catch (error: any) {
      console.error("Erro ao criar pedido de férias:", error);

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível criar o pedido de férias."
      );
    } finally {
      setSaving(false);
    }
  }

  async function changePendingStatus(
    request: VacationRequest,
    action: "approve" | "reject"
  ) {
    const actionText =
      action === "approve" ? "aprovar" : "rejeitar";

    const confirmed = window.confirm(
      `Tem a certeza de que pretende ${actionText} o pedido de férias de ${request.employeeName}?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setStatusAction({
        id: request.id,
        action,
      });
      setError("");

      await api.put(
        `/VacationRequests/${request.id}/${action}`
      );

      await loadRequests(page);

      if (balanceEmployeeId === request.employeeId) {
        await loadBalance();
      }
    } catch (error: any) {
      console.error(
        `Erro ao ${actionText} pedido de férias:`,
        error
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          `Não foi possível ${actionText} o pedido de férias.`
      );
    } finally {
      setStatusAction(null);
    }
  }

  function applyFilters() {
    if (page !== 1) {
      setPage(1);
    } else {
      void loadRequests(1);
    }
  }

  function clearFilters() {
    setEmployeeId("");
    setDepartmentId("");
    setStatus("");
    setYear(new Date().getFullYear());
    setPage(1);

    setTimeout(() => {
      void loadRequests(1);
    }, 0);
  }

  function employeeLabel(employee: Employee) {
    const name =
      `${employee.firstName} ${employee.lastName}`.trim();

    return employee.employeeNumber
      ? `${employee.employeeNumber} - ${name}`
      : name;
  }

  const totalPages = Math.max(
    1,
    Math.ceil(totalCount / pageSize)
  );

  const balanceCards = [
    {
      label: "Direito",
      value: balance?.entitledDays ?? 0,
    },
    {
      label: "Transitado",
      value: balance?.carriedOverDays ?? 0,
    },
    {
      label: "Ajustes",
      value: balance?.adjustmentDays ?? 0,
    },
    {
      label: "Aprovados",
      value: balance?.approvedDays ?? 0,
    },
    {
      label: "Disponíveis",
      value: balance?.remainingDays ?? 0,
    },
  ];

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-3xl font-bold text-slate-900">
            Férias
          </h2>

          <p className="mt-1 text-sm text-slate-500">
            Gestão de saldos e pedidos de férias dos funcionários.
          </p>
        </div>

        <button
          type="button"
          onClick={showForm ? closeForm : openCreateForm}
          className="self-start rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
        >
          {showForm ? "Cancelar" : "+ Novo pedido"}
        </button>
      </div>

      {error && (
        <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
          {error}
        </div>
      )}

      {showForm && (
        <section className="rounded-xl bg-white p-6 shadow-sm">
          <h3 className="text-lg font-semibold text-slate-900">
            Novo pedido de férias
          </h3>

          <form
            onSubmit={handleSubmit}
            className="mt-5 space-y-5"
          >
            <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
              <Field label="Funcionário *">
                <select
                  value={form.employeeId}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      employeeId: event.target.value,
                    }))
                  }
                  required
                  className={inputClass}
                >
                  <option value="">
                    Selecionar funcionário
                  </option>

                  {employees.map((employee) => (
                    <option
                      key={employee.id}
                      value={employee.id}
                    >
                      {employeeLabel(employee)}
                    </option>
                  ))}
                </select>
              </Field>

              <div />

              <Field label="Data de início *">
                <input
                  type="date"
                  value={form.startDate}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      startDate: event.target.value,
                    }))
                  }
                  required
                  className={inputClass}
                />
              </Field>

              <Field label="Data de fim *">
                <input
                  type="date"
                  value={form.endDate}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      endDate: event.target.value,
                    }))
                  }
                  required
                  className={inputClass}
                />
              </Field>

              <Field
                label="Notas"
                className="md:col-span-2"
              >
                <textarea
                  value={form.notes}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      notes: event.target.value,
                    }))
                  }
                  maxLength={1000}
                  rows={3}
                  className={inputClass}
                />
              </Field>
            </div>

            <div className="flex justify-end gap-3 border-t border-slate-100 pt-5">
              <button
                type="button"
                onClick={closeForm}
                className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
              >
                Cancelar
              </button>

              <button
                type="submit"
                disabled={saving}
                className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
              >
                {saving
                  ? "A guardar..."
                  : "Criar pedido"}
              </button>
            </div>
          </form>
        </section>
      )}

      <section className="rounded-xl bg-white p-6 shadow-sm">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <h3 className="text-lg font-semibold text-slate-900">
              Saldo anual
            </h3>

            <p className="mt-1 text-sm text-slate-500">
              Consulte o saldo de férias de um funcionário por ano.
            </p>
          </div>

          <div className="grid gap-3 sm:grid-cols-2 lg:w-[620px]">
            <Field label="Funcionário">
              <select
                value={balanceEmployeeId}
                onChange={(event) =>
                  setBalanceEmployeeId(event.target.value)
                }
                className={inputClass}
              >
                <option value="">
                  Selecionar funcionário
                </option>

                {employees.map((employee) => (
                  <option
                    key={employee.id}
                    value={employee.id}
                  >
                    {employeeLabel(employee)}
                  </option>
                ))}
              </select>
            </Field>

            <Field label="Ano">
              <input
                type="number"
                min={2000}
                max={2100}
                value={balanceYear}
                onChange={(event) =>
                  setBalanceYear(
                    Number(event.target.value) ||
                      new Date().getFullYear()
                  )
                }
                className={inputClass}
              />
            </Field>
          </div>
        </div>

        <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
          {balanceCards.map((card) => (
            <div
              key={card.label}
              className="rounded-xl border border-slate-200 bg-slate-50 p-4"
            >
              <p className="text-sm font-medium text-slate-500">
                {card.label}
              </p>

              <p className="mt-2 text-2xl font-bold text-slate-900">
                {balanceLoading
                  ? "..."
                  : formatDays(card.value)}
              </p>

              <p className="mt-1 text-xs text-slate-400">
                dias
              </p>
            </div>
          ))}
        </div>

        {!balanceLoading && balanceMessage && (
          <div className="mt-4 rounded-lg border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-600">
            {balanceMessage}
          </div>
        )}

        {balance && (
          <div className="mt-4 flex flex-wrap gap-x-6 gap-y-2 text-sm text-slate-500">
            <span>
              Total atribuído:{" "}
              <strong className="font-semibold text-slate-700">
                {formatDays(balance.totalDays)} dias
              </strong>
            </span>

            {balance.notes && (
              <span>
                Observações:{" "}
                <strong className="font-semibold text-slate-700">
                  {balance.notes}
                </strong>
              </span>
            )}
          </div>
        )}
      </section>

      <section className="rounded-xl bg-white p-5 shadow-sm">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-4">
          <Field label="Funcionário">
            <select
              value={employeeId}
              onChange={(event) =>
                setEmployeeId(event.target.value)
              }
              className={inputClass}
            >
              <option value="">Todos</option>

              {employees.map((employee) => (
                <option
                  key={employee.id}
                  value={employee.id}
                >
                  {employee.firstName}{" "}
                  {employee.lastName}
                </option>
              ))}
            </select>
          </Field>

          <Field label="Departamento">
            <select
              value={departmentId}
              onChange={(event) =>
                setDepartmentId(event.target.value)
              }
              className={inputClass}
            >
              <option value="">Todos</option>

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

          <Field label="Estado">
            <select
              value={status}
              onChange={(event) =>
                setStatus(event.target.value)
              }
              className={inputClass}
            >
              {statusOptions.map((option) => (
                <option
                  key={option.value}
                  value={option.value}
                >
                  {option.label}
                </option>
              ))}
            </select>
          </Field>

          <Field label="Ano">
            <input
              type="number"
              min={2000}
              max={2100}
              value={year}
              onChange={(event) =>
                setYear(
                  Number(event.target.value) ||
                    new Date().getFullYear()
                )
              }
              className={inputClass}
            />
          </Field>
        </div>

        <div className="mt-4 flex justify-end gap-3">
          <button
            type="button"
            onClick={clearFilters}
            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Limpar
          </button>

          <button
            type="button"
            onClick={applyFilters}
            className="rounded-lg bg-slate-800 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-900"
          >
            Aplicar filtros
          </button>
        </div>
      </section>

      <section className="overflow-hidden rounded-xl bg-white shadow-sm">
        {loading ? (
          <div className="p-8 text-center text-slate-500">
            A carregar pedidos de férias...
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full text-left text-sm">
                <thead className="border-b bg-slate-50">
                  <tr>
                    <th className="px-6 py-4 font-semibold text-slate-600">
                      Funcionário
                    </th>

                    <th className="px-6 py-4 font-semibold text-slate-600">
                      Início
                    </th>

                    <th className="px-6 py-4 font-semibold text-slate-600">
                      Fim
                    </th>

                    <th className="px-6 py-4 font-semibold text-slate-600">
                      Dias
                    </th>

                    <th className="px-6 py-4 font-semibold text-slate-600">
                      Estado
                    </th>

                    <th className="px-6 py-4 font-semibold text-slate-600">
                      Notas
                    </th>

                    <th className="px-6 py-4 text-right font-semibold text-slate-600">
                      Ações
                    </th>
                  </tr>
                </thead>

                <tbody className="divide-y">
                  {requests.map((request) => (
                    <tr
                      key={request.id}
                      className="hover:bg-slate-50"
                    >
                      <td className="px-6 py-4 font-medium text-slate-900">
                        {request.employeeName}
                      </td>

                      <td className="px-6 py-4 text-slate-600">
                        {formatDate(request.startDate)}
                      </td>

                      <td className="px-6 py-4 text-slate-600">
                        {formatDate(request.endDate)}
                      </td>

                      <td className="px-6 py-4 text-slate-600">
                        {formatDays(request.days)}
                      </td>

                      <td className="px-6 py-4">
                        <StatusBadge
                          status={request.status}
                        />
                      </td>

                      <td className="max-w-xs px-6 py-4 text-slate-600">
                        <span
                          className="block truncate"
                          title={request.notes ?? ""}
                        >
                          {request.notes || "-"}
                        </span>
                      </td>

                      <td className="px-6 py-4">
                        <div className="flex flex-wrap justify-end gap-3">
                          {request.status === "Pending" && (
                            <>
                              <button
                                type="button"
                                disabled={
                                  statusAction?.id ===
                                  request.id
                                }
                                onClick={() =>
                                  void changePendingStatus(
                                    request,
                                    "approve"
                                  )
                                }
                                className="font-medium text-green-600 hover:text-green-700 disabled:cursor-not-allowed disabled:opacity-50"
                              >
                                {statusAction?.id ===
                                  request.id &&
                                statusAction.action ===
                                  "approve"
                                  ? "A aprovar..."
                                  : "Aprovar"}
                              </button>

                              <button
                                type="button"
                                disabled={
                                  statusAction?.id ===
                                  request.id
                                }
                                onClick={() =>
                                  void changePendingStatus(
                                    request,
                                    "reject"
                                  )
                                }
                                className="font-medium text-amber-600 hover:text-amber-700 disabled:cursor-not-allowed disabled:opacity-50"
                              >
                                {statusAction?.id ===
                                  request.id &&
                                statusAction.action ===
                                  "reject"
                                  ? "A rejeitar..."
                                  : "Rejeitar"}
                              </button>
                            </>
                          )}

                          {request.status !== "Pending" && (
                            <span className="text-xs text-slate-400">
                              Sem ações
                            </span>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}

                  {requests.length === 0 && (
                    <tr>
                      <td
                        colSpan={7}
                        className="px-6 py-12 text-center text-slate-500"
                      >
                        Não existem pedidos de férias para os filtros selecionados.
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>

            <div className="flex flex-col gap-3 border-t px-6 py-4 sm:flex-row sm:items-center sm:justify-between">
              <p className="text-sm text-slate-500">
                {totalCount}{" "}
                {totalCount === 1
                  ? "pedido"
                  : "pedidos"}
              </p>

              <div className="flex items-center gap-3">
                <button
                  type="button"
                  disabled={page <= 1}
                  onClick={() =>
                    setPage((current) => current - 1)
                  }
                  className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
                >
                  Anterior
                </button>

                <span className="text-sm text-slate-600">
                  Página {page} de {totalPages}
                </span>

                <button
                  type="button"
                  disabled={page >= totalPages}
                  onClick={() =>
                    setPage((current) => current + 1)
                  }
                  className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-40"
                >
                  Seguinte
                </button>
              </div>
            </div>
          </>
        )}
      </section>
    </div>
  );
}

const inputClass =
  "mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500";

function Field({
  label,
  children,
  className = "",
}: {
  label: string;
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <label className={`block ${className}`}>
      <span className="text-sm font-medium text-slate-700">
        {label}
      </span>

      {children}
    </label>
  );
}

function StatusBadge({
  status,
}: {
  status: string;
}) {
  const info: Record<
    string,
    {
      label: string;
      className: string;
    }
  > = {
    Pending: {
      label: "Pendente",
      className:
        "bg-amber-100 text-amber-700",
    },
    Approved: {
      label: "Aprovado",
      className:
        "bg-green-100 text-green-700",
    },
    Rejected: {
      label: "Rejeitado",
      className:
        "bg-red-100 text-red-700",
    },
    Cancelled: {
      label: "Cancelado",
      className:
        "bg-slate-100 text-slate-600",
    },
  };

  const current = info[status] ?? {
    label: status,
    className:
      "bg-slate-100 text-slate-600",
  };

  return (
    <span
      className={`inline-flex rounded-full px-3 py-1 text-xs font-medium ${current.className}`}
    >
      {current.label}
    </span>
  );
}

function formatDate(value: string) {
  return new Date(value).toLocaleDateString(
    "pt-PT"
  );
}

function formatDays(value: number) {
  return Number.isInteger(value)
    ? value.toString()
    : value.toFixed(2).replace(".", ",");
}
