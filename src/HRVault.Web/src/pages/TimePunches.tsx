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
  createdAt: string;
}

type PunchDirection = 1 | 2;

export default function TimePunches() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [punches, setPunches] = useState<TimePunch[]>([]);
  const [employeeId, setEmployeeId] = useState("");
  const [loading, setLoading] = useState(true);
  const [punching, setPunching] = useState<PunchDirection | null>(null);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  useEffect(() => {
    void loadInitialData();
  }, []);

  async function loadInitialData() {
    try {
      setLoading(true);
      setError("");

      const [employeesResponse, punchesResponse] = await Promise.all([
        api.get<Employee[]>("/Employees"),
        api.get<TimePunch[]>("/TimePunches/today"),
      ]);

      setEmployees(employeesResponse.data);
      setPunches(punchesResponse.data);
    } catch (error: any) {
      console.error("Erro ao carregar ponto:", error);

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar os dados de ponto."
      );
    } finally {
      setLoading(false);
    }
  }

  async function loadTodayPunches() {
    try {
      setError("");

      const response = await api.get<TimePunch[]>(
        "/TimePunches/today"
      );

      setPunches(response.data);
    } catch (error: any) {
      console.error(
        "Erro ao carregar picagens de hoje:",
        error
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível atualizar as picagens."
      );
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

      if (error.response?.status === 409) {
        setError(
          error.response?.data?.message ??
            "Já existe uma picagem recente para este funcionário."
        );
      } else {
        setError(
          error.response?.data?.message ??
            error.response?.data?.title ??
            "Não foi possível registar a picagem."
        );
      }
    } finally {
      setPunching(null);
    }
  }

  const selectedEmployee =
    employees.find(
      (employee) => employee.id === employeeId
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
            new Date(b.timestampUtc).getTime() -
            new Date(a.timestampUtc).getTime()
        )[0]
      : null;

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-3xl font-bold text-slate-900">
          Ponto
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Registo e consulta das picagens dos funcionários.
        </p>
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

      <section className="rounded-xl bg-white p-6 shadow-sm">
        <div className="grid grid-cols-1 gap-6 xl:grid-cols-[1fr_auto] xl:items-end">
          <div>
            <label className="block text-sm font-medium text-slate-700">
              Funcionário
            </label>

            <select
              value={employeeId}
              onChange={(event) => {
                setEmployeeId(event.target.value);
                setError("");
                setSuccess("");
              }}
              disabled={loading}
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
            onClick={() =>
              void loadTodayPunches()
            }
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

                    <td className="px-6 py-4">
                      <div className="font-medium text-slate-800">
                        {punch.employeeName}
                      </div>
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
    </div>
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
