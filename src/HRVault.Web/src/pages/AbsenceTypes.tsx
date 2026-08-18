import { useEffect, useState } from "react";
import { api } from "../api/client";

interface AbsenceType {
  id: string;
  name: string;
  description?: string | null;
  requiresApproval: boolean;
  requiresDocument: boolean;
  isPaid: boolean;
}

interface AbsenceTypeForm {
  name: string;
  description: string;
  requiresApproval: boolean;
  requiresDocument: boolean;
  isPaid: boolean;
}

const emptyForm: AbsenceTypeForm = {
  name: "",
  description: "",
  requiresApproval: true,
  requiresDocument: false,
  isPaid: false,
};

export default function AbsenceTypes() {
  const [types, setTypes] = useState<AbsenceType[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [error, setError] = useState("");
  const [showForm, setShowForm] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<AbsenceTypeForm>(emptyForm);

  useEffect(() => {
    void loadTypes();
  }, []);

  async function loadTypes() {
    try {
      setLoading(true);
      setError("");
      const response = await api.get<AbsenceType[]>("/AbsenceTypes");
      setTypes(response.data);
    } catch (error: any) {
      console.error("Erro ao carregar tipos de ausência:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar os tipos de ausência."
      );
    } finally {
      setLoading(false);
    }
  }

  function openCreateForm() {
    setEditingId(null);
    setForm(emptyForm);
    setError("");
    setShowForm(true);
  }

  function openEditForm(type: AbsenceType) {
    setEditingId(type.id);
    setForm({
      name: type.name,
      description: type.description ?? "",
      requiresApproval: type.requiresApproval,
      requiresDocument: type.requiresDocument,
      isPaid: type.isPaid,
    });
    setError("");
    setShowForm(true);
  }

  function closeForm() {
    setShowForm(false);
    setEditingId(null);
    setForm(emptyForm);
    setError("");
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const name = form.name.trim();
    const description = form.description.trim();

    if (!name) {
      setError("O nome do tipo de ausência é obrigatório.");
      return;
    }

    if (name.length > 150) {
      setError("O nome não pode ultrapassar 150 caracteres.");
      return;
    }

    if (description.length > 500) {
      setError("A descrição não pode ultrapassar 500 caracteres.");
      return;
    }

    const payload = {
      name,
      description: description || null,
      requiresApproval: form.requiresApproval,
      requiresDocument: form.requiresDocument,
      isPaid: form.isPaid,
    };

    try {
      setSaving(true);
      setError("");

      if (editingId) {
        await api.put(`/AbsenceTypes/${editingId}`, {
          id: editingId,
          ...payload,
        });
      } else {
        await api.post("/AbsenceTypes", payload);
      }

      closeForm();
      await loadTypes();
    } catch (error: any) {
      console.error("Erro ao guardar tipo de ausência:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível guardar o tipo de ausência."
      );
    } finally {
      setSaving(false);
    }
  }

  async function deleteType(type: AbsenceType) {
    const confirmed = window.confirm(
      `Tem a certeza de que pretende apagar o tipo "${type.name}"?`
    );

    if (!confirmed) return;

    try {
      setDeletingId(type.id);
      setError("");
      await api.delete(`/AbsenceTypes/${type.id}`);
      setTypes((current) => current.filter((item) => item.id !== type.id));

      if (editingId === type.id) {
        closeForm();
      }
    } catch (error: any) {
      console.error("Erro ao apagar tipo de ausência:", error);
      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível apagar o tipo de ausência."
      );
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-3xl font-bold text-slate-900">
            Tipos de ausência
          </h2>
          <p className="mt-1 text-sm text-slate-500">
            Configure os tipos de ausência utilizados na gestão dos funcionários.
          </p>
        </div>

        <button
          type="button"
          onClick={showForm ? closeForm : openCreateForm}
          className="self-start rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
        >
          {showForm ? "Cancelar" : "+ Novo tipo"}
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
            {editingId ? "Editar tipo de ausência" : "Novo tipo de ausência"}
          </h3>

          <form onSubmit={handleSubmit} className="mt-5 space-y-5">
            <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
              <label className="block">
                <span className="text-sm font-medium text-slate-700">Nome *</span>
                <input
                  type="text"
                  value={form.name}
                  onChange={(event) =>
                    setForm((current) => ({ ...current, name: event.target.value }))
                  }
                  maxLength={150}
                  required
                  className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500"
                />
                <p className="mt-1 text-xs text-slate-400">
                  {form.name.length}/150
                </p>
              </label>

              <label className="block lg:col-span-2">
                <span className="text-sm font-medium text-slate-700">Descrição</span>
                <textarea
                  value={form.description}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      description: event.target.value,
                    }))
                  }
                  maxLength={500}
                  rows={3}
                  className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500"
                />
                <p className="mt-1 text-right text-xs text-slate-400">
                  {form.description.length}/500
                </p>
              </label>

              <div className="lg:col-span-2">
                <span className="text-sm font-medium text-slate-700">
                  Comportamento
                </span>
                <div className="mt-3 grid grid-cols-1 gap-3 md:grid-cols-3">
                  <Option
                    label="Requer aprovação"
                    description="A ausência necessita de aprovação."
                    checked={form.requiresApproval}
                    onChange={(checked) =>
                      setForm((current) => ({
                        ...current,
                        requiresApproval: checked,
                      }))
                    }
                  />
                  <Option
                    label="Requer comprovativo"
                    description="Deve ser apresentado um documento justificativo."
                    checked={form.requiresDocument}
                    onChange={(checked) =>
                      setForm((current) => ({
                        ...current,
                        requiresDocument: checked,
                      }))
                    }
                  />
                  <Option
                    label="Ausência remunerada"
                    description="Este tipo de ausência é considerado remunerado."
                    checked={form.isPaid}
                    onChange={(checked) =>
                      setForm((current) => ({ ...current, isPaid: checked }))
                    }
                  />
                </div>
              </div>
            </div>

            <div className="flex flex-wrap justify-end gap-3 border-t border-slate-100 pt-5">
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
                className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {saving
                  ? "A guardar..."
                  : editingId
                    ? "Guardar alterações"
                    : "Criar tipo"}
              </button>
            </div>
          </form>
        </section>
      )}

      <section className="overflow-hidden rounded-xl bg-white shadow-sm">
        {loading ? (
          <div className="p-8 text-center text-sm text-slate-500">
            A carregar tipos de ausência...
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="border-b bg-slate-50">
                <tr>
                  <th className="px-6 py-4 font-semibold text-slate-600">Nome</th>
                  <th className="px-6 py-4 font-semibold text-slate-600">Descrição</th>
                  <th className="px-6 py-4 font-semibold text-slate-600">Aprovação</th>
                  <th className="px-6 py-4 font-semibold text-slate-600">Comprovativo</th>
                  <th className="px-6 py-4 font-semibold text-slate-600">Remunerada</th>
                  <th className="px-6 py-4 text-right font-semibold text-slate-600">Ações</th>
                </tr>
              </thead>

              <tbody className="divide-y">
                {types.map((type) => (
                  <tr key={type.id} className="hover:bg-slate-50">
                    <td className="px-6 py-4 font-medium text-slate-900">
                      {type.name}
                    </td>
                    <td className="max-w-md px-6 py-4 text-slate-600">
                      {type.description ?? "-"}
                    </td>
                    <BooleanBadge value={type.requiresApproval} />
                    <BooleanBadge value={type.requiresDocument} />
                    <BooleanBadge value={type.isPaid} />
                    <td className="px-6 py-4">
                      <div className="flex justify-end gap-3">
                        <button
                          type="button"
                          onClick={() => openEditForm(type)}
                          className="text-sm font-medium text-blue-600 hover:text-blue-700"
                        >
                          Editar
                        </button>
                        <button
                          type="button"
                          disabled={deletingId === type.id}
                          onClick={() => void deleteType(type)}
                          className="text-sm font-medium text-red-600 hover:text-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                          {deletingId === type.id ? "A apagar..." : "Apagar"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}

                {types.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-6 py-12 text-center text-slate-500">
                      Não existem tipos de ausência.
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

interface OptionProps {
  label: string;
  description: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
}

function Option({ label, description, checked, onChange }: OptionProps) {
  return (
    <label className="flex cursor-pointer gap-3 rounded-lg border border-slate-200 p-4 hover:bg-slate-50">
      <input
        type="checkbox"
        checked={checked}
        onChange={(event) => onChange(event.target.checked)}
        className="mt-1 h-4 w-4 rounded border-slate-300"
      />
      <span>
        <span className="block text-sm font-medium text-slate-800">{label}</span>
        <span className="mt-1 block text-xs leading-5 text-slate-500">
          {description}
        </span>
      </span>
    </label>
  );
}

function BooleanBadge({ value }: { value: boolean }) {
  return (
    <td className="px-6 py-4">
      <span
        className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${
          value
            ? "bg-emerald-100 text-emerald-700"
            : "bg-slate-100 text-slate-600"
        }`}
      >
        {value ? "Sim" : "Não"}
      </span>
    </td>
  );
}
