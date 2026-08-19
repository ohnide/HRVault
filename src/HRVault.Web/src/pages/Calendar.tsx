import { useEffect, useMemo, useState } from "react";
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
}

interface VacationRequest {
  id: string;
  employeeId: string;
  employeeName: string;
  startDate: string;
  endDate: string;
  days: number;
  status: string;
}

interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

type CalendarSource = "all" | "absence" | "vacation";
type ViewMode = "month" | "year";

interface CalendarEvent {
  id: string;
  source: "absence" | "vacation";
  employeeId: string;
  employeeName: string;
  title: string;
  color: string;
  start: string;
  end: string;
  status: string;
}

const vacationColor = "#2563EB";

const statusOptions = [
  { value: "", label: "Todos os estados" },
  { value: "Pending", label: "Pendente" },
  { value: "Approved", label: "Aprovado" },
  { value: "Rejected", label: "Rejeitado" },
  { value: "Cancelled", label: "Cancelado" },
];

export default function Calendar() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [absenceTypes, setAbsenceTypes] = useState<AbsenceType[]>([]);

  const [absences, setAbsences] = useState<Absence[]>([]);
  const [vacations, setVacations] = useState<VacationRequest[]>([]);

  const [employeeId, setEmployeeId] = useState("");
  const [departmentId, setDepartmentId] = useState("");
  const [source, setSource] = useState<CalendarSource>("all");
  const [absenceTypeId, setAbsenceTypeId] = useState("");
  const [status, setStatus] = useState("");

  const [viewMode, setViewMode] = useState<ViewMode>("month");
  const [calendarMonth, setCalendarMonth] = useState(
    () => new Date(new Date().getFullYear(), new Date().getMonth(), 1)
  );

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);

  useEffect(() => {
    void loadReferenceData();
  }, []);

  useEffect(() => {
    void loadCalendarData();
  }, [calendarMonth]);

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

  async function loadCalendarData() {
    try {
      setLoading(true);
      setError("");

      const year = calendarMonth.getFullYear();

      const yearStart = new Date(Date.UTC(year, 0, 1, 0, 0, 0));
      const yearEnd = new Date(Date.UTC(year, 11, 31, 23, 59, 59));

      const absenceParams: Record<string, string | number> = {
        page: 1,
        pageSize: 10000,
        dateFrom: yearStart.toISOString(),
        dateTo: yearEnd.toISOString(),
      };

      if (employeeId) absenceParams.employeeId = employeeId;
      if (departmentId) absenceParams.departmentId = departmentId;
      if (absenceTypeId) absenceParams.absenceTypeId = absenceTypeId;
      if (status) absenceParams.status = status;

      const vacationParams: Record<string, string | number> = {
        page: 1,
        pageSize: 10000,
        year,
      };

      if (employeeId) vacationParams.employeeId = employeeId;
      if (departmentId) vacationParams.departmentId = departmentId;
      if (status) vacationParams.status = status;

      const absenceRequest =
        source === "vacation"
          ? Promise.resolve<PagedResult<Absence>>({
              items: [],
              totalCount: 0,
              page: 1,
              pageSize: 10000,
            })
          : api
              .get<PagedResult<Absence>>("/Absences/search", {
                params: absenceParams,
              })
              .then((response) => response.data);

      const vacationRequest =
        source === "absence"
          ? Promise.resolve<PagedResult<VacationRequest>>({
              items: [],
              totalCount: 0,
              page: 1,
              pageSize: 10000,
            })
          : api
              .get<PagedResult<VacationRequest>>(
                "/VacationRequests/search",
                { params: vacationParams }
              )
              .then((response) => response.data);

      const [absenceData, vacationData] = await Promise.all([
        absenceRequest,
        vacationRequest,
      ]);

      setAbsences(absenceData.items);
      setVacations(vacationData.items);
    } catch (error: any) {
      console.error("Erro ao carregar calendário:", error);

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar o calendário."
      );
    } finally {
      setLoading(false);
    }
  }

  function applyFilters() {
    void loadCalendarData();
  }

  function clearFilters() {
    setEmployeeId("");
    setDepartmentId("");
    setSource("all");
    setAbsenceTypeId("");
    setStatus("");

    setTimeout(() => {
      void loadCalendarData();
    }, 0);
  }

  const events = useMemo<CalendarEvent[]>(() => {
    const absenceEvents = absences.map((absence) => ({
      id: absence.id,
      source: "absence" as const,
      employeeId: absence.employeeId,
      employeeName: absence.employeeName,
      title: absence.absenceTypeName,
      color: validHex(absence.absenceTypeColor),
      start: absence.startDateTime,
      end: absence.endDateTime,
      status: absence.status,
    }));

    const vacationEvents = vacations.map((vacation) => ({
      id: vacation.id,
      source: "vacation" as const,
      employeeId: vacation.employeeId,
      employeeName: vacation.employeeName,
      title: "Férias",
      color: vacationColor,
      start: vacation.startDate,
      end: vacation.endDate,
      status: vacation.status,
    }));

    return [...absenceEvents, ...vacationEvents];
  }, [absences, vacations]);

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-3xl font-bold text-slate-900">
            Calendário
          </h2>
          <p className="mt-1 text-sm text-slate-500">
            Planeamento conjunto de ausências e férias dos funcionários.
          </p>
        </div>

        <div className="inline-flex self-start rounded-lg border border-slate-200 bg-white p-1 shadow-sm">
          {(["month", "year"] as const).map((mode) => (
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
              {mode === "month" ? "Mês" : "Ano"}
            </button>
          ))}
        </div>
      </div>

      {error && (
        <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
          {error}
        </div>
      )}

      <section className="rounded-xl bg-white p-5 shadow-sm">
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-5">
          <Field label="Funcionário">
            <select
              value={employeeId}
              onChange={(event) => setEmployeeId(event.target.value)}
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
              onChange={(event) => setDepartmentId(event.target.value)}
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

          <Field label="Origem">
            <select
              value={source}
              onChange={(event) => {
                const value = event.target.value as CalendarSource;
                setSource(value);

                if (value === "vacation") {
                  setAbsenceTypeId("");
                }
              }}
              className={inputClass}
            >
              <option value="all">Todos</option>
              <option value="absence">Ausências</option>
              <option value="vacation">Férias</option>
            </select>
          </Field>

          <Field label="Tipo">
            <select
              value={absenceTypeId}
              onChange={(event) =>
                setAbsenceTypeId(event.target.value)
              }
              disabled={source === "vacation"}
              className={`${inputClass} disabled:cursor-not-allowed disabled:bg-slate-100 disabled:text-slate-400`}
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
              onChange={(event) => setStatus(event.target.value)}
              className={inputClass}
            >
              {statusOptions.map((item) => (
                <option key={item.value} value={item.value}>
                  {item.label}
                </option>
              ))}
            </select>
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

      {viewMode === "month" ? (
        <MonthCalendar
          month={calendarMonth}
          events={events}
          loading={loading}
          onEventClick={setSelectedEvent}
          onPrevious={() =>
            setCalendarMonth(
              (current) =>
                new Date(
                  current.getFullYear(),
                  current.getMonth() - 1,
                  1
                )
            )
          }
          onNext={() =>
            setCalendarMonth(
              (current) =>
                new Date(
                  current.getFullYear(),
                  current.getMonth() + 1,
                  1
                )
            )
          }
          onToday={() => {
            const today = new Date();
            setCalendarMonth(
              new Date(today.getFullYear(), today.getMonth(), 1)
            );
          }}
        />
      ) : (
        <YearCalendar
          year={calendarMonth.getFullYear()}
          events={events}
          loading={loading}
          onEventClick={setSelectedEvent}
          onPrevious={() =>
            setCalendarMonth(
              (current) =>
                new Date(
                  current.getFullYear() - 1,
                  current.getMonth(),
                  1
                )
            )
          }
          onNext={() =>
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
              new Date(today.getFullYear(), today.getMonth(), 1)
            );
          }}
          onOpenMonth={(monthIndex) => {
            setCalendarMonth(
              new Date(calendarMonth.getFullYear(), monthIndex, 1)
            );
            setViewMode("month");
          }}
        />
      )}

      {selectedEvent && (
        <EventDetailsModal
          event={selectedEvent}
          onClose={() => setSelectedEvent(null)}
        />
      )}

      <Legend absenceTypes={absenceTypes} />
    </div>
  );
}

