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

interface AbsenceType {
  id: string;
  name: string;
  color: string;
}

interface Absence {
  id: string;
  employeeId: string;
  employeeName: string;
  absenceTypeId: string;
  absenceTypeName: string;
  absenceTypeColor: string;
  startDateTime: string;
  endDateTime: string;
  status: string;
  reason?: string | null;
  notes?: string | null;
}

interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

interface AbsenceForm {
  employeeId: string;
  absenceTypeId: string;
  startDateTime: string;
  endDateTime: string;
  status: string;
  reason: string;
  notes: string;
}

const emptyForm: AbsenceForm = {
  employeeId: "",
  absenceTypeId: "",
  startDateTime: "",
  endDateTime: "",
  status: "Pending",
  reason: "",
  notes: "",
};

const filterStatusOptions = [
  { value: "", label: "Todos os estados" },
  { value: "Pending", label: "Pendente" },
  { value: "Approved", label: "Aprovada" },
  { value: "Rejected", label: "Rejeitada" },
  { value: "Cancelled", label: "Cancelada" },
];

const editStatusOptions = [
  { value: "1", label: "Pendente" },
  { value: "2", label: "Aprovada" },
  { value: "3", label: "Rejeitada" },
  { value: "4", label: "Cancelada" },
];

export default function Absences() {
  const [absences, setAbsences] = useState<Absence[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [absenceTypes, setAbsenceTypes] = useState<AbsenceType[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [statusAction, setStatusAction] = useState<{
    id: string;
    action: "approve" | "reject";
  } | null>(null);
  const [error, setError] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<AbsenceForm>(emptyForm);

  const [employeeId, setEmployeeId] = useState("");
  const [departmentId, setDepartmentId] = useState("");
  const [absenceTypeId, setAbsenceTypeId] = useState("");
  const [status, setStatus] = useState("");
  const [dateFrom, setDateFrom] = useState("");
  const [dateTo, setDateTo] = useState("");
  const [page, setPage] = useState(1);
  const pageSize = 20;
  const [totalCount, setTotalCount] = useState(0);
  const [viewMode, setViewMode] = useState<"list" | "month" | "year">("list");
  const [calendarMonth, setCalendarMonth] = useState(
    () => new Date(new Date().getFullYear(), new Date().getMonth(), 1)
  );
  const [calendarAbsences, setCalendarAbsences] = useState<Absence[]>([]);
  const [calendarLoading, setCalendarLoading] = useState(false);

  useEffect(() => {
    void loadReferenceData();
  }, []);

  useEffect(() => {
    void loadAbsences();
  }, [page]);

  useEffect(() => {
    if (viewMode !== "list") {
      void loadCalendarAbsences(calendarMonth);
    }
  }, [viewMode, calendarMonth]);

  async function loadReferenceData() {
    try {
      const [employeesResponse, departmentsResponse, typesResponse] =
        await Promise.all([
          api.get<Employee[]>("/Employees"),
          api.get<Department[]>("/Departments"),
          api.get<AbsenceType[]>("/AbsenceTypes"),
        ]);

      setEmployees(employeesResponse.data);
      setDepartments(departmentsResponse.data);
      setAbsenceTypes(typesResponse.data);
    } catch (error) {
      console.error("Erro ao carregar dados auxiliares:", error);
    }
  }

  async function loadAbsences(targetPage = page) {
    try {
      setLoading(true);
      setError("");

      const params: Record<string, string | number> = {
        page: targetPage,
        pageSize,
      };

      if (employeeId) params.employeeId = employeeId;
      if (departmentId) params.departmentId = departmentId;
      if (absenceTypeId) params.absenceTypeId = absenceTypeId;
      if (status) params.status = status;
      if (dateFrom) params.dateFrom = `${dateFrom}T00:00:00Z`;
      if (dateTo) params.dateTo = `${dateTo}T23:59:59Z`;

      const response = await api.get<PagedResult<Absence>>(
        "/Absences/search",
        { params }
      );

      setAbsences(response.data.items);
      setTotalCount(response.data.totalCount);
    } catch (error: any) {
      console.error("Erro ao carregar ausências:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar as ausências."
      );
    } finally {
      setLoading(false);
    }
  }

  async function loadCalendarAbsences(month: Date) {
    try {
      setCalendarLoading(true);
      setError("");

      const yearStart = new Date(month.getFullYear(), 0, 1);
      const yearEnd = new Date(
        month.getFullYear(),
        11,
        31,
        23,
        59,
        59,
        999
      );

      const params: Record<string, string | number> = {
        page: 1,
        pageSize: 10000,
        dateFrom: yearStart.toISOString(),
        dateTo: yearEnd.toISOString(),
      };

      if (employeeId) params.employeeId = employeeId;
      if (departmentId) params.departmentId = departmentId;
      if (absenceTypeId) params.absenceTypeId = absenceTypeId;
      if (status) params.status = status;

      const response = await api.get<PagedResult<Absence>>(
        "/Absences/search",
        { params }
      );

      setCalendarAbsences(response.data.items);
    } catch (error: any) {
      console.error("Erro ao carregar calendário de ausências:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar o calendário de ausências."
      );
    } finally {
      setCalendarLoading(false);
    }
  }

  function applyFilters() {
    if (viewMode !== "list") {
      void loadCalendarAbsences(calendarMonth);
      return;
    }

    if (page !== 1) {
      setPage(1);
    } else {
      void loadAbsences(1);
    }
  }

  function clearFilters() {
    setEmployeeId("");
    setDepartmentId("");
    setAbsenceTypeId("");
    setStatus("");
    setDateFrom("");
    setDateTo("");
    setPage(1);

    setTimeout(() => {
      if (viewMode !== "list") {
        void loadCalendarAbsences(calendarMonth);
      } else {
        void loadAbsences(1);
      }
    }, 0);
  }

  function openCreate() {
    setEditingId(null);
    setForm(emptyForm);
    setError("");
    setShowForm(true);
  }

  function openEdit(absence: Absence) {
    setEditingId(absence.id);
    setForm({
      employeeId: absence.employeeId,
      absenceTypeId: absence.absenceTypeId,
      startDateTime: toLocalInputValue(absence.startDateTime),
      endDateTime: toLocalInputValue(absence.endDateTime),
      status: getStatusValue(absence.status),
      reason: absence.reason ?? "",
      notes: absence.notes ?? "",
    });
    setError("");
    setShowForm(true);
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  function closeForm() {
    setEditingId(null);
    setForm(emptyForm);
    setShowForm(false);
    setError("");
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form.employeeId || !form.absenceTypeId) {
      setError("Funcionário e tipo de ausência são obrigatórios.");
      return;
    }

    if (!form.startDateTime || !form.endDateTime) {
      setError("A data/hora de início e fim são obrigatórias.");
      return;
    }

    const start = new Date(form.startDateTime);
    const end = new Date(form.endDateTime);

    if (end <= start) {
      setError("A data/hora de fim tem de ser posterior ao início.");
      return;
    }

    const commonPayload = {
      employeeId: form.employeeId,
      absenceTypeId: form.absenceTypeId,
      startDateTime: start.toISOString(),
      endDateTime: end.toISOString(),
      reason: form.reason.trim() || null,
      notes: form.notes.trim() || null,
    };

    try {
      setSaving(true);
      setError("");

      if (editingId) {
        await api.put(`/Absences/${editingId}`, {
          id: editingId,
          ...commonPayload,
          status: Number(form.status),
        });
      } else {
        await api.post("/Absences", commonPayload);
      }

      closeForm();
      await loadAbsences(page);
      if (viewMode !== "list") {
        await loadCalendarAbsences(calendarMonth);
      }
    } catch (error: any) {
      console.error("Erro ao guardar ausência:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível guardar a ausência."
      );
    } finally {
      setSaving(false);
    }
  }

  async function changePendingStatus(
    absence: Absence,
    action: "approve" | "reject"
  ) {
    const actionLabel = action === "approve" ? "aprovar" : "rejeitar";

    if (
      !window.confirm(
        `Tem a certeza de que pretende ${actionLabel} a ausência de ${absence.employeeName}?`
      )
    ) {
      return;
    }

    try {
      setStatusAction({ id: absence.id, action });
      setError("");

      await api.put(`/Absences/${absence.id}/${action}`);

      await loadAbsences(page);
      if (viewMode !== "list") {
        await loadCalendarAbsences(calendarMonth);
      }
    } catch (error: any) {
      console.error(`Erro ao ${actionLabel} ausência:`, error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          `Não foi possível ${actionLabel} a ausência.`
      );
    } finally {
      setStatusAction(null);
    }
  }

  async function deleteAbsence(absence: Absence) {
    if (
      !window.confirm(
        `Tem a certeza de que pretende apagar a ausência de ${absence.employeeName}?`
      )
    ) {
      return;
    }

    try {
      setDeletingId(absence.id);
      setError("");
      await api.delete(`/Absences/${absence.id}`);
      await loadAbsences(page);
      if (viewMode !== "list") {
        await loadCalendarAbsences(calendarMonth);
      }
    } catch (error: any) {
      console.error("Erro ao apagar ausência:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível apagar a ausência."
      );
    } finally {
      setDeletingId(null);
    }
  }

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-3xl font-bold text-slate-900">Ausências</h2>
          <p className="mt-1 text-sm text-slate-500">
            Gestão de férias, faltas e outras ausências dos funcionários.
          </p>
        </div>

        <button
          type="button"
          onClick={showForm ? closeForm : openCreate}
          className="self-start rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
        >
          {showForm ? "Cancelar" : "+ Nova ausência"}
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
            {editingId ? "Editar ausência" : "Nova ausência"}
          </h3>

          <form onSubmit={handleSubmit} className="mt-5 space-y-5">
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
                  <option value="">Selecionar funcionário</option>
                  {employees.map((employee) => (
                    <option key={employee.id} value={employee.id}>
                      {employee.employeeNumber} — {employee.firstName}{" "}
                      {employee.lastName}
                    </option>
                  ))}
                </select>
              </Field>

              <Field label="Tipo de ausência *">
                <select
                  value={form.absenceTypeId}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      absenceTypeId: event.target.value,
                    }))
                  }
                  required
                  className={inputClass}
                >
                  <option value="">Selecionar tipo</option>
                  {absenceTypes.map((type) => (
                    <option key={type.id} value={type.id}>
                      {type.name}
                    </option>
                  ))}
                </select>
              </Field>

              <Field label="Início *">
                <input
                  type="datetime-local"
                  value={form.startDateTime}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      startDateTime: event.target.value,
                    }))
                  }
                  required
                  className={inputClass}
                />
              </Field>

              <Field label="Fim *">
                <input
                  type="datetime-local"
                  value={form.endDateTime}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      endDateTime: event.target.value,
                    }))
                  }
                  required
                  className={inputClass}
                />
              </Field>

              {editingId && (
                <Field label="Estado *">
                  <select
                    value={form.status}
                    onChange={(event) =>
                      setForm((current) => ({
                        ...current,
                        status: event.target.value,
                      }))
                    }
                    className={inputClass}
                  >
                    {editStatusOptions.map((item) => (
                      <option key={item.value} value={item.value}>
                        {item.label}
                      </option>
                    ))}
                  </select>
                </Field>
              )}

              <Field label="Motivo" className="md:col-span-2">
                <textarea
                  value={form.reason}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      reason: event.target.value,
                    }))
                  }
                  maxLength={500}
                  rows={2}
                  className={inputClass}
                />
              </Field>

              <Field label="Notas" className="md:col-span-2">
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
                className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
              >
                {saving
                  ? "A guardar..."
                  : editingId
                    ? "Guardar alterações"
                    : "Registar ausência"}
              </button>
            </div>
          </form>
        </section>
      )}

      <div className="flex items-center justify-between gap-4">
        <div className="inline-flex rounded-lg border border-slate-200 bg-white p-1 shadow-sm">
          {(["list", "month", "year"] as const).map((mode) => (
            <button
              key={mode}
              type="button"
              onClick={() => setViewMode(mode)}
              className={`rounded-md px-4 py-2 text-sm font-medium transition ${
                viewMode === mode
                  ? "bg-slate-900 text-white"
                  : "text-slate-600 hover:bg-slate-50"
              }`}
            >
              {mode === "list" ? "Lista" : mode === "month" ? "Mês" : "Ano"}
            </button>
          ))}
        </div>
      </div>

      <section className="rounded-xl bg-white p-5 shadow-sm">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-6">
          <Field label="Funcionário">
            <select
              value={employeeId}
              onChange={(e) => setEmployeeId(e.target.value)}
              className={inputClass}
            >
              <option value="">Todos</option>
              {employees.map((employee) => (
                <option key={employee.id} value={employee.id}>
                  {employee.firstName} {employee.lastName}
                </option>
              ))}
            </select>
          </Field>

          <Field label="Departamento">
            <select
              value={departmentId}
              onChange={(e) => setDepartmentId(e.target.value)}
              className={inputClass}
            >
              <option value="">Todos</option>
              {departments.map((department) => (
                <option key={department.id} value={department.id}>
                  {department.name}
                </option>
              ))}
            </select>
          </Field>

          <Field label="Tipo">
            <select
              value={absenceTypeId}
              onChange={(e) => setAbsenceTypeId(e.target.value)}
              className={inputClass}
            >
              <option value="">Todos</option>
              {absenceTypes.map((type) => (
                <option key={type.id} value={type.id}>
                  {type.name}
                </option>
              ))}
            </select>
          </Field>

          <Field label="Estado">
            <select
              value={status}
              onChange={(e) => setStatus(e.target.value)}
              className={inputClass}
            >
              {filterStatusOptions.map((item) => (
                <option key={item.value} value={item.value}>
                  {item.label}
                </option>
              ))}
            </select>
          </Field>

          <Field label="De">
            <input
              type="date"
              value={dateFrom}
              onChange={(e) => setDateFrom(e.target.value)}
              className={inputClass}
            />
          </Field>

          <Field label="Até">
            <input
              type="date"
              value={dateTo}
              onChange={(e) => setDateTo(e.target.value)}
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

      {viewMode === "list" && (
        <section className="overflow-hidden rounded-xl bg-white shadow-sm">
          {loading ? (
            <div className="p-8 text-center text-slate-500">
              A carregar ausências...
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
                        Tipo
                      </th>
                      <th className="px-6 py-4 font-semibold text-slate-600">
                        Início
                      </th>
                      <th className="px-6 py-4 font-semibold text-slate-600">
                        Fim
                      </th>
                      <th className="px-6 py-4 font-semibold text-slate-600">
                        Estado
                      </th>
                      <th className="px-6 py-4 text-right font-semibold text-slate-600">
                        Ações
                      </th>
                    </tr>
                  </thead>
  
                  <tbody className="divide-y">
                    {absences.map((absence) => (
                      <tr key={absence.id} className="hover:bg-slate-50">
                        <td className="px-6 py-4 font-medium text-slate-900">
                          {absence.employeeName}
                        </td>
                        <td className="px-6 py-4 text-slate-600">
                          {absence.absenceTypeName}
                        </td>
                        <td className="px-6 py-4 text-slate-600">
                          {formatDateTime(absence.startDateTime)}
                        </td>
                        <td className="px-6 py-4 text-slate-600">
                          {formatDateTime(absence.endDateTime)}
                        </td>
                        <td className="px-6 py-4">
                          <StatusBadge status={absence.status} />
                        </td>
                        <td className="px-6 py-4">
                          <div className="flex flex-wrap justify-end gap-3">
                            {absence.status === "Pending" && (
                              <>
                                <button
                                  type="button"
                                  disabled={
                                    statusAction?.id === absence.id ||
                                    deletingId === absence.id
                                  }
                                  onClick={() =>
                                    void changePendingStatus(absence, "approve")
                                  }
                                  className="font-medium text-green-600 hover:text-green-700 disabled:cursor-not-allowed disabled:opacity-50"
                                >
                                  {statusAction?.id === absence.id &&
                                  statusAction.action === "approve"
                                    ? "A aprovar..."
                                    : "Aprovar"}
                                </button>
  
                                <button
                                  type="button"
                                  disabled={
                                    statusAction?.id === absence.id ||
                                    deletingId === absence.id
                                  }
                                  onClick={() =>
                                    void changePendingStatus(absence, "reject")
                                  }
                                  className="font-medium text-amber-600 hover:text-amber-700 disabled:cursor-not-allowed disabled:opacity-50"
                                >
                                  {statusAction?.id === absence.id &&
                                  statusAction.action === "reject"
                                    ? "A rejeitar..."
                                    : "Rejeitar"}
                                </button>
                              </>
                            )}
  
                            <button
                              type="button"
                              disabled={
                                statusAction?.id === absence.id ||
                                deletingId === absence.id
                              }
                              onClick={() => openEdit(absence)}
                              className="font-medium text-blue-600 hover:text-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
                            >
                              Editar
                            </button>
  
                            <button
                              type="button"
                              disabled={
                                deletingId === absence.id ||
                                statusAction?.id === absence.id
                              }
                              onClick={() => void deleteAbsence(absence)}
                              className="font-medium text-red-600 hover:text-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                            >
                              {deletingId === absence.id ? "A apagar..." : "Apagar"}
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
  
                    {absences.length === 0 && (
                      <tr>
                        <td
                          colSpan={6}
                          className="px-6 py-12 text-center text-slate-500"
                        >
                          Não existem ausências para os filtros selecionados.
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
  
              <div className="flex items-center justify-between border-t px-6 py-4">
                <p className="text-sm text-slate-500">
                  {totalCount} {totalCount === 1 ? "registo" : "registos"}
                </p>
  
                <div className="flex items-center gap-3">
                  <button
                    type="button"
                    disabled={page <= 1}
                    onClick={() => setPage((current) => current - 1)}
                    className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-40"
                  >
                    Anterior
                  </button>
                  <span className="text-sm text-slate-600">
                    Página {page} de {totalPages}
                  </span>
                  <button
                    type="button"
                    disabled={page >= totalPages}
                    onClick={() => setPage((current) => current + 1)}
                    className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-40"
                  >
                    Seguinte
                  </button>
                </div>
              </div>
            </>
          )}
        </section>
      )}

      {viewMode === "month" && (
        <CalendarView
          month={calendarMonth}
          absences={calendarAbsences}
          absenceTypes={absenceTypes}
          loading={calendarLoading}
          onPreviousMonth={() =>
            setCalendarMonth(
              (current) =>
                new Date(current.getFullYear(), current.getMonth() - 1, 1)
            )
          }
          onNextMonth={() =>
            setCalendarMonth(
              (current) =>
                new Date(current.getFullYear(), current.getMonth() + 1, 1)
            )
          }
          onToday={() => {
            const today = new Date();
            setCalendarMonth(
              new Date(today.getFullYear(), today.getMonth(), 1)
            );
          }}
          onEdit={openEdit}
        />
      )}

      {viewMode === "year" && (
        <YearCalendarView
          year={calendarMonth.getFullYear()}
          absences={calendarAbsences}
          absenceTypes={absenceTypes}
          loading={calendarLoading}
          onPreviousYear={() =>
            setCalendarMonth(
              (current) =>
                new Date(
                  current.getFullYear() - 1,
                  current.getMonth(),
                  1
                )
            )
          }
          onNextYear={() =>
            setCalendarMonth(
              (current) =>
                new Date(
                  current.getFullYear() + 1,
                  current.getMonth(),
                  1
                )
            )
          }
          onToday={() => {
            const today = new Date();
            setCalendarMonth(
              new Date(
                today.getFullYear(),
                today.getMonth(),
                1
              )
            );
          }}
          onOpenMonth={(monthIndex) => {
            setCalendarMonth(
              new Date(
                calendarMonth.getFullYear(),
                monthIndex,
                1
              )
            );
            setViewMode("month");
          }}
          onEdit={openEdit}
        />
      )}
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
      <span className="text-sm font-medium text-slate-700">{label}</span>
      {children}
    </label>
  );
}

function CalendarView({
  month,
  absences,
  absenceTypes,
  loading,
  onPreviousMonth,
  onNextMonth,
  onToday,
  onEdit,
}: {
  month: Date;
  absences: Absence[];
  absenceTypes: AbsenceType[];
  loading: boolean;
  onPreviousMonth: () => void;
  onNextMonth: () => void;
  onToday: () => void;
  onEdit: (absence: Absence) => void;
}) {
  const days = buildCalendarDays(month);
  const weekDays = ["Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Dom"];
  const monthAbsences = absences.filter((absence) =>
    absenceTouchesMonth(absence, month)
  );

  return (
    <section className="overflow-hidden rounded-xl bg-white shadow-sm">
      <CalendarHeader
        title={month.toLocaleDateString("pt-PT", {
          month: "long",
          year: "numeric",
        })}
        onPrevious={onPreviousMonth}
        onNext={onNextMonth}
        onToday={onToday}
      />

      {loading ? (
        <CalendarLoading />
      ) : (
        <div className="overflow-x-auto">
          <div className="min-w-[900px]">
            <div className="grid grid-cols-7 border-b border-slate-200 bg-slate-50">
              {weekDays.map((day) => (
                <div
                  key={day}
                  className="px-3 py-3 text-center text-xs font-semibold uppercase tracking-wide text-slate-500"
                >
                  {day}
                </div>
              ))}
            </div>

            <div className="grid grid-cols-7">
              {days.map((day) => {
                const dayAbsences = monthAbsences.filter(
                  (absence) =>
                    absenceTouchesDay(absence, day.date)
                );

                const today = isSameDay(
                  day.date,
                  new Date()
                );

                return (
                  <div
                    key={day.date.toISOString()}
                    className={`min-h-32 border-b border-r border-slate-200 p-2 ${
                      day.inCurrentMonth
                        ? "bg-white"
                        : "bg-slate-50"
                    }`}
                  >
                    <div className="mb-2 flex justify-end">
                      <span
                        className={`flex h-7 w-7 items-center justify-center rounded-full text-xs font-medium ${
                          today
                            ? "bg-blue-600 text-white"
                            : day.inCurrentMonth
                              ? "text-slate-700"
                              : "text-slate-400"
                        }`}
                      >
                        {day.date.getDate()}
                      </span>
                    </div>

                    <div className="space-y-1.5">
                      {dayAbsences
                        .slice(0, 4)
                        .map((absence) => (
                          <button
                            key={absence.id}
                            type="button"
                            onClick={() =>
                              onEdit(absence)
                            }
                            title={`${absence.employeeName} — ${absence.absenceTypeName} — ${statusLabel(absence.status)}`}
                            className="block w-full truncate rounded-md border px-2 py-1.5 text-left text-xs font-medium transition hover:brightness-95"
                            style={absenceColorStyle(
                              absence.absenceTypeColor
                            )}
                          >
                            <span className="block truncate">
                              {absence.employeeName}
                            </span>

                            <span className="block truncate text-[11px] font-normal opacity-80">
                              {absence.absenceTypeName}
                            </span>
                          </button>
                        ))}

                      {dayAbsences.length > 4 && (
                        <div className="px-1 text-xs font-medium text-slate-500">
                          + {dayAbsences.length - 4} mais
                        </div>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        </div>
      )}

      <TypeLegend
        absenceTypes={absenceTypes}
        absences={monthAbsences}
      />
    </section>
  );
}

function YearCalendarView({
  year,
  absences,
  absenceTypes,
  loading,
  onPreviousYear,
  onNextYear,
  onToday,
  onOpenMonth,
  onEdit,
}: {
  year: number;
  absences: Absence[];
  absenceTypes: AbsenceType[];
  loading: boolean;
  onPreviousYear: () => void;
  onNextYear: () => void;
  onToday: () => void;
  onOpenMonth: (monthIndex: number) => void;
  onEdit: (absence: Absence) => void;
}) {
  return (
    <section className="overflow-hidden rounded-xl bg-white shadow-sm">
      <CalendarHeader
        title={String(year)}
        onPrevious={onPreviousYear}
        onNext={onNextYear}
        onToday={onToday}
      />

      {loading ? (
        <CalendarLoading />
      ) : (
        <div className="grid grid-cols-1 gap-4 p-4 md:grid-cols-2 xl:grid-cols-3">
          {Array.from(
            { length: 12 },
            (_, monthIndex) => (
              <YearMonth
                key={monthIndex}
                year={year}
                monthIndex={monthIndex}
                absences={absences}
                onOpen={() =>
                  onOpenMonth(monthIndex)
                }
                onEdit={onEdit}
              />
            )
          )}
        </div>
      )}

      <TypeLegend
        absenceTypes={absenceTypes}
        absences={absences}
      />
    </section>
  );
}

function YearMonth({
  year,
  monthIndex,
  absences,
  onOpen,
  onEdit,
}: {
  year: number;
  monthIndex: number;
  absences: Absence[];
  onOpen: () => void;
  onEdit: (absence: Absence) => void;
}) {
  const month = new Date(
    year,
    monthIndex,
    1
  );

  const days =
    buildCompactMonthDays(month);

  const weekDays = [
    "S",
    "T",
    "Q",
    "Q",
    "S",
    "S",
    "D",
  ];

  return (
    <div className="rounded-xl border border-slate-200 p-3">
      <button
        type="button"
        onClick={onOpen}
        className="mb-3 w-full text-left text-sm font-semibold capitalize text-slate-800 hover:text-blue-600"
      >
        {month.toLocaleDateString(
          "pt-PT",
          { month: "long" }
        )}
      </button>

      <div className="grid grid-cols-7 gap-1">
        {weekDays.map((day, index) => (
          <div
            key={`${day}-${index}`}
            className="pb-1 text-center text-[10px] font-semibold text-slate-400"
          >
            {day}
          </div>
        ))}

        {days.map((item, index) => {
          if (!item) {
            return (
              <div
                key={`empty-${index}`}
                className="h-9"
              />
            );
          }

          const date = new Date(
            year,
            monthIndex,
            item
          );

          const dayAbsences =
            absences.filter((absence) =>
              absenceTouchesDay(
                absence,
                date
              )
            );

          const today = isSameDay(
            date,
            new Date()
          );

          return (
            <div
              key={item}
              className={`relative flex h-9 items-center justify-center rounded-md text-xs ${
                today
                  ? "ring-2 ring-blue-500"
                  : ""
              }`}
            >
              <span>{item}</span>

              {dayAbsences.length > 0 && (
                <div className="absolute bottom-0.5 left-1/2 flex max-w-[90%] -translate-x-1/2 gap-0.5">
                  {dayAbsences
                    .slice(0, 3)
                    .map((absence) => (
                      <button
                        key={absence.id}
                        type="button"
                        onClick={(event) => {
                          event.stopPropagation();
                          onEdit(absence);
                        }}
                        title={`${absence.employeeName} — ${absence.absenceTypeName} — ${statusLabel(absence.status)}`}
                        className="h-1.5 w-1.5 rounded-full"
                        style={{
                          backgroundColor:
                            validHex(
                              absence.absenceTypeColor
                            ),
                        }}
                      />
                    ))}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}

function CalendarHeader({
  title,
  onPrevious,
  onNext,
  onToday,
}: {
  title: string;
  onPrevious: () => void;
  onNext: () => void;
  onToday: () => void;
}) {
  return (
    <div className="flex flex-col gap-4 border-b border-slate-100 p-5 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h3 className="text-lg font-semibold capitalize text-slate-900">
          {title}
        </h3>

        <p className="mt-1 text-sm text-slate-500">
          As cores representam o tipo de ausência.
        </p>
      </div>

      <div className="flex items-center gap-2">
        <button
          type="button"
          onClick={onPrevious}
          className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          ←
        </button>

        <button
          type="button"
          onClick={onToday}
          className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          Hoje
        </button>

        <button
          type="button"
          onClick={onNext}
          className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
        >
          →
        </button>
      </div>
    </div>
  );
}

function CalendarLoading() {
  return (
    <div className="p-12 text-center text-sm text-slate-500">
      A carregar calendário...
    </div>
  );
}

function TypeLegend({
  absenceTypes,
  absences,
}: {
  absenceTypes: AbsenceType[];
  absences: Absence[];
}) {
  const usedIds = new Set(
    absences.map(
      (absence) => absence.absenceTypeId
    )
  );

  const types = absenceTypes.filter(
    (type) => usedIds.has(type.id)
  );

  if (types.length === 0) {
    return null;
  }

  return (
    <div className="flex flex-wrap gap-3 border-t border-slate-100 px-5 py-4">
      {types.map((type) => (
        <span
          key={type.id}
          className="inline-flex items-center gap-2 text-xs font-medium text-slate-600"
        >
          <span
            className="h-3 w-3 rounded-full"
            style={{
              backgroundColor:
                validHex(type.color),
            }}
          />

          {type.name}
        </span>
      ))}
    </div>
  );
}

function buildCalendarDays(
  month: Date
) {
  const firstDay = new Date(
    month.getFullYear(),
    month.getMonth(),
    1
  );

  const mondayOffset =
    (firstDay.getDay() + 6) % 7;

  const start = new Date(
    firstDay.getFullYear(),
    firstDay.getMonth(),
    firstDay.getDate() -
      mondayOffset
  );

  return Array.from(
    { length: 42 },
    (_, index) => {
      const date = new Date(
        start.getFullYear(),
        start.getMonth(),
        start.getDate() + index
      );

      return {
        date,
        inCurrentMonth:
          date.getMonth() ===
          month.getMonth(),
      };
    }
  );
}

function buildCompactMonthDays(
  month: Date
): Array<number | null> {
  const first = new Date(
    month.getFullYear(),
    month.getMonth(),
    1
  );

  const offset =
    (first.getDay() + 6) % 7;

  const count = new Date(
    month.getFullYear(),
    month.getMonth() + 1,
    0
  ).getDate();

  return [
    ...Array.from(
      { length: offset },
      () => null
    ),
    ...Array.from(
      { length: count },
      (_, index) => index + 1
    ),
  ];
}

function absenceTouchesMonth(
  absence: Absence,
  month: Date
) {
  const start = new Date(
    month.getFullYear(),
    month.getMonth(),
    1
  );

  const end = new Date(
    month.getFullYear(),
    month.getMonth() + 1,
    0,
    23,
    59,
    59,
    999
  );

  return (
    new Date(
      absence.startDateTime
    ) <= end &&
    new Date(
      absence.endDateTime
    ) >= start
  );
}

function absenceTouchesDay(
  absence: Absence,
  day: Date
) {
  const dayStart = new Date(
    day.getFullYear(),
    day.getMonth(),
    day.getDate(),
    0,
    0,
    0,
    0
  );

  const dayEnd = new Date(
    day.getFullYear(),
    day.getMonth(),
    day.getDate(),
    23,
    59,
    59,
    999
  );

  return (
    new Date(
      absence.startDateTime
    ) <= dayEnd &&
    new Date(
      absence.endDateTime
    ) >= dayStart
  );
}

function isSameDay(
  a: Date,
  b: Date
) {
  return (
    a.getFullYear() ===
      b.getFullYear() &&
    a.getMonth() ===
      b.getMonth() &&
    a.getDate() ===
      b.getDate()
  );
}

function validHex(
  color?: string
) {
  return color &&
    /^#[0-9A-Fa-f]{6}$/.test(
      color
    )
    ? color
    : "#3B82F6";
}

function absenceColorStyle(
  color?: string
): React.CSSProperties {
  const hex = validHex(color);

  return {
    backgroundColor: `${hex}20`,
    borderColor: `${hex}55`,
    color: hex,
  };
}

function statusLabel(
  status: string
) {
  const labels: Record<
    string,
    string
  > = {
    Pending: "Pendente",
    Approved: "Aprovada",
    Rejected: "Rejeitada",
    Cancelled: "Cancelada",
  };

  return labels[status] ?? status;
}

function getStatusValue(status: string) {
  switch (status) {
    case "Pending":
      return "1";
    case "Approved":
      return "2";
    case "Rejected":
      return "3";
    case "Cancelled":
      return "4";
    default:
      return "1";
  }
}

function StatusBadge({ status }: { status: string }) {
  const info: Record<string, { label: string; className: string }> = {
    Pending: {
      label: "Pendente",
      className: "bg-amber-100 text-amber-700",
    },
    Approved: {
      label: "Aprovada",
      className: "bg-green-100 text-green-700",
    },
    Rejected: {
      label: "Rejeitada",
      className: "bg-red-100 text-red-700",
    },
    Cancelled: {
      label: "Cancelada",
      className: "bg-slate-100 text-slate-600",
    },
  };

  const current = info[status] ?? {
    label: status,
    className: "bg-slate-100 text-slate-600",
  };

  return (
    <span
      className={`inline-flex rounded-full px-3 py-1 text-xs font-medium ${current.className}`}
    >
      {current.label}
    </span>
  );
}

function formatDateTime(value: string) {
  return new Date(value).toLocaleString("pt-PT", {
    dateStyle: "short",
    timeStyle: "short",
  });
}

function toLocalInputValue(value: string) {
  const date = new Date(value);
  const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return local.toISOString().slice(0, 16);
}
