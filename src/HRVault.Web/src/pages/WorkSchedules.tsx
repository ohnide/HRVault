import { useEffect, useState } from "react";
import { api } from "../api/client";

type ScheduleType = 1 | 2 | 3 | 4;

interface Period {
  id?: string;
  startTime: string;
  endTime: string;
}

interface ScheduleDay {
  id?: string;
  dayOfWeek: number;
  dayName?: string;
  isWorkingDay: boolean;
  requiredDailyTime: string | null;
  periods: Period[];
}

interface WorkSchedule {
  id: string;
  name: string;
  description?: string | null;
  type: ScheduleType;
  typeName: string;
  requiredWorkingDaysPerWeek?: number | null;
  isActive: boolean;
  days: ScheduleDay[];
}

interface FormState {
  id?: string;
  name: string;
  description: string;
  type: ScheduleType;
  requiredWorkingDaysPerWeek: number | null;
  days: ScheduleDay[];
}

const dayDefinitions = [
  { dayOfWeek: 1, label: "Segunda-feira" },
  { dayOfWeek: 2, label: "Terça-feira" },
  { dayOfWeek: 3, label: "Quarta-feira" },
  { dayOfWeek: 4, label: "Quinta-feira" },
  { dayOfWeek: 5, label: "Sexta-feira" },
  { dayOfWeek: 6, label: "Sábado" },
  { dayOfWeek: 0, label: "Domingo" },
];

function emptyDays(): ScheduleDay[] {
  return dayDefinitions.map((day) => ({
    dayOfWeek: day.dayOfWeek,
    dayName: day.label,
    isWorkingDay: false,
    requiredDailyTime: null,
    periods: [],
  }));
}

function emptyForm(): FormState {
  return {
    name: "",
    description: "",
    type: 1,
    requiredWorkingDaysPerWeek: null,
    days: emptyDays(),
  };
}

