import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";

interface Position {
  id: string;
  companyId: string;
  code: string;
  name: string;
  description?: string | null;
  isActive: boolean;
}

export default function Positions() {
  const [positions, setPositions] =
    useState<Position[]>([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const navigate = useNavigate();

  useEffect(() => {
    loadPositions();
  }, []);

  async function loadPositions() {
    try {
      setLoading(true);
      setError("");

      const response =
        await api.get<Position[]>(
          "/Positions"
        );

      setPositions(response.data);

    } catch (error: any) {
      console.error(
        "Erro ao carregar cargos:",
        error
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível carregar os cargos."
      );

    } finally {
      setLoading(false);
    }
  }

  function getStatusInfo(
    isActive: boolean
  ) {
    if (isActive) {
      return {
        label: "Ativo",
        className:
          "bg-green-100 text-green-700",
      };
    }

    return {
      label: "Inativo",
      className:
        "bg-slate-100 text-slate-600",
    };
  }

  return (
    <div>

      <div className="mb-6 flex items-center justify-between">

        <div>
          <h2 className="text-3xl font-bold text-slate-900">
            Cargos
          </h2>

          <p className="mt-1 text-sm text-slate-500">
            Gestão dos cargos da empresa.
          </p>
        </div>

        <button
          type="button"
          onClick={() =>
            navigate("/positions/new")
          }
          className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
        >
          + Novo cargo
        </button>

      </div>

      {loading && (
        <div className="rounded-xl bg-white p-8 text-center shadow-sm">
          <p className="text-slate-500">
            A carregar cargos...
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
                    Código
                  </th>

                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Nome
                  </th>

                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Descrição
                  </th>

                  <th className="px-6 py-4 font-semibold text-slate-600">
                    Estado
                  </th>

                </tr>
              </thead>

              <tbody className="divide-y">

                {positions.map(
                  (position) => {
                    const status =
                      getStatusInfo(
                        position.isActive
                      );

                    return (
                      <tr
                        key={position.id}
                        onClick={() =>
                          navigate(
                            `/positions/${position.id}`
                          )
                        }
                        className="cursor-pointer hover:bg-slate-50"
                      >

                        <td className="px-6 py-4 font-medium text-slate-700">
                          {position.code}
                        </td>

                        <td className="px-6 py-4 font-medium text-slate-900">
                          {position.name}
                        </td>

                        <td className="px-6 py-4 text-slate-600">
                          {position.description ??
                            "-"}
                        </td>

                        <td className="px-6 py-4">

                          <span
                            className={`rounded-full px-3 py-1 text-xs font-medium ${status.className}`}
                          >
                            {status.label}
                          </span>

                        </td>

                      </tr>
                    );
                  }
                )}

                {positions.length === 0 && (
                  <tr>
                    <td
                      colSpan={4}
                      className="px-6 py-12 text-center text-slate-500"
                    >
                      Não existem cargos.
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