import { useEffect, useState } from "react";
import { api } from "../api/client";

interface EmployeeDocumentType {
  id: string;
  name: string;
  description?: string | null;
  hasExpiration: boolean;
  expirationWarningDays?: number | null;
}

interface DocumentTypeForm {
  name: string;
  description: string;
  hasExpiration: boolean;
  expirationWarningDays: string;
}

const emptyForm: DocumentTypeForm = {
  name: "",
  description: "",
  hasExpiration: false,
  expirationWarningDays: "",
};

export default function DocumentTypes() {
  const [documentTypes, setDocumentTypes] =
    useState<EmployeeDocumentType[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] =
    useState<string | null>(null);
  const [showForm, setShowForm] =
    useState(false);
  const [editingId, setEditingId] =
    useState<string | null>(null);
  const [form, setForm] =
    useState<DocumentTypeForm>(emptyForm);

  useEffect(() => {
    void loadDocumentTypes();
  }, []);

  async function loadDocumentTypes() {
    try {
      setLoading(true);
      setError("");

      const response =
        await api.get<EmployeeDocumentType[]>(
          "/Employees/document-types"
        );

      setDocumentTypes(response.data);
    } catch (error: any) {
      console.error(
        "Erro ao carregar tipos de documentos:",
        error
      );

      setError(
        error.response?.data?.message ??
          "Não foi possível carregar os tipos de documentos."
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

  function openEditForm(
    documentType: EmployeeDocumentType
  ) {
    setEditingId(documentType.id);
    setForm({
      name: documentType.name,
      description:
        documentType.description ?? "",
      hasExpiration:
        documentType.hasExpiration,
      expirationWarningDays:
        documentType.expirationWarningDays?.toString() ??
        "",
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

  async function handleSubmit(
    event: React.FormEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    const name = form.name.trim();

    if (!name) {
      setError(
        "O nome do tipo de documento é obrigatório."
      );
      return;
    }

    if (name.length > 150) {
      setError(
        "O nome não pode ultrapassar 150 caracteres."
      );
      return;
    }

    const description =
      form.description.trim();

    if (description.length > 500) {
      setError(
        "A descrição não pode ultrapassar 500 caracteres."
      );
      return;
    }

    let expirationWarningDays:
      | number
      | null = null;

    if (
      form.hasExpiration &&
      form.expirationWarningDays !== ""
    ) {
      expirationWarningDays = Number(
        form.expirationWarningDays
      );

      if (
        !Number.isInteger(
          expirationWarningDays
        ) ||
        expirationWarningDays < 0
      ) {
        setError(
          "Os dias de antecedência devem ser um número inteiro igual ou superior a zero."
        );
        return;
      }
    }

    const payload = {
      name,
      description:
        description || null,
      hasExpiration:
        form.hasExpiration,
      expirationWarningDays:
        form.hasExpiration
          ? expirationWarningDays
          : null,
    };

    try {
      setSaving(true);
      setError("");

      if (editingId) {
        await api.put(
          `/Employees/document-types/${editingId}`,
          {
            id: editingId,
            ...payload,
          }
        );
      } else {
        await api.post(
          "/Employees/document-types",
          payload
        );
      }

      closeForm();
      await loadDocumentTypes();
    } catch (error: any) {
      console.error(
        "Erro ao guardar tipo de documento:",
        error
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível guardar o tipo de documento."
      );
    } finally {
      setSaving(false);
    }
  }

  async function deleteDocumentType(
    documentType: EmployeeDocumentType
  ) {
    const confirmed =
      window.confirm(
        `Tem a certeza de que pretende apagar o tipo "${documentType.name}"?`
      );

    if (!confirmed) {
      return;
    }

    try {
      setDeletingId(documentType.id);
      setError("");

      await api.delete(
        `/Employees/document-types/${documentType.id}`
      );

      setDocumentTypes((current) =>
        current.filter(
          (item) =>
            item.id !== documentType.id
        )
      );

      if (
        editingId === documentType.id
      ) {
        closeForm();
      }
    } catch (error: any) {
      console.error(
        "Erro ao apagar tipo de documento:",
        error
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível apagar o tipo de documento."
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
            Tipos de documentos
          </h2>

          <p className="mt-1 text-sm text-slate-500">
            Configure os tipos de documentos utilizados nos funcionários e os respetivos alertas de validade.
          </p>
        </div>

        <button
          type="button"
          onClick={
            showForm
              ? closeForm
              : openCreateForm
          }
          className="self-start rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
        >
          {showForm
            ? "Cancelar"
            : "+ Novo tipo"}
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
            {editingId
              ? "Editar tipo de documento"
              : "Novo tipo de documento"}
          </h3>

          <form
            onSubmit={handleSubmit}
            className="mt-5 space-y-5"
          >
            <div className="grid grid-cols-1 gap-5 lg:grid-cols-2">
              <label className="block">
                <span className="text-sm font-medium text-slate-700">
                  Nome *
                </span>

                <input
                  type="text"
                  value={form.name}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      name: event.target.value,
                    }))
                  }
                  maxLength={150}
                  required
                  className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500"
                />

                <p className="mt-1 text-xs text-slate-400">
                  {form.name.length}/150
                </p>
              </label>

              <div className="block">
                <span className="text-sm font-medium text-slate-700">
                  Validade
                </span>

                <label className="mt-3 flex cursor-pointer items-center gap-3">
                  <input
                    type="checkbox"
                    checked={
                      form.hasExpiration
                    }
                    onChange={(event) =>
                      setForm((current) => ({
                        ...current,
                        hasExpiration:
                          event.target.checked,
                        expirationWarningDays:
                          event.target.checked
                            ? current.expirationWarningDays
                            : "",
                      }))
                    }
                    className="h-4 w-4 rounded border-slate-300"
                  />

                  <span className="text-sm text-slate-700">
                    Este tipo de documento tem data de validade
                  </span>
                </label>
              </div>

              <label className="block lg:col-span-2">
                <span className="text-sm font-medium text-slate-700">
                  Descrição
                </span>

                <textarea
                  value={form.description}
                  onChange={(event) =>
                    setForm((current) => ({
                      ...current,
                      description:
                        event.target.value,
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

              {form.hasExpiration && (
                <label className="block">
                  <span className="text-sm font-medium text-slate-700">
                    Avisar com antecedência
                  </span>

                  <div className="mt-1 flex items-center gap-3">
                    <input
                      type="number"
                      min={0}
                      step={1}
                      value={
                        form.expirationWarningDays
                      }
                      onChange={(event) =>
                        setForm(
                          (current) => ({
                            ...current,
                            expirationWarningDays:
                              event.target
                                .value,
                          })
                        )
                      }
                      className="w-32 rounded-lg border border-slate-300 px-3 py-2.5 text-sm text-slate-800 outline-none focus:border-blue-500"
                    />

                    <span className="text-sm text-slate-500">
                      dias antes da validade
                    </span>
                  </div>
                </label>
              )}
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
            A carregar tipos de documentos...
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
                    Descrição
                  </th>

                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Validade
                  </th>

                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Alerta
                  </th>

                  <th className="px-6 py-4 text-right font-semibold text-slate-600">
                    Ações
                  </th>
                </tr>
              </thead>

              <tbody className="divide-y">
                {documentTypes.map(
                  (documentType) => (
                    <tr
                      key={documentType.id}
                      className="hover:bg-slate-50"
                    >
                      <td className="px-6 py-4 font-medium text-slate-900">
                        {documentType.name}
                      </td>

                      <td className="max-w-md px-6 py-4 text-slate-600">
                        {documentType.description ??
                          "-"}
                      </td>

                      <td className="px-6 py-4">
                        {documentType.hasExpiration ? (
                          <span className="inline-flex rounded-full bg-blue-100 px-2.5 py-1 text-xs font-medium text-blue-700">
                            Com validade
                          </span>
                        ) : (
                          <span className="inline-flex rounded-full bg-slate-100 px-2.5 py-1 text-xs font-medium text-slate-600">
                            Sem validade
                          </span>
                        )}
                      </td>

                      <td className="px-6 py-4 text-slate-600">
                        {documentType.hasExpiration &&
                        documentType.expirationWarningDays !=
                          null
                          ? `${documentType.expirationWarningDays} dias`
                          : "-"}
                      </td>

                      <td className="px-6 py-4">
                        <div className="flex justify-end gap-3">
                          <button
                            type="button"
                            onClick={() =>
                              openEditForm(
                                documentType
                              )
                            }
                            className="text-sm font-medium text-blue-600 hover:text-blue-700"
                          >
                            Editar
                          </button>

                          <button
                            type="button"
                            disabled={
                              deletingId ===
                              documentType.id
                            }
                            onClick={() =>
                              void deleteDocumentType(
                                documentType
                              )
                            }
                            className="text-sm font-medium text-red-600 hover:text-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                          >
                            {deletingId ===
                            documentType.id
                              ? "A apagar..."
                              : "Apagar"}
                          </button>
                        </div>
                      </td>
                    </tr>
                  )
                )}

                {documentTypes.length ===
                  0 && (
                  <tr>
                    <td
                      colSpan={5}
                      className="px-6 py-12 text-center text-slate-500"
                    >
                      Não existem tipos de documentos.
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
