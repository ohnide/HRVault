import { useEffect, useMemo, useState } from "react";
import { api } from "../api/client";

interface Employee {
  id: string;
  employeeNumber?: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  status?: string | number;
}

interface TimePunch {
  id: string;
  employeeId: string;
  employeeName: string;
  timestampUtc: string;
  source: number;
  sourceName: string;
  direction: number;
  directionName: string;
  attendanceDeviceId?: string | null;
  isVoided: boolean;
  voidReason?: string | null;
  adjustmentReason?: string | null;
  createdAt: string;
}

interface AttendanceAlert {
  type: string;
  message: string;
}

interface AttendanceDay {
  employeeId: string;
  date: string;

  workScheduleId?: string | null;
  workScheduleName?: string | null;
  workScheduleType?: string | null;

  status: string;

  expectedTime: string;
  workedTime: string;
  breakTime: string;
  balance: string;

  lateTime: string;
  earlyLeaveTime: string;
  overtime: string;

  firstEntryUtc?: string | null;
  lastExitUtc?: string | null;

  alerts: AttendanceAlert[];
}

interface AttendanceWeekDay {
  date: string;

  workScheduleId?: string | null;
  workScheduleName?: string | null;
  workScheduleType?: string | null;

  status: string;

  expectedTime: string;
  workedTime: string;
  breakTime: string;
  balance: string;

  lateTime: string;
  earlyLeaveTime: string;
  overtime: string;

  hasWorked: boolean;

  alerts: AttendanceAlert[];
}

interface AttendanceWeek {
  employeeId: string;

  weekStart: string;
  weekEnd: string;

  status: string;

  requiredWorkingDays: number;
  workedDays: number;
  missingWorkingDays: number;

  expectedTime: string;
  workedTime: string;
  breakTime: string;
  balance: string;
  overtime: string;

  alerts: AttendanceAlert[];

  days: AttendanceWeekDay[];
}

type PunchDirection = 1 | 2;
type ViewMode = "today" | "day" | "week";