function MonthCalendar({
  month,
  events,
  loading,
  onEventClick,
  onPrevious,
  onNext,
  onToday,
}: {
  month: Date;
  events: CalendarEvent[];
  loading: boolean;
  onEventClick: (event: CalendarEvent) => void;
  onPrevious: () => void;
  onNext: () => void;
  onToday: () => void;
}) {
  const days = buildCalendarDays(month);
  const weekDays = ["Seg", "Ter", "Qua", "Qui", "Sex", "Sáb", "Dom"];
  const monthEvents = events.filter((event) =>
    eventTouchesMonth(event, month)
  );

  return (
    <section className="overflow-hidden rounded-xl bg-white shadow-sm">
      <CalendarHeader
        title={month.toLocaleDateString("pt-PT", {
          month: "long",
          year: "numeric",
        })}
        onPrevious={onPrevious}
        onNext={onNext}
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
                const dayEvents = monthEvents.filter((event) =>
                  eventTouchesDay(event, day.date)
                );

                const today = isSameDay(day.date, new Date());

                return (
                  <div
                    key={day.date.toISOString()}
                    className={`min-h-32 border-b border-r border-slate-200 p-2 ${
                      day.inCurrentMonth ? "bg-white" : "bg-slate-50"
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
                      {dayEvents.slice(0, 4).map((event) => (
                        <button
                          type="button"
                          key={`${event.source}-${event.id}`}
                          onClick={() => onEventClick(event)}
                          title={`${event.employeeName} — ${event.title} — ${sourceLabel(event.source)} — ${statusLabel(event.status)}`}
                          className="block w-full truncate rounded-md border px-2 py-1.5 text-left text-xs font-medium hover:brightness-95"
                          style={eventColorStyle(event.color)}
                        >
                          <span className="block truncate">
                            {event.employeeName}
                          </span>
                          <span className="block truncate text-[11px] font-normal opacity-80">
                            {event.title}
                          </span>
                        </button>
                      ))}

                      {dayEvents.length > 4 && (
                        <div className="px-1 text-xs font-medium text-slate-500">
                          + {dayEvents.length - 4} mais
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
    </section>
  );
}

function YearCalendar({
  year,
  events,
  loading,
  onEventClick,
  onPrevious,
  onNext,
  onToday,
  onOpenMonth,
}: {
  year: number;
  events: CalendarEvent[];
  loading: boolean;
  onEventClick: (event: CalendarEvent) => void;
  onPrevious: () => void;
  onNext: () => void;
  onToday: () => void;
  onOpenMonth: (monthIndex: number) => void;
}) {
  return (
    <section className="overflow-hidden rounded-xl bg-white shadow-sm">
      <CalendarHeader
        title={String(year)}
        onPrevious={onPrevious}
        onNext={onNext}
        onToday={onToday}
      />

      {loading ? (
        <CalendarLoading />
      ) : (
        <div className="grid grid-cols-1 gap-4 p-4 md:grid-cols-2 xl:grid-cols-3">
          {Array.from({ length: 12 }, (_, monthIndex) => (
            <YearMonth
              key={monthIndex}
              year={year}
              monthIndex={monthIndex}
              events={events}
              onEventClick={onEventClick}
              onOpen={() => onOpenMonth(monthIndex)}
            />
          ))}
        </div>
      )}
    </section>
  );
}

function YearMonth({
  year,
  monthIndex,
  events,
  onEventClick,
  onOpen,
}: {
  year: number;
  monthIndex: number;
  events: CalendarEvent[];
  onEventClick: (event: CalendarEvent) => void;
  onOpen: () => void;
}) {
  const month = new Date(year, monthIndex, 1);
  const days = buildCompactMonthDays(month);
  const weekDays = ["S", "T", "Q", "Q", "S", "S", "D"];

  return (
    <div className="rounded-xl border border-slate-200 p-3">
      <button
        type="button"
        onClick={onOpen}
        className="mb-3 w-full text-left text-sm font-semibold capitalize text-slate-800 hover:text-blue-600"
      >
        {month.toLocaleDateString("pt-PT", { month: "long" })}
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
            return <div key={`empty-${index}`} className="h-9" />;
          }

          const date = new Date(year, monthIndex, item);

          const dayEvents = events.filter((event) =>
            eventTouchesDay(event, date)
          );

          const today = isSameDay(date, new Date());

          return (
            <div
              key={item}
              className={`relative flex h-9 items-center justify-center rounded-md text-xs ${
                today ? "ring-2 ring-blue-500" : ""
              }`}
            >
              <span>{item}</span>

              {dayEvents.length > 0 && (
                <div className="absolute bottom-0.5 left-1/2 flex max-w-[90%] -translate-x-1/2 gap-0.5">
                  {dayEvents.slice(0, 3).map((event) => (
                    <button
                      type="button"
                      key={`${event.source}-${event.id}`}
                      onClick={() => onEventClick(event)}
                      title={`${event.employeeName} — ${event.title}`}
                      className="h-2 w-2 rounded-full"
                      style={{ backgroundColor: event.color }}
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

function EventDetailsModal({ event, onClose }: { event: CalendarEvent; onClose: () => void }) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4" onMouseDown={onClose}>
      <div className="w-full max-w-lg rounded-2xl bg-white shadow-2xl" onMouseDown={(e) => e.stopPropagation()}>
        <div className="flex items-start justify-between border-b px-6 py-5">
          <div>
            <p className="text-sm text-slate-500">{sourceLabel(event.source)}</p>
            <h3 className="mt-1 text-xl font-bold text-slate-900">{event.employeeName}</h3>
          </div>
          <button type="button" onClick={onClose} className="rounded-lg px-3 py-2 hover:bg-slate-100">✕</button>
        </div>
        <div className="space-y-4 px-6 py-5">
          <div className="flex items-center gap-3">
            <span className="h-4 w-4 rounded-full" style={{ backgroundColor: event.color }} />
            <strong className="text-slate-800">{event.title}</strong>
          </div>
          <div className="grid grid-cols-2 gap-4">
            <Detail label="Início" value={formatEventDate(event.start)} />
            <Detail label="Fim" value={formatEventDate(event.end)} />
            <Detail label="Estado" value={statusLabel(event.status)} />
            <Detail label="Origem" value={sourceLabel(event.source)} />
          </div>
        </div>
        <div className="flex justify-end border-t px-6 py-4">
          <button type="button" onClick={onClose} className="rounded-lg bg-slate-800 px-4 py-2 text-sm font-semibold text-white">Fechar</button>
        </div>
      </div>
    </div>
  );
}

function Detail({ label, value }: { label: string; value: string }) {
  return <div><p className="text-xs font-medium uppercase text-slate-400">{label}</p><p className="mt-1 text-sm font-semibold text-slate-800">{value}</p></div>;
}

function formatEventDate(value: string) {
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? value : date.toLocaleString("pt-PT", { day: "2-digit", month: "2-digit", year: "numeric", hour: "2-digit", minute: "2-digit" });
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
          Ausências e férias no mesmo calendário.
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

function Legend({
  absenceTypes,
}: {
  absenceTypes: AbsenceType[];
}) {
  return (
    <section className="rounded-xl bg-white p-5 shadow-sm">
      <div className="flex flex-wrap gap-4">
        <LegendItem
          label="Férias"
          color={vacationColor}
        />

        {absenceTypes.map((type) => (
          <LegendItem
            key={type.id}
            label={type.name}
            color={validHex(type.color)}
          />
        ))}
      </div>
    </section>
  );
}

function LegendItem({
  label,
  color,
}: {
  label: string;
  color: string;
}) {
  return (
    <span className="inline-flex items-center gap-2 text-xs font-medium text-slate-600">
      <span
        className="h-3 w-3 rounded-full"
        style={{ backgroundColor: color }}
      />
      {label}
    </span>
  );
}

function CalendarLoading() {
  return (
    <div className="p-12 text-center text-sm text-slate-500">
      A carregar calendário...
    </div>
  );
}

function buildCalendarDays(month: Date) {
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
    firstDay.getDate() - mondayOffset
  );

  return Array.from({ length: 42 }, (_, index) => {
    const date = new Date(
      start.getFullYear(),
      start.getMonth(),
      start.getDate() + index
    );

    return {
      date,
      inCurrentMonth:
        date.getMonth() === month.getMonth(),
    };
  });
}

function buildCompactMonthDays(
  month: Date
): Array<number | null> {
  const first = new Date(
    month.getFullYear(),
    month.getMonth(),
    1
  );

  const offset = (first.getDay() + 6) % 7;

  const count = new Date(
    month.getFullYear(),
    month.getMonth() + 1,
    0
  ).getDate();

  return [
    ...Array.from({ length: offset }, () => null),
    ...Array.from(
      { length: count },
      (_, index) => index + 1
    ),
  ];
}

function eventTouchesMonth(
  event: CalendarEvent,
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
    new Date(event.start) <= end &&
    new Date(event.end) >= start
  );
}

function eventTouchesDay(
  event: CalendarEvent,
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
    new Date(event.start) <= dayEnd &&
    new Date(event.end) >= dayStart
  );
}

function isSameDay(a: Date, b: Date) {
  return (
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate()
  );
}

function validHex(color?: string) {
  return color && /^#[0-9A-Fa-f]{6}$/.test(color)
    ? color
    : "#3B82F6";
}

function eventColorStyle(
  color: string
): React.CSSProperties {
  return {
    backgroundColor: `${color}20`,
    borderColor: `${color}55`,
    color,
  };
}

function sourceLabel(
  source: CalendarEvent["source"]
) {
  return source === "vacation"
    ? "Férias"
    : "Ausência";
}

function statusLabel(status: string) {
  const labels: Record<string, string> = {
    Pending: "Pendente",
    Approved: "Aprovado",
    Rejected: "Rejeitado",
    Cancelled: "Cancelado",
  };

  return labels[status] ?? status;
}

const inputClass =
  "mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500";

function Field({
  label,
  children,
}: {
  label: string;
  children: React.ReactNode;
}) {
  return (
    <label className="block">
      <span className="text-sm font-medium text-slate-700">
        {label}
      </span>
      {children}
    </label>
  );
}
