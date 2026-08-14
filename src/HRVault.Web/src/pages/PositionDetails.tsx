import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";

interface Position {
  id: string;
  companyId: string;
  code: string;
  name: string;
  description?: string | null;
  isActive: boolean;
}

export default function PositionDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [position, setPosition] =
    useState<Position | null>(null);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");

  const [deleting, setDeleting] =
    useState(false);

  useEffect(() => {
    if (!id) {
      setError("Cargo inválido.");
      setLoading(false);
      return;
    }

    loadPosition(id);
  }, [id]);

  async function loadPosition(positionId: string) {
    try {
      setLoading(true);
      setError("");

      const response =
        await api.get<Position>(
          `/Positions/${positionId}`
        );

      setPosition(response.data);

    } catch (error: any) {
      console.error(
        "Erro ao carregar cargo:",
        error
      );

      if (error.response?.status === 404) {
        setError("Cargo não encontrado.");
      } else {
        setError(
          error.response?.data?.message ??
            error.response?.data?.title ??
            "Não foi possível carregar o cargo."
        );
      }

    } finally {
      setLoading(false);
    }
  }

  async function handleDelete() {
    if (!position) {
      return;
    }

    const confirmed = window.confirm(
      `Tem a certeza que pretende eliminar o cargo "${position.name}"?`
    );

    if (!confirmed) {
      return;
    }

    try {
      setDeleting(true);
      setError("");

      await api.delete(
        `/Positions/${position.id}`
      );

      navigate("/positions");

    } catch (error: any) {
      console.error(
        "Erro ao eliminar cargo:",
        error
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível eliminar o cargo."
      );

    } finally {
      setDeleting(false);
    }
  }

  if (loading) {
    return (
      <div className="rounded-xl bg-white p-8 text-center shadow-sm">
        <p className="text-slate-500">
          A carregar cargo...
        </p>
      </div>
    );
  }

  if (error && !position) {
    return (
      <div>

        <button
          type="button"
          onClick={() =>
            navigate("/positions")
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para cargos
        </button>

        <div className="rounded-xl border border-red-200 bg-red-50 p-5 text-red-700">
          {error}
        </div>

      </div>
    );
  }

  if (!position) {
    return null;
  }

  return (
    <div>

      <div className="mb-6">

        <button
          type="button"
          onClick={() =>
            navigate("/positions")
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para cargos
        </button>

        <div className="flex flex-col justify-between gap-4 md:flex-row md:items-center">

          <div>

            <div className="flex items-center gap-3">

              <h2 className="text-3xl font-bold text-slate-900">
                {position.name}
              </h2>

              <span
                className={`rounded-full px-3 py-1 text-xs font-medium ${
                  position.isActive
                    ? "bg-green-100 text-green-700"
                    : "bg-slate-100 text-slate-600"
                }`}
              >
                {position.isActive
                  ? "Ativo"
                  : "Inativo"}
              </span>

            </div>

            <p className="mt-1 text-sm text-slate-500">
              Código: {position.code}
            </p>

          </div>

          <div className="flex gap-3">

            <button
              type="button"
              onClick={() =>
                navigate(
                  `/positions/${position.id}/edit`
                )
              }
              className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
            >
              Editar
            </button>

            <button
              type="button"
              onClick={handleDelete}
              disabled={deleting}
              className="rounded-lg bg-red-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-red-700 disabled:opacity-50"
            >
              {deleting
                ? "A eliminar..."
                : "Eliminar"}
            </button>

          </div>

        </div>

      </div>

      {error && (
        <div className="mb-6 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">

        <section className="rounded-xl bg-white p-6 shadow-sm">

          <h3 className="mb-5 text-lg font-semibold text-slate-900">
            Dados do cargo
          </h3>

          <div className="space-y-4">

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Código
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {position.code}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Nome
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {position.name}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Descrição
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {position.description ?? "-"}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Estado
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {position.isActive
                  ? "Ativo"
                  : "Inativo"}
              </p>
            </div>

          </div>

        </section>

      </div>

    </div>
  );
}