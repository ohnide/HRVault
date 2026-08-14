import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";

interface Department {
  id: string;
  companyId: string;
  name: string;
  description?: string | null;
  parentDepartmentId?: string | null;
}


export default function NewDepartment() {
  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [parentDepartmentId, setParentDepartmentId] = useState("");

  const [departments, setDepartments] = useState<Department[]>([]);
  const [companyId, setCompanyId] = useState("");

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    try {
      setLoading(true);
      setError("");

      const departmentsResponse =
        await api.get<Department[]>("/Departments");

      setDepartments(departmentsResponse.data);

      /*
       * Nesta fase o backend ainda não tem um endpoint
       * específico para obter a empresa atual.
       *
       * Vamos obter o CompanyId através do primeiro
       * departamento existente.
       *
       * Se ainda não existir nenhum departamento,
       * usamos o CompanyId guardado no utilizador/token,
       * caso esteja disponível.
       */

      if (departmentsResponse.data.length > 0) {
        setCompanyId(
          departmentsResponse.data[0].companyId
        );
      } else {
        const storedCompanyId =
          localStorage.getItem("companyId");

        if (storedCompanyId) {
          setCompanyId(storedCompanyId);
        }
      }

    } catch (error: any) {
      console.error(
        "Erro ao carregar dados:",
        error
      );

      setError(
        error.response?.data?.message ??
          "Não foi possível carregar os dados."
      );
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

    if (!companyId) {
      setError(
        "Não foi possível determinar a empresa."
      );
      return;
    }

    try {
      setSaving(true);
      setError("");

      await api.post("/Departments", {
        companyId,
        name,
        description: description || null,
        parentDepartmentId:
          parentDepartmentId || null,
      });

      navigate("/departments");

    } catch (error: any) {
      console.error(
        "Erro ao criar departamento:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível criar o departamento."
      );
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="rounded-xl bg-white p-8 text-center shadow-sm">
        <p className="text-slate-500">
          A carregar...
        </p>
      </div>
    );
  }

  return (
    <div>

      <div className="mb-6">

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
          Novo departamento
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Criar um novo departamento.
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
              placeholder="Ex.: Produção"
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
              placeholder="Descrição do departamento..."
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

              {departments.map((department) => (
                <option
                  key={department.id}
                  value={department.id}
                >
                  {department.name}
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
              navigate("/departments")
            }
            className="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Cancelar
          </button>

          <button
            type="submit"
            disabled={saving || !companyId}
            className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {saving
              ? "A criar..."
              : "Criar departamento"}
          </button>

        </div>

      </form>

    </div>
  );
}