export default function WorkSchedules() {
  const [schedules, setSchedules] = useState<WorkSchedule[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [changingId, setChangingId] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm());

  useEffect(() => {
    void loadSchedules();
  }, []);

  async function loadSchedules() {
    try {
      setLoading(true);
      setError("");
      const response = await api.get<WorkSchedule[]>("/WorkSchedules");
      setSchedules(response.data);
    } catch (error: any) {
      console.error("Erro ao carregar horários:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar os horários."
      );
    } finally {
      setLoading(false);
    }
  }

  function openCreate() {
    setForm(emptyForm());
    setError("");
    setShowForm(true);
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  function openEdit(schedule: WorkSchedule) {
    setForm({
      id: schedule.id,
      name: schedule.name,
      description: schedule.description ?? "",
      type: schedule.type,
      requiredWorkingDaysPerWeek:
        schedule.requiredWorkingDaysPerWeek ?? null,
      days: dayDefinitions.map((definition) => {
        const source = schedule.days.find(
          (day) => day.dayOfWeek === definition.dayOfWeek
        );
        return {
          id: source?.id,
          dayOfWeek: definition.dayOfWeek,
          dayName: definition.label,
          isWorkingDay: source?.isWorkingDay ?? false,
          requiredDailyTime: source?.requiredDailyTime
            ? toHHmm(source.requiredDailyTime)
            : null,
          periods:
            source?.periods.map((period) => ({
              id: period.id,
              startTime: toHHmm(period.startTime),
              endTime: toHHmm(period.endTime),
            })) ?? [],
        };
      }),
    });
    setError("");
    setShowForm(true);
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  function closeForm() {
    setForm(emptyForm());
    setShowForm(false);
    setError("");
  }

  function changeType(type: ScheduleType) {
    setForm((current) => ({
      ...current,
      type,
      requiredWorkingDaysPerWeek:
        type === 3 ? current.requiredWorkingDaysPerWeek ?? 5 : null,
      days: current.days.map((day) => ({
        ...day,
        requiredDailyTime:
          type === 2 || type === 3
            ? day.isWorkingDay
              ? day.requiredDailyTime ?? "08:00"
              : null
            : null,
        periods:
          type === 1
            ? day.isWorkingDay && day.periods.length === 0
              ? [{ startTime: "08:00", endTime: "17:00" }]
              : day.periods
            : [],
      })),
    }));
  }

  function toggleDay(index: number, checked: boolean) {
    setForm((current) => ({
      ...current,
      days: current.days.map((day, dayIndex) => {
        if (dayIndex !== index) return day;

        return {
          ...day,
          isWorkingDay: checked,
          requiredDailyTime:
            checked && (current.type === 2 || current.type === 3)
              ? day.requiredDailyTime ?? "08:00"
              : null,
          periods:
            checked && current.type === 1
              ? day.periods.length > 0
                ? day.periods
                : [{ startTime: "08:00", endTime: "17:00" }]
              : [],
        };
      }),
    }));
  }

  function updateDailyTime(index: number, value: string) {
    setForm((current) => ({
      ...current,
      days: current.days.map((day, dayIndex) =>
        dayIndex === index
          ? { ...day, requiredDailyTime: value || null }
          : day
      ),
    }));
  }

  function addPeriod(dayIndex: number) {
    setForm((current) => ({
      ...current,
      days: current.days.map((day, index) =>
        index === dayIndex
          ? {
              ...day,
              periods: [
                ...day.periods,
                { startTime: "13:00", endTime: "17:00" },
              ],
            }
          : day
      ),
    }));
  }

  function updatePeriod(
    dayIndex: number,
    periodIndex: number,
    field: "startTime" | "endTime",
    value: string
  ) {
    setForm((current) => ({
      ...current,
      days: current.days.map((day, index) =>
        index === dayIndex
          ? {
              ...day,
              periods: day.periods.map((period, pIndex) =>
                pIndex === periodIndex
                  ? { ...period, [field]: value }
                  : period
              ),
            }
          : day
      ),
    }));
  }

  function removePeriod(dayIndex: number, periodIndex: number) {
    setForm((current) => ({
      ...current,
      days: current.days.map((day, index) =>
        index === dayIndex
          ? {
              ...day,
              periods: day.periods.filter(
                (_, pIndex) => pIndex !== periodIndex
              ),
            }
          : day
      ),
    }));
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!form.name.trim()) {
      setError("O nome do horário é obrigatório.");
      return;
    }

    if (form.type === 3 && !form.requiredWorkingDaysPerWeek) {
      setError("Indique os dias obrigatórios por semana.");
      return;
    }

    const payload = {
      name: form.name.trim(),
      description: form.description.trim() || null,
      type: form.type,
      requiredWorkingDaysPerWeek:
        form.type === 3 ? form.requiredWorkingDaysPerWeek : null,
      days: form.days.map((day) => ({
        dayOfWeek: day.dayOfWeek,
        isWorkingDay: day.isWorkingDay,
        requiredDailyTime:
          day.isWorkingDay && (form.type === 2 || form.type === 3)
            ? apiTime(day.requiredDailyTime)
            : null,
        periods:
          day.isWorkingDay && form.type === 1
            ? day.periods.map((period) => ({
                startTime: apiTime(period.startTime),
                endTime: apiTime(period.endTime),
              }))
            : [],
      })),
    };

    try {
      setSaving(true);
      setError("");

      if (form.id) {
        await api.put(`/WorkSchedules/${form.id}`, {
          id: form.id,
          ...payload,
        });
      } else {
        await api.post("/WorkSchedules", payload);
      }

      closeForm();
      await loadSchedules();
    } catch (error: any) {
      console.error("Erro ao guardar horário:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível guardar o horário."
      );
    } finally {
      setSaving(false);
    }
  }

  async function toggleActive(schedule: WorkSchedule) {
    try {
      setChangingId(schedule.id);
      setError("");

      // Endpoint preparado no backend anterior para ativar/desativar.
      await api.put(`/WorkSchedules/${schedule.id}/active`, {
        isActive: !schedule.isActive,
      });

      await loadSchedules();
    } catch (error: any) {
      console.error("Erro ao alterar estado do horário:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível alterar o estado do horário."
      );
    } finally {
      setChangingId(null);
    }
  }

  async function deleteSchedule(schedule: WorkSchedule) {
    const confirmed = window.confirm(
      `Tem a certeza de que pretende apagar o horário "${schedule.name}"?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setDeletingId(schedule.id);
      setError("");

      await api.delete(`/WorkSchedules/${schedule.id}`);

      if (form.id === schedule.id) {
        closeForm();
      }

      await loadSchedules();
    } catch (error: any) {
      console.error("Erro ao apagar horário:", error);

      if (error.response?.status === 409) {
        setError(
          error.response?.data?.message ??
            "Não é possível apagar este horário porque está atribuído a pelo menos um funcionário."
        );
      } else {
        setError(
          error.response?.data?.message ??
            error.response?.data?.title ??
            "Não foi possível apagar o horário."
        );
      }
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-3xl font-bold text-slate-900">Horários</h2>
          <p className="mt-1 text-sm text-slate-500">
            Gestão dos modelos de horário de trabalho.
          </p>
        </div>

        <button
          type="button"
          onClick={showForm ? closeForm : openCreate}
          className="self-start rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
        >
          {showForm ? "Cancelar" : "+ Novo horário"}
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
            {form.id ? "Editar horário" : "Novo horário"}
          </h3>

          <form onSubmit={handleSubmit} className="mt-5 space-y-6">
            <div className="grid grid-cols-1 gap-5 md:grid-cols-2">
              <Field label="Nome *">
                <input
                  value={form.name}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      name: event.target.value,
                    }))
                  }
                  maxLength={150}
                  required
                  className={inputClass}
                />
              </Field>

              <Field label="Tipo de horário *">
                <select
                  value={form.type}
                  onChange={(event) =>
                    changeType(Number(event.target.value) as ScheduleType)
                  }
                  className={inputClass}
                >
                  <option value={1}>Fixo</option>
                  <option value={2}>Livre</option>
                  <option value={3}>Semanal variável</option>
                  <option value={4}>Isenção de horário</option>
                </select>
              </Field>

              <Field label="Descrição" className="md:col-span-2">
                <textarea
                  value={form.description}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      description: event.target.value,
                    }))
                  }
                  rows={2}
                  maxLength={500}
                  className={inputClass}
                />
              </Field>

              {form.type === 3 && (
                <Field label="Dias obrigatórios por semana *">
                  <input
                    type="number"
                    min={1}
                    max={7}
                    value={form.requiredWorkingDaysPerWeek ?? ""}
                    onChange={(event) =>
                      setForm((current) => ({
                        ...current,
                        requiredWorkingDaysPerWeek:
                          Number(event.target.value) || null,
                      }))
                    }
                    className={inputClass}
                  />
                </Field>
              )}
            </div>

            <div>
              <div className="mb-4">
                <h4 className="font-semibold text-slate-900">
                  {form.type === 3
                    ? "Dias possíveis de trabalho"
                    : "Dias da semana"}
                </h4>
                <p className="mt-1 text-sm text-slate-500">
                  {typeHelp(form.type)}
                </p>
              </div>

              <div className="space-y-3">
                {form.days.map((day, dayIndex) => (
                  <div
                    key={day.dayOfWeek}
                    className="rounded-xl border border-slate-200 p-4"
                  >
                    <div className="flex flex-col gap-4 lg:flex-row lg:items-start">
                      <label className="flex min-w-48 items-center gap-3 pt-2">
                        <input
                          type="checkbox"
                          checked={day.isWorkingDay}
                          onChange={(event) =>
                            toggleDay(dayIndex, event.target.checked)
                          }
                          className="h-4 w-4 rounded border-slate-300"
                        />
                        <span className="font-medium text-slate-800">
                          {day.dayName}
                        </span>
                      </label>

                      {!day.isWorkingDay ? (
                        <div className="pt-2 text-sm text-slate-400">
                          {form.type === 3
                            ? "Não disponível para escala"
                            : "Não trabalhado"}
                        </div>
                      ) : form.type === 1 ? (
                        <div className="flex-1 space-y-3">
                          {day.periods.map((period, periodIndex) => (
                            <div
                              key={periodIndex}
                              className="flex flex-wrap items-end gap-3"
                            >
                              <TimeField
                                label="Entrada"
                                value={period.startTime}
                                onChange={(value) =>
                                  updatePeriod(
                                    dayIndex,
                                    periodIndex,
                                    "startTime",
                                    value
                                  )
                                }
                              />
                              <TimeField
                                label="Saída"
                                value={period.endTime}
                                onChange={(value) =>
                                  updatePeriod(
                                    dayIndex,
                                    periodIndex,
                                    "endTime",
                                    value
                                  )
                                }
                              />
                              <button
                                type="button"
                                onClick={() =>
                                  removePeriod(dayIndex, periodIndex)
                                }
                                className="mb-0.5 rounded-lg border border-red-200 px-3 py-2 text-sm font-medium text-red-600 hover:bg-red-50"
                              >
                                Remover
                              </button>
                            </div>
                          ))}

                          <button
                            type="button"
                            onClick={() => addPeriod(dayIndex)}
                            className="text-sm font-semibold text-blue-600 hover:text-blue-700"
                          >
                            + Adicionar período
                          </button>
                        </div>
                      ) : form.type === 2 || form.type === 3 ? (
                        <div className="w-full max-w-48">
                          <TimeField
                            label="Tempo diário"
                            value={day.requiredDailyTime ?? ""}
                            onChange={(value) =>
                              updateDailyTime(dayIndex, value)
                            }
                          />
                        </div>
                      ) : (
                        <div className="pt-2 text-sm text-slate-500">
                          Sem hora de entrada, saída ou duração obrigatória.
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
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
                  : form.id
                    ? "Guardar alterações"
                    : "Criar horário"}
              </button>
            </div>
          </form>
        </section>
      )}

      <section className="overflow-hidden rounded-xl bg-white shadow-sm">
        {loading ? (
          <div className="p-8 text-center text-slate-500">
            A carregar horários...
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="border-b bg-slate-50">
                <tr>
                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Nome
                  </th>
                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Tipo
                  </th>
                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Resumo
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
                {schedules.map((schedule) => (
                  <tr key={schedule.id} className="hover:bg-slate-50">
                    <td className="px-6 py-4">
                      <div className="font-medium text-slate-900">
                        {schedule.name}
                      </div>
                      {schedule.description && (
                        <div className="mt-1 max-w-xs truncate text-xs text-slate-500">
                          {schedule.description}
                        </div>
                      )}
                    </td>

                    <td className="px-6 py-4">
                      <TypeBadge type={schedule.type} label={schedule.typeName} />
                    </td>

                    <td className="px-6 py-4 text-slate-600">
                      {scheduleSummary(schedule)}
                    </td>

                    <td className="px-6 py-4">
                      <span
                        className={`inline-flex rounded-full px-3 py-1 text-xs font-medium ${
                          schedule.isActive
                            ? "bg-green-100 text-green-700"
                            : "bg-slate-100 text-slate-600"
                        }`}
                      >
                        {schedule.isActive ? "Ativo" : "Inativo"}
                      </span>
                    </td>

                    <td className="px-6 py-4">
                      <div className="flex justify-end gap-4">
                        <button
                          type="button"
                          onClick={() => openEdit(schedule)}
                          className="font-medium text-blue-600 hover:text-blue-700"
                        >
                          Editar
                        </button>
                        <button
                          type="button"
                          disabled={changingId === schedule.id}
                          onClick={() => void toggleActive(schedule)}
                          className="font-medium text-slate-600 hover:text-slate-900 disabled:opacity-50"
                        >
                          {changingId === schedule.id
                            ? "A guardar..."
                            : schedule.isActive
                              ? "Desativar"
                              : "Ativar"}
                        </button>
                        <button
                          type="button"
                          disabled={deletingId === schedule.id}
                          onClick={() => void deleteSchedule(schedule)}
                          className="font-medium text-red-600 hover:text-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                          {deletingId === schedule.id
                            ? "A apagar..."
                            : "Apagar"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}

                {schedules.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      className="px-6 py-12 text-center text-slate-500"
                    >
                      Ainda não existem horários.
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

function TimeField({
  label,
  value,
  onChange,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
}) {
  return (
    <label className="block">
      <span className="text-xs font-medium text-slate-500">{label}</span>
      <input
        type="time"
        step={60}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        className="mt-1 rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-800 outline-none focus:border-blue-500"
      />
    </label>
  );
}

function TypeBadge({
  type,
  label,
}: {
  type: ScheduleType;
  label: string;
}) {
  const classes: Record<ScheduleType, string> = {
    1: "bg-blue-100 text-blue-700",
    2: "bg-violet-100 text-violet-700",
    3: "bg-amber-100 text-amber-700",
    4: "bg-slate-100 text-slate-700",
  };

  return (
    <span
      className={`inline-flex rounded-full px-3 py-1 text-xs font-medium ${classes[type]}`}
    >
      {label}
    </span>
  );
}

function typeHelp(type: ScheduleType) {
  switch (type) {
    case 1:
      return "Defina os dias de trabalho e os períodos de entrada e saída.";
    case 2:
      return "Defina os dias de trabalho e o tempo que deve ser cumprido em cada dia.";
    case 3:
      return "Marque todos os dias em que o funcionário pode trabalhar. A escala escolherá os dias concretos de cada semana.";
    case 4:
      return "Marque os dias aplicáveis. Não são definidas horas de entrada, saída ou duração diária.";
  }
}

function scheduleSummary(schedule: WorkSchedule) {
  const workingDays = schedule.days.filter((day) => day.isWorkingDay);

  if (schedule.type === 3) {
    const required = schedule.requiredWorkingDaysPerWeek ?? 0;
    return `${required} dias/semana entre ${workingDays.length} dias possíveis`;
  }

  if (schedule.type === 4) {
    return "Sem entrada ou saída obrigatória";
  }

  if (schedule.type === 2) {
    const times = Array.from(
      new Set(
        workingDays
          .map((day) =>
            day.requiredDailyTime ? toHHmm(day.requiredDailyTime) : ""
          )
          .filter(Boolean)
      )
    );

    return times.length === 1
      ? `${workingDays.length} dias · ${times[0]}/dia`
      : `${workingDays.length} dias · carga diária variável`;
  }

  return `${workingDays.length} dias com horário definido`;
}

function toHHmm(value: string) {
  return value ? value.slice(0, 5) : "";
}

function apiTime(value: string | null) {
  if (!value) return null;
  return value.length === 5 ? `${value}:00` : value;
}