export default function TimePunches() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [punches, setPunches] = useState<TimePunch[]>([]);
  const [dayPunches, setDayPunches] = useState<TimePunch[]>([]);

  const [employeeId, setEmployeeId] = useState("");

  const [viewMode, setViewMode] =
    useState<ViewMode>("today");

  const [selectedDate, setSelectedDate] =
    useState(todayInputValue());

  const [dayResult, setDayResult] =
    useState<AttendanceDay | null>(null);

  const [weekResult, setWeekResult] =
    useState<AttendanceWeek | null>(null);

  const [loading, setLoading] = useState(true);
  const [analysisLoading, setAnalysisLoading] =
    useState(false);

  const [punching, setPunching] =
    useState<PunchDirection | null>(null);

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [showManualForm, setShowManualForm] =
    useState(false);

  const [manualDirection, setManualDirection] =
    useState<PunchDirection>(1);

  const [manualTime, setManualTime] =
    useState("08:00");

  const [manualReason, setManualReason] =
    useState("");

  const [manualSaving, setManualSaving] =
    useState(false);

  useEffect(() => {
    void loadInitialData();
  }, []);

  useEffect(() => {
    if (viewMode === "day") {
      setWeekResult(null);

      if (employeeId && selectedDate) {
        void loadDay();
      } else {
        setDayResult(null);
        setDayPunches([]);
      }
    }

    if (viewMode === "week") {
      setDayResult(null);
      setDayPunches([]);

      if (employeeId && selectedDate) {
        void loadWeek();
      } else {
        setWeekResult(null);
      }
    }
  }, [viewMode, employeeId, selectedDate]);

  async function loadInitialData() {
    try {
      setLoading(true);
      setError("");

      const [employeesResponse, punchesResponse] =
        await Promise.all([
          api.get<Employee[]>("/Employees"),
          api.get<TimePunch[]>("/TimePunches/today"),
        ]);

      setEmployees(employeesResponse.data);
      setPunches(punchesResponse.data);
    } catch (error: any) {
      console.error("Erro ao carregar ponto:", error);

      setError(
        apiError(
          error,
          "Não foi possível carregar os dados de ponto."
        )
      );
    } finally {
      setLoading(false);
    }
  }

  async function loadTodayPunches() {
    try {
      setError("");

      const response =
        await api.get<TimePunch[]>(
          "/TimePunches/today"
        );

      setPunches(response.data);
    } catch (error: any) {
      console.error(
        "Erro ao carregar picagens de hoje:",
        error
      );

      setError(
        apiError(
          error,
          "Não foi possível atualizar as picagens."
        )
      );
    }
  }

  async function loadDay() {
    if (!employeeId || !selectedDate) {
      return;
    }

    try {
      setAnalysisLoading(true);
      setError("");
      setSuccess("");

      const fromUtc =
        new Date(
          `${selectedDate}T00:00:00`
        ).toISOString();

      const toUtc =
        new Date(
          `${selectedDate}T23:59:59`
        ).toISOString();

      const [summaryResponse, punchesResponse] =
        await Promise.all([
          api.get<AttendanceDay>(
            `/Attendance/employee/${employeeId}/day/${selectedDate}`
          ),
          api.get<TimePunch[]>(
            `/TimePunches/employee/${employeeId}`,
            {
              params: {
                fromUtc,
                toUtc,
              },
            }
          ),
        ]);

      setDayResult(summaryResponse.data);
      setDayPunches(punchesResponse.data ?? []);
    } catch (error: any) {
      console.error(
        "Erro ao carregar resumo diário:",
        error
      );

      setDayResult(null);
      setDayPunches([]);

      setError(
        apiError(
          error,
          "Não foi possível carregar o resumo diário."
        )
      );
    } finally {
      setAnalysisLoading(false);
    }
  }

  async function loadWeek() {
    if (!employeeId || !selectedDate) {
      return;
    }

    try {
      setAnalysisLoading(true);
      setError("");
      setSuccess("");

      const response =
        await api.get<AttendanceWeek>(
          `/Attendance/employee/${employeeId}/week/${selectedDate}`
        );

      setWeekResult(response.data);
    } catch (error: any) {
      console.error(
        "Erro ao carregar resumo semanal:",
        error
      );

      setWeekResult(null);

      setError(
        apiError(
          error,
          "Não foi possível carregar o resumo semanal."
        )
      );
    } finally {
      setAnalysisLoading(false);
    }
  }

  async function registerPunch(
    direction: PunchDirection
  ) {
    if (!employeeId) {
      setError("Selecione um funcionário.");
      return;
    }

    try {
      setPunching(direction);
      setError("");
      setSuccess("");

      await api.post("/TimePunches", {
        employeeId,
        direction,
      });

      setSuccess(
        direction === 1
          ? "Entrada registada com sucesso."
          : "Saída registada com sucesso."
      );

      await loadTodayPunches();
    } catch (error: any) {
      console.error(
        "Erro ao registar picagem:",
        error
      );

      setError(
        error.response?.status === 409
          ? apiError(
              error,
              "Já existe uma picagem recente para este funcionário."
            )
          : apiError(
              error,
              "Não foi possível registar a picagem."
            )
      );
    } finally {
      setPunching(null);
    }
  }

  async function createManualPunch(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    if (!employeeId) {
      setError("Selecione um funcionário.");
      return;
    }

    if (!selectedDate) {
      setError("Selecione uma data.");
      return;
    }

    if (!manualTime) {
      setError("Indique a hora.");
      return;
    }

    if (!manualReason.trim()) {
      setError(
        "O motivo da picagem manual é obrigatório."
      );
      return;
    }

    try {
      setManualSaving(true);
      setError("");
      setSuccess("");

      await api.post(
        "/TimePunches/manual",
        {
          employeeId,
          localDate: selectedDate,
          localTime: manualTime,
          direction: manualDirection,
          reason: manualReason.trim(),
        }
      );

      setSuccess(
        "Picagem manual adicionada com sucesso."
      );

      setManualReason("");
      setShowManualForm(false);

      await loadDay();
    } catch (error: any) {
      console.error(
        "Erro ao criar picagem manual:",
        error
      );

      setError(
        apiError(
          error,
          "Não foi possível adicionar a picagem manual."
        )
      );
    } finally {
      setManualSaving(false);
    }
  }

  const selectedEmployee =
    employees.find(
      (employee) =>
        employee.id === employeeId
    ) ?? null;

  const selectedEmployeePunches =
    useMemo(
      () =>
        employeeId
          ? punches.filter(
              (punch) =>
                punch.employeeId === employeeId &&
                !punch.isVoided
            )
          : [],
      [punches, employeeId]
    );

  const lastPunch =
    selectedEmployeePunches.length > 0
      ? [...selectedEmployeePunches].sort(
          (a, b) =>
            new Date(
              b.timestampUtc
            ).getTime() -
            new Date(
              a.timestampUtc
            ).getTime()
        )[0]
      : null;

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold text-slate-900">
          Ponto
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Registo e análise das presenças dos funcionários.
        </p>
      </div>

      <div className="inline-flex rounded-xl bg-slate-200 p-1">
        <TabButton
          active={viewMode === "today"}
          onClick={() =>
            setViewMode("today")
          }
        >
          Hoje
        </TabButton>

        <TabButton
          active={viewMode === "day"}
          onClick={() =>
            setViewMode("day")
          }
        >
          Dia
        </TabButton>

        <TabButton
          active={viewMode === "week"}
          onClick={() =>
            setViewMode("week")
          }
        >
          Semana
        </TabButton>
      </div>

      {error && (
        <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
          {error}
        </div>
      )}

      {success && (
        <div className="rounded-xl border border-green-200 bg-green-50 p-4 text-sm text-green-700">
          {success}
        </div>
      )}

      {viewMode === "today" && (
        <>
          <section className="rounded-xl bg-white p-6 shadow-sm">
            <div className="grid grid-cols-1 gap-6 xl:grid-cols-[1fr_auto] xl:items-end">
              <EmployeeSelect
                employees={employees}
                value={employeeId}
                disabled={loading}
                onChange={(value) => {
                  setEmployeeId(value);
                  setError("");
                  setSuccess("");
                }}
              />

              <div className="flex flex-wrap gap-3">
                <button
                  type="button"
                  disabled={
                    !employeeId ||
                    punching !== null
                  }
                  onClick={() =>
                    void registerPunch(1)
                  }
                  className="min-w-32 rounded-lg bg-green-600 px-5 py-3 text-sm font-semibold text-white hover:bg-green-700 disabled:cursor-not-allowed disabled:opacity-50"
                >
                  {punching === 1
                    ? "A registar..."
                    : "Entrada"}
                </button>

                <button
                  type="button"
                  disabled={
                    !employeeId ||
                    punching !== null
                  }
                  onClick={() =>
                    void registerPunch(2)
                  }
                  className="min-w-32 rounded-lg bg-red-600 px-5 py-3 text-sm font-semibold text-white hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                >
                  {punching === 2
                    ? "A registar..."
                    : "Saída"}
                </button>
              </div>
            </div>

            {selectedEmployee && (
              <div className="mt-6 grid grid-cols-1 gap-4 border-t border-slate-100 pt-5 sm:grid-cols-3">
                <InfoCard
                  label="Funcionário"
                  value={employeeName(
                    selectedEmployee
                  )}
                />

                <InfoCard
                  label="Picagens hoje"
                  value={String(
                    selectedEmployeePunches.length
                  )}
                />

                <InfoCard
                  label="Última picagem"
                  value={
                    lastPunch
                      ? `${formatTime(
                          lastPunch.timestampUtc
                        )} · ${
                          lastPunch.directionName
                        }`
                      : "-"
                  }
                />
              </div>
            )}
          </section>

          <TodayPunchTable
            punches={punches}
            loading={loading}
            onRefresh={() =>
              void loadTodayPunches()
            }
          />
        </>
      )}

      {viewMode === "day" && (
        <>
          <AnalysisFilters
            employees={employees}
            employeeId={employeeId}
            selectedDate={selectedDate}
            loading={loading}
            onEmployeeChange={(value) => {
              setEmployeeId(value);
              setError("");
            }}
            onDateChange={(value) => {
              setSelectedDate(value);
              setError("");
            }}
            onRefresh={() =>
              void loadDay()
            }
          />

          <div className="flex justify-end">
            <button
              type="button"
              disabled={!employeeId || !selectedDate}
              onClick={() => {
                setShowManualForm(
                  (current) => !current
                );
                setError("");
                setSuccess("");
              }}
              className="rounded-lg bg-slate-900 px-4 py-2.5 text-sm font-semibold text-white hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {showManualForm
                ? "Cancelar picagem manual"
                : "+ Adicionar picagem manual"}
            </button>
          </div>

          {showManualForm && (
            <section className="rounded-xl border border-blue-100 bg-blue-50 p-6">
              <h3 className="text-lg font-semibold text-slate-900">
                Nova picagem manual
              </h3>

              <p className="mt-1 text-sm text-slate-600">
                A picagem será registada para a data selecionada e ficará identificada como ajuste manual.
              </p>

              <form
                onSubmit={createManualPunch}
                className="mt-5 grid grid-cols-1 gap-5 lg:grid-cols-[180px_180px_1fr_auto] lg:items-end"
              >
                <div>
                  <label className="block text-sm font-medium text-slate-700">
                    Tipo
                  </label>

                  <select
                    value={manualDirection}
                    onChange={(event) =>
                      setManualDirection(
                        Number(
                          event.target.value
                        ) as PunchDirection
                      )
                    }
                    className="mt-2 w-full rounded-lg border border-slate-300 bg-white px-3 py-3 text-sm text-slate-800 outline-none focus:border-blue-500"
                  >
                    <option value={1}>
                      Entrada
                    </option>
                    <option value={2}>
                      Saída
                    </option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-700">
                    Hora
                  </label>

                  <input
                    type="time"
                    value={manualTime}
                    onChange={(event) =>
                      setManualTime(
                        event.target.value
                      )
                    }
                    className="mt-2 w-full rounded-lg border border-slate-300 bg-white px-3 py-3 text-sm text-slate-800 outline-none focus:border-blue-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-700">
                    Motivo
                  </label>

                  <input
                    type="text"
                    value={manualReason}
                    onChange={(event) =>
                      setManualReason(
                        event.target.value
                      )
                    }
                    maxLength={500}
                    placeholder="Ex.: Correção de picagem esquecida"
                    className="mt-2 w-full rounded-lg border border-slate-300 bg-white px-3 py-3 text-sm text-slate-800 outline-none focus:border-blue-500"
                  />
                </div>

                <button
                  type="submit"
                  disabled={manualSaving}
                  className="rounded-lg bg-blue-600 px-5 py-3 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
                >
                  {manualSaving
                    ? "A guardar..."
                    : "Adicionar"}
                </button>
              </form>
            </section>
          )}

          {analysisLoading ? (
            <LoadingCard text="A calcular o dia..." />
          ) : dayResult ? (
            <>
              <DaySummary result={dayResult} />
              <DayPunchTable punches={dayPunches} />
            </>
          ) : (
            <EmptyAnalysis
              text="Selecione um funcionário e uma data para consultar o dia."
            />
          )}
        </>
      )}

      {viewMode === "week" && (
        <>
          <AnalysisFilters
            employees={employees}
            employeeId={employeeId}
            selectedDate={selectedDate}
            loading={loading}
            onEmployeeChange={(value) => {
              setEmployeeId(value);
              setError("");
            }}
            onDateChange={(value) => {
              setSelectedDate(value);
              setError("");
            }}
            onRefresh={() =>
              void loadWeek()
            }
          />

          {analysisLoading ? (
            <LoadingCard text="A calcular a semana..." />
          ) : weekResult ? (
            <WeekSummary result={weekResult} />
          ) : (
            <EmptyAnalysis
              text="Selecione um funcionário e uma data para consultar a semana."
            />
          )}
        </>
      )}
    </div>
  );
}

