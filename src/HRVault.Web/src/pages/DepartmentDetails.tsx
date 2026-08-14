import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";

interface Department {
  id: string;
  companyId: string;
  name: string;
  description?: string | null;
  parentDepartmentId?: string | null;
}

export default function DepartmentDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [department, setDepartment] =
    useState<Department | null>(null);

  const [parentDepartment, setParentDepartment] =
    useState<Department | null>(null);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!id) {
      setError("Departamento inválido.");
      setLoading(false);
      return;
    }

    loadDepartment(id);
  }, [id]);

  async function loadDepartment(
    departmentId: string
  ) {
    try {
      setLoading(true);
      setError("");

      const response = await api.get<Department>(
        `/Departments/${departmentId}`
      );

      const data = response.data;

      setDepartment(data);

      if (data.parentDepartmentId) {
        try {
          const parentResponse =
            await api.get<Department>(
              `/Departments/${data.parentDepartmentId}`
            );

          setParentDepartment(
            parentResponse.data
          );
        } catch {
          setParentDepartment(null);
        }
      } else {
        setParentDepartment(null);
      }

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

  async function handleDelete() {
    if (!department) {
      return;
    }

    const confirmed = window.confirm(
      `Tem a certeza que pretende eliminar o departamento "${department.name}"?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setError("");

      await api.delete(
        `/Departments/${department.id}`
      );

      navigate("/departments");

    } catch (error: any) {
      console.error(
        "Erro ao eliminar departamento:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível eliminar o departamento."
      );
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

      <div className="mb-6 flex items-start justify-between">

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

          <h2 className="text-3xl font-bold text-slate-900">
            {department.name}
          </h2>

          <p className="mt-1 text-sm text-slate-500">
            Detalhes do departamento
          </p>

        </div>

        <div className="flex gap-3">

          <button
            type="button"
            onClick={() =>
              navigate(
                `/departments/${department.id}/edit`
              )
            }
            className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
          >
            Editar
          </button>

          <button
            type="button"
            onClick={handleDelete}
            className="rounded-lg bg-red-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-red-700"
          >
            Eliminar
          </button>

        </div>

      </div>

      {error && (
        <div className="mb-6 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      <div className="max-w-4xl rounded-xl bg-white shadow-sm">

        <div className="border-b px-6 py-5">
          <h3 className="text-lg font-semibold text-slate-900">
            Informação do departamento
          </h3>
        </div>

        <div className="grid grid-cols-1 gap-6 p-6 md:grid-cols-2">

          <div>
            <p className="text-xs font-semibold uppercase tracking-wide text-slate-400">
              Nome
            </p>

            <p className="mt-1 text-sm font-medium text-slate-900">
              {department.name}
            </p>
          </div>

          <div>
            <p className="text-xs font-semibold uppercase tracking-wide text-slate-400">
              Departamento pai
            </p>

            <p className="mt-1 text-sm text-slate-700">
              {parentDepartment?.name ?? "-"}
            </p>
          </div>

          <div className="md:col-span-2">

            <p className="text-xs font-semibold uppercase tracking-wide text-slate-400">
              Descrição
            </p>

            <p className="mt-1 whitespace-pre-wrap text-sm text-slate-700">
              {department.description ?? "-"}
            </p>

          </div>

        </div>

        <div className="border-t px-6 py-4">

          <p className="text-xs text-slate-400">
            ID: {department.id}
          </p>

        </div>

      </div>

    </div>
  );
}



