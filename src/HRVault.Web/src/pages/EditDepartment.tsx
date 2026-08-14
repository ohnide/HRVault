import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";

interface Department {
  id: string;
  companyId: string;
  name: string;
  description?: string | null;
  parentDepartmentId?: string | null;
}

export default function EditDepartment() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [department, setDepartment] =
    useState<Department | null>(null);

  const [departments, setDepartments] =
    useState<Department[]>([]);

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [parentDepartmentId, setParentDepartmentId] =
    useState("");

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!id) {
      setError("Departamento inválido.");
      setLoading(false);
      return;
    }

    loadData(id);
  }, [id]);

  async function loadData(departmentId: string) {
    try {
      setLoading(true);
      setError("");

      const [departmentResponse, departmentsResponse] =
        await Promise.all([
          api.get<Department>(
            `/Departments/${departmentId}`
          ),
          api.get<Department[]>("/Departments"),
        ]);

      const data = departmentResponse.data;

      setDepartment(data);
      setDepartments(departmentsResponse.data);

      setName(data.name);
      setDescription(data.description ?? "");
      setParentDepartmentId(
        data.parentDepartmentId ?? ""
      );

    } catch (error: any) {
      console.error(
        "Erro ao carregar departamento:",
        error
      );

      setError(
        error.response?.data?.message ??
          "Não foi possível carregar o departamento."
      );
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

    if (!department) {
      return;
    }

    if (
      parentDepartmentId &&
      parentDepartmentId === department.id
    ) {
      setError(
        "Um departamento não pode ser pai de si próprio."
      );
      return;
    }

    try {
      setSaving(true);
      setError("");

      await api.put(
        `/Departments/${department.id}`,
        {
          id: department.id,
          companyId: department.companyId,
          name,
          description: description || null,
          parentDepartmentId:
            parentDepartmentId || null,
        }
      );

      navigate(
        `/departments/${department.id}`
      );

    } catch (error: any) {
      console.error(
        "Erro ao atualizar departamento:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível atualizar o departamento."
      );
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="rounded-xl bg-white p-8 text-center shadow-sm">
        <p className="text-slate-500">
          A carregar departamento...
        </p>
      </div>
    );
  }

  if (!department) {
    return (
      <div>

        <button
          type="button"
          onClick={() =>
            navigate("/departments")
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para departamentos
        </button>

        <div className="rounded-xl border border-red-200 bg-red-50 p-5 text-red-700">
          {error || "Departamento não encontrado."}
        </div>

      </div>
    );
  }

  return (
    <div>

      <div className="mb-6">

        <button
          type="button"
          onClick={() =>
            navigate(
              `/departments/${department.id}`
            )
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para departamento
        </button>

        <h2 className="text-3xl font-bold text-slate-900">
          Editar departamento
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Alterar os dados do departamento.
        </p>

      </div>

      <form
        onSubmit={handleSubmit}
        className="max-w-3xl rounded-xl bg-white p-8 shadow-sm"
      >

        <div className="space-y-6">

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Nome
            </label>

            <input
              type="text"
              value={name}
              onChange={(event) =>
                setName(event.target.value)
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Descrição
            </label>

            <textarea
              value={description}
              onChange={(event) =>
                setDescription(event.target.value)
              }
              rows={4}
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Departamento pai
            </label>

            <select
              value={parentDepartmentId}
              onChange={(event) =>
                setParentDepartmentId(
                  event.target.value
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            >
              <option value="">
                Sem departamento pai
              </option>

              {departments
                .filter(
                  (item) =>
                    item.id !== department.id
                )
                .map((item) => (
                  <option
                    key={item.id}
                    value={item.id}
                  >
                    {item.name}
                  </option>
                ))}
            </select>
          </div>

        </div>

        {error && (
          <div className="mt-6 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            {error}
          </div>
        )}

        <div className="mt-8 flex justify-end gap-3 border-t pt-6">

          <button
            type="button"
            onClick={() =>
              navigate(
                `/departments/${department.id}`
              )
            }
            className="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Cancelar
          </button>

          <button
            type="submit"
            disabled={saving}
            className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {saving
              ? "A guardar..."
              : "Guardar alterações"}
          </button>

        </div>

      </form>

    </div>
  );
}