function DayPunchTable({
  punches,
}: {
  punches: TimePunch[];
}) {
  const ordered =
    [...punches].sort(
      (a, b) =>
        new Date(
          a.timestampUtc
        ).getTime() -
        new Date(
          b.timestampUtc
        ).getTime()
    );

  return (
    <section className="overflow-hidden rounded-xl bg-white shadow-sm">
      <div className="border-b border-slate-100 px-6 py-5">
        <h3 className="text-lg font-semibold text-slate-900">
          Picagens do dia
        </h3>

        <p className="mt-1 text-sm text-slate-500">
          Registos que estão a ser usados no cálculo diário.
        </p>
      </div>

      {ordered.length === 0 ? (
        <div className="p-8 text-center text-slate-500">
          Não existem picagens para este dia.
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-50">
              <tr className="border-b">
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Hora
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Tipo
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Origem
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Motivo
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Estado
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-slate-100">
              {ordered.map((punch) => (
                <tr
                  key={punch.id}
                  className={
                    punch.isVoided
                      ? "bg-slate-50 opacity-60"
                      : "hover:bg-slate-50"
                  }
                >
                  <td className="px-6 py-4 font-semibold tabular-nums text-slate-900">
                    {formatTime(
                      punch.timestampUtc
                    )}
                  </td>

                  <td className="px-6 py-4">
                    <DirectionBadge
                      direction={
                        punch.direction
                      }
                      label={
                        punch.directionName
                      }
                    />
                  </td>

                  <td className="px-6 py-4">
                    <SourceBadge
                      source={punch.source}
                      label={punch.sourceName}
                    />
                  </td>

                  <td className="max-w-md px-6 py-4 text-slate-600">
                    {punch.adjustmentReason ??
                      "-"}
                  </td>

                  <td className="px-6 py-4">
                    {punch.isVoided ? (
                      <span className="inline-flex rounded-full bg-slate-200 px-3 py-1 text-xs font-medium text-slate-600">
                        Anulada
                      </span>
                    ) : (
                      <span className="inline-flex rounded-full bg-green-100 px-3 py-1 text-xs font-medium text-green-700">
                        Válida
                      </span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}

function SourceBadge({
  source,
  label,
}: {
  source: number;
  label: string;
}) {
  if (source === 3) {
    return (
      <span className="inline-flex rounded-full bg-amber-100 px-3 py-1 text-xs font-medium text-amber-800">
        {label || "Ajuste manual"}
      </span>
    );
  }

  if (source === 2) {
    return (
      <span className="inline-flex rounded-full bg-purple-100 px-3 py-1 text-xs font-medium text-purple-700">
        {label || "Dispositivo"}
      </span>
    );
  }

  return (
    <span className="inline-flex rounded-full bg-blue-100 px-3 py-1 text-xs font-medium text-blue-700">
      {label || "HRVault"}
    </span>
  );
}

function AnalysisFilters({
  employees,
  employeeId,
  selectedDate,
  loading,
  onEmployeeChange,
  onDateChange,
  onRefresh,
}: {
  employees: Employee[];
  employeeId: string;
  selectedDate: string;
  loading: boolean;
  onEmployeeChange: (value: string) => void;
  onDateChange: (value: string) => void;
  onRefresh: () => void;
}) {
  return (
    <section className="rounded-xl bg-white p-6 shadow-sm">
      <div className="grid grid-cols-1 gap-5 md:grid-cols-[1fr_220px_auto] md:items-end">
        <EmployeeSelect
          employees={employees}
          value={employeeId}
          disabled={loading}
          onChange={onEmployeeChange}
        />

        <div>
          <label className="block text-sm font-medium text-slate-700">
            Data
          </label>

          <input
            type="date"
            value={selectedDate}
            onChange={(event) =>
              onDateChange(
                event.target.value
              )
            }
            className="mt-2 w-full rounded-lg border border-slate-300 px-3 py-3 text-sm text-slate-800 outline-none focus:border-blue-500"
          />
        </div>

        <button
          type="button"
          onClick={onRefresh}
          disabled={
            !employeeId ||
            !selectedDate
          }
          className="rounded-lg bg-blue-600 px-5 py-3 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
        >
          Atualizar
        </button>
      </div>
    </section>
  );
}

function DaySummary({
  result,
}: {
  result: AttendanceDay;
}) {
  return (
    <div className="space-y-6">
      <section className="rounded-xl bg-white p-6 shadow-sm">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p className="text-sm text-slate-500">
              {formatDate(result.date)}
            </p>

            <h3 className="mt-1 text-xl font-semibold text-slate-900">
              {result.workScheduleName ??
                "Sem horário"}
            </h3>

            {result.workScheduleType && (
              <p className="mt-1 text-sm text-slate-500">
                {scheduleTypeLabel(
                  result.workScheduleType
                )}
              </p>
            )}
          </div>

          <StatusBadge
            status={result.status}
          />
        </div>

        <div className="mt-6 grid grid-cols-2 gap-4 md:grid-cols-4 xl:grid-cols-7">
          <Metric
            label="Previsto"
            value={result.expectedTime}
          />
          <Metric
            label="Trabalhado"
            value={result.workedTime}
          />
          <Metric
            label="Pausas"
            value={result.breakTime}
          />
          <Metric
            label="Saldo"
            value={result.balance}
          />
          <Metric
            label="Atraso"
            value={result.lateTime}
          />
          <Metric
            label="Saída antecipada"
            value={result.earlyLeaveTime}
          />
          <Metric
            label="Horas extra"
            value={result.overtime}
          />
        </div>

        <div className="mt-6 grid grid-cols-1 gap-4 border-t border-slate-100 pt-5 sm:grid-cols-2">
          <InfoCard
            label="Primeira entrada"
            value={
              result.firstEntryUtc
                ? formatTime(
                    result.firstEntryUtc
                  )
                : "-"
            }
          />

          <InfoCard
            label="Última saída"
            value={
              result.lastExitUtc
                ? formatTime(
                    result.lastExitUtc
                  )
                : "-"
            }
          />
        </div>
      </section>

      <Alerts alerts={result.alerts} />
    </div>
  );
}

function WeekSummary({
  result,
}: {
  result: AttendanceWeek;
}) {
  return (
    <div className="space-y-6">
      <section className="rounded-xl bg-white p-6 shadow-sm">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <p className="text-sm text-slate-500">
              Semana
            </p>

            <h3 className="mt-1 text-xl font-semibold text-slate-900">
              {formatDate(result.weekStart)}
              {" — "}
              {formatDate(result.weekEnd)}
            </h3>
          </div>

          <StatusBadge
            status={result.status}
          />
        </div>

        <div className="mt-6 grid grid-cols-2 gap-4 md:grid-cols-4 xl:grid-cols-8">
          <Metric
            label="Dias obrigatórios"
            value={String(
              result.requiredWorkingDays
            )}
          />
          <Metric
            label="Dias trabalhados"
            value={String(
              result.workedDays
            )}
          />
          <Metric
            label="Dias em falta"
            value={String(
              result.missingWorkingDays
            )}
          />
          <Metric
            label="Previsto"
            value={result.expectedTime}
          />
          <Metric
            label="Trabalhado"
            value={result.workedTime}
          />
          <Metric
            label="Pausas"
            value={result.breakTime}
          />
          <Metric
            label="Saldo"
            value={result.balance}
          />
          <Metric
            label="Horas extra"
            value={result.overtime}
          />
        </div>
      </section>

      <Alerts alerts={result.alerts} />

      <section className="overflow-hidden rounded-xl bg-white shadow-sm">
        <div className="border-b border-slate-100 px-6 py-5">
          <h3 className="text-lg font-semibold text-slate-900">
            Detalhe da semana
          </h3>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-50">
              <tr className="border-b">
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Dia
                </th>
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Horário
                </th>
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Estado
                </th>
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Previsto
                </th>
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Trabalhado
                </th>
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Pausas
                </th>
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Saldo
                </th>
                <th className="px-5 py-4 font-semibold text-slate-600">
                  Extra
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-slate-100">
              {result.days.map((day) => (
                <tr
                  key={day.date}
                  className="hover:bg-slate-50"
                >
                  <td className="px-5 py-4">
                    <div className="font-medium text-slate-800">
                      {weekdayLabel(
                        day.date
                      )}
                    </div>

                    <div className="mt-0.5 text-xs text-slate-400">
                      {formatDate(
                        day.date
                      )}
                    </div>
                  </td>

                  <td className="px-5 py-4 text-slate-600">
                    {day.workScheduleName ??
                      "-"}
                  </td>

                  <td className="px-5 py-4">
                    <StatusBadge
                      status={day.status}
                    />
                  </td>

                  <td className="px-5 py-4 tabular-nums text-slate-600">
                    {day.expectedTime}
                  </td>

                  <td className="px-5 py-4 tabular-nums font-medium text-slate-800">
                    {day.workedTime}
                  </td>

                  <td className="px-5 py-4 tabular-nums text-slate-600">
                    {day.breakTime}
                  </td>

                  <td className="px-5 py-4 tabular-nums text-slate-600">
                    {day.balance}
                  </td>

                  <td className="px-5 py-4 tabular-nums text-slate-600">
                    {day.overtime}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}

function TodayPunchTable({
  punches,
  loading,
  onRefresh,
}: {
  punches: TimePunch[];
  loading: boolean;
  onRefresh: () => void;
}) {
  return (
    <section className="overflow-hidden rounded-xl bg-white shadow-sm">
      <div className="flex flex-col gap-3 border-b border-slate-100 px-6 py-5 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-lg font-semibold text-slate-900">
            Picagens de hoje
          </h3>

          <p className="mt-1 text-sm text-slate-500">
            {loading
              ? "A carregar..."
              : `${punches.length} ${
                  punches.length === 1
                    ? "picagem"
                    : "picagens"
                }`}
          </p>
        </div>

        <button
          type="button"
          onClick={onRefresh}
          disabled={loading}
          className="self-start rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:opacity-50"
        >
          Atualizar
        </button>
      </div>

      {loading ? (
        <div className="p-8 text-center text-slate-500">
          A carregar picagens...
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead className="bg-slate-50">
              <tr className="border-b">
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Hora
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Funcionário
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Picagem
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Origem
                </th>
                <th className="px-6 py-4 font-semibold text-slate-600">
                  Estado
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-slate-100">
              {punches.map((punch) => (
                <tr
                  key={punch.id}
                  className={
                    punch.isVoided
                      ? "bg-slate-50 opacity-60"
                      : "hover:bg-slate-50"
                  }
                >
                  <td className="px-6 py-4 font-semibold tabular-nums text-slate-900">
                    {formatTime(
                      punch.timestampUtc
                    )}
                  </td>

                  <td className="px-6 py-4 font-medium text-slate-800">
                    {punch.employeeName}
                  </td>

                  <td className="px-6 py-4">
                    <DirectionBadge
                      direction={
                        punch.direction
                      }
                      label={
                        punch.directionName
                      }
                    />
                  </td>

                  <td className="px-6 py-4 text-slate-600">
                    {punch.sourceName}
                  </td>

                  <td className="px-6 py-4">
                    {punch.isVoided ? (
                      <span className="inline-flex rounded-full bg-slate-200 px-3 py-1 text-xs font-medium text-slate-600">
                        Anulada
                      </span>
                    ) : (
                      <span className="inline-flex rounded-full bg-green-100 px-3 py-1 text-xs font-medium text-green-700">
                        Válida
                      </span>
                    )}
                  </td>
                </tr>
              ))}

              {punches.length === 0 && (
                <tr>
                  <td
                    colSpan={5}
                    className="px-6 py-12 text-center text-slate-500"
                  >
                    Ainda não existem picagens hoje.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}

function EmployeeSelect({
  employees,
  value,
  disabled,
  onChange,
}: {
  employees: Employee[];
  value: string;
  disabled?: boolean;
  onChange: (value: string) => void;
}) {
  return (
    <div>
      <label className="block text-sm font-medium text-slate-700">
        Funcionário
      </label>

      <select
        value={value}
        onChange={(event) =>
          onChange(event.target.value)
        }
        disabled={disabled}
        className="mt-2 w-full rounded-lg border border-slate-300 px-3 py-3 text-sm text-slate-800 outline-none focus:border-blue-500 disabled:bg-slate-100"
      >
        <option value="">
          Selecione um funcionário
        </option>

        {employees
          .slice()
          .sort((a, b) =>
            employeeName(a).localeCompare(
              employeeName(b),
              "pt"
            )
          )
          .map((employee) => (
            <option
              key={employee.id}
              value={employee.id}
            >
              {employee.employeeNumber
                ? `${employee.employeeNumber} - `
                : ""}
              {employeeName(employee)}
            </option>
          ))}
      </select>
    </div>
  );
}

function Alerts({
  alerts,
}: {
  alerts: AttendanceAlert[];
}) {
  if (alerts.length === 0) {
    return null;
  }

  return (
    <section className="rounded-xl border border-amber-200 bg-amber-50 p-5">
      <h3 className="font-semibold text-amber-900">
        Alertas
      </h3>

      <div className="mt-3 space-y-2">
        {alerts.map((alert, index) => (
          <div
            key={`${alert.type}-${index}`}
            className="rounded-lg bg-white/70 px-4 py-3 text-sm text-amber-900"
          >
            <span className="font-semibold">
              {alertTypeLabel(
                alert.type
              )}
            </span>
            {" — "}
            {alert.message}
          </div>
        ))}
      </div>
    </section>
  );
}

function Metric({
  label,
  value,
}: {
  label: string;
  value: string;
}) {
  return (
    <div className="rounded-lg bg-slate-50 p-4">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
        {label}
      </p>

      <p className="mt-2 text-lg font-bold tabular-nums text-slate-900">
        {value}
      </p>
    </div>
  );
}

function InfoCard({
  label,
  value,
}: {
  label: string;
  value: string;
}) {
  return (
    <div className="rounded-lg bg-slate-50 p-4">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
        {label}
      </p>

      <p className="mt-2 font-semibold text-slate-800">
        {value}
      </p>
    </div>
  );
}

function LoadingCard({
  text,
}: {
  text: string;
}) {
  return (
    <div className="rounded-xl bg-white p-10 text-center text-slate-500 shadow-sm">
      {text}
    </div>
  );
}

function EmptyAnalysis({
  text,
}: {
  text: string;
}) {
  return (
    <div className="rounded-xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-500">
      {text}
    </div>
  );
}

function TabButton({
  active,
  onClick,
  children,
}: {
  active: boolean;
  onClick: () => void;
  children: React.ReactNode;
}) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`rounded-lg px-5 py-2.5 text-sm font-semibold transition ${
        active
          ? "bg-white text-blue-700 shadow-sm"
          : "text-slate-600 hover:text-slate-900"
      }`}
    >
      {children}
    </button>
  );
}

function StatusBadge({
  status,
}: {
  status: string;
}) {
  const config: Record<
    string,
    string
  > = {
    Complete:
      "bg-green-100 text-green-700",
    InProgress:
      "bg-blue-100 text-blue-700",
    Incomplete:
      "bg-amber-100 text-amber-800",
    NoPunches:
      "bg-red-100 text-red-700",
    NonWorkingDay:
      "bg-slate-100 text-slate-600",
  };

  return (
    <span
      className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${
        config[status] ??
        "bg-slate-100 text-slate-600"
      }`}
    >
      {statusLabel(status)}
    </span>
  );
}

function DirectionBadge({
  direction,
  label,
}: {
  direction: number;
  label: string;
}) {
  if (direction === 1) {
    return (
      <span className="inline-flex rounded-full bg-green-100 px-3 py-1 text-xs font-medium text-green-700">
        {label || "Entrada"}
      </span>
    );
  }

  if (direction === 2) {
    return (
      <span className="inline-flex rounded-full bg-red-100 px-3 py-1 text-xs font-medium text-red-700">
        {label || "Saída"}
      </span>
    );
  }

  return (
    <span className="inline-flex rounded-full bg-slate-100 px-3 py-1 text-xs font-medium text-slate-600">
      {label || "Não definido"}
    </span>
  );
}

function employeeName(employee: Employee) {
  if (employee.fullName?.trim()) {
    return employee.fullName.trim();
  }

  const name = [
    employee.firstName,
    employee.lastName,
  ]
    .filter(Boolean)
    .join(" ")
    .trim();

  return name || "Funcionário";
}

function formatTime(timestampUtc: string) {
  return new Intl.DateTimeFormat(
    "pt-PT",
    {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    }
  ).format(new Date(timestampUtc));
}

function formatDate(value: string) {
  const [year, month, day] =
    value.split("-").map(Number);

  return new Intl.DateTimeFormat(
    "pt-PT",
    {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    }
  ).format(
    new Date(year, month - 1, day)
  );
}

function weekdayLabel(value: string) {
  const [year, month, day] =
    value.split("-").map(Number);

  const text =
    new Intl.DateTimeFormat(
      "pt-PT",
      {
        weekday: "long",
      }
    ).format(
      new Date(year, month - 1, day)
    );

  return (
    text.charAt(0).toUpperCase() +
    text.slice(1)
  );
}

function statusLabel(status: string) {
  const labels: Record<
    string,
    string
  > = {
    InProgress: "Em curso",
    Complete: "Completo",
    Incomplete: "Incompleto",
    NoPunches: "Sem picagens",
    NonWorkingDay: "Dia não trabalhado",
  };

  return labels[status] ?? status;
}

function alertTypeLabel(type: string) {
  const labels: Record<
    string,
    string
  > = {
    MissingExit: "Falta saída",
    MissingEntry: "Falta entrada",
    InvalidSequence:
      "Sequência inválida",
    MissingWorkingDays:
      "Dias de trabalho em falta",
  };

  return labels[type] ?? type;
}

function scheduleTypeLabel(type: string) {
  const labels: Record<
    string,
    string
  > = {
    Fixed: "Horário fixo",
    Flexible: "Horário livre",
    WeeklyVariable:
      "Semana variável",
    ScheduleExempt:
      "Isenção de horário",
  };

  return labels[type] ?? type;
}

function apiError(
  error: any,
  fallback: string
) {
  return (
    error.response?.data?.message ??
    error.response?.data?.title ??
    fallback
  );
}

function todayInputValue() {
  const now = new Date();

  const year = now.getFullYear();
  const month = String(
    now.getMonth() + 1
  ).padStart(2, "0");

  const day = String(
    now.getDate()
  ).padStart(2, "0");

  return `${year}-${month}-${day}`;
}
