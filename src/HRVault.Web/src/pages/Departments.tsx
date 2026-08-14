import { useEffect, useState } from "react";
import { api } from "../api/client";
import { useNavigate } from "react-router-dom";

interface Department {
  id: string;
  companyId: string;
  name: string;
  description?: string | null;
  parentDepartmentId?: string | null;
}

export default function Departments() {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  useEffect(() => {
    loadDepartments();
  }, []);

  async function loadDepartments() {
    try {
      setLoading(true);
      setError("");

      const response = await api.get<Department[]>(
        "/Departments"
      );

      setDepartments(response.data);
    } catch (error: any) {
      console.error(
        "Erro ao carregar departamentos:",
        error
      );

      setError(
        error.response?.data?.message ??
          "Não foi possível carregar os departamentos."
      );
    } finally {
      setLoading(false);
    }
  }

  function getParentDepartmentName(
    parentDepartmentId?: string | null
  ) {
    if (!parentDepartmentId) {
      return "-";
    }

    const parent = departments.find(
      (department) =>
        department.id === parentDepartmentId
    );

    return parent?.name ?? "-";
  }

  return (
    <div>

      <div className="mb-6 flex items-center justify-between">

        <div>
          <h2 className="text-3xl font-bold text-slate-900">
            Departamentos
          </h2>

          <p className="mt-1 text-sm text-slate-500">
            Gestão dos departamentos da empresa.
          </p>
        </div>

        <button
          onClick={() =>
            navigate("/departments/new")
          }
          className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
        >
          + Novo departamento
        </button>

      </div>

      {loading && (
        <div className="rounded-xl bg-white p-8 text-center shadow-sm">
          <p className="text-slate-500">
            A carregar departamentos...
          </p>
        </div>
      )}

      {error && (
        <div className="rounded-xl border border-red-200 bg-red-50 p-5 text-red-700">
          {error}
        </div>
      )}

      {!loading && !error && (
        <div className="overflow-hidden rounded-xl bg-white shadow-sm">

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
                    Departamento pai
                  </th>

                </tr>
              </thead>

              <tbody className="divide-y">

                {departments.map((department) => (
                  <tr
                    key={department.id}
                    onClick={() =>
                      navigate(
                        `/departments/${department.id}`
                      )
                    }
                    className="cursor-pointer hover:bg-slate-50"
                  >

                    <td className="px-6 py-4 font-medium text-slate-900">
                      {department.name}
                    </td>

                    <td className="px-6 py-4 text-slate-600">
                      {department.description ?? "-"}
                    </td>

                    <td className="px-6 py-4 text-slate-600">
                      {getParentDepartmentName(
                        department.parentDepartmentId
                      )}
                    </td>

                  </tr>
                ))}

                {departments.length === 0 && (
                  <tr>
                    <td
                      colSpan={3}
                      className="px-6 py-12 text-center text-slate-500"
                    >
                      Não existem departamentos.
                    </td>
                  </tr>
                )}

              </tbody>

            </table>

          </div>

        </div>
      )}

    </div>
  );
}

