import { useEffect, useState } from "react";
import type { FormEvent } from "react";
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

export default function EditPosition() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [position, setPosition] =
    useState<Position | null>(null);

  const [code, setCode] =
    useState("");

  const [name, setName] =
    useState("");

  const [description, setDescription] =
    useState("");

  const [isActive, setIsActive] =
    useState(true);

  const [loading, setLoading] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [error, setError] =
    useState("");

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

      const data = response.data;

      setPosition(data);

      setCode(data.code);
      setName(data.name);
      setDescription(data.description ?? "");
      setIsActive(data.isActive);

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

  async function handleSubmit(
    event: FormEvent
  ) {
    event.preventDefault();

    if (!position) {
      return;
    }

    try {
      setSaving(true);
      setError("");

      await api.put(
        `/Positions/${position.id}`,
        {
          id: position.id,
          companyId: position.companyId,
          code,
          name,
          description:
            description || null,
          isActive,
        }
      );

      navigate(
        `/positions/${position.id}`
      );

    } catch (error: any) {
      console.error(
        "Erro ao atualizar cargo:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível atualizar o cargo."
      );

    } finally {
      setSaving(false);
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

  if (!position) {
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
          {error ||
            "Cargo não encontrado."}
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
              `/positions/${position.id}`
            )
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para cargo
        </button>

        <h2 className="text-3xl font-bold text-slate-900">
          Editar cargo
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          {position.name}
        </p>

      </div>

      <form
        onSubmit={handleSubmit}
        className="max-w-3xl rounded-xl bg-white p-8 shadow-sm"
      >

        <div className="space-y-6">

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Código
            </label>

            <input
              type="text"
              value={code}
              onChange={(event) =>
                setCode(event.target.value)
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="DIR"
            />
          </div>

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
              placeholder="Diretor"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Descrição
            </label>

            <textarea
              value={description}
              onChange={(event) =>
                setDescription(
                  event.target.value
                )
              }
              rows={5}
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="Descrição do cargo..."
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Estado
            </label>

            <select
              value={
                isActive ? "active" : "inactive"
              }
              onChange={(event) =>
                setIsActive(
                  event.target.value ===
                    "active"
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            >
              <option value="active">
                Ativo
              </option>

              <option value="inactive">
                Inativo
              </option>
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
                `/positions/${position.id}`
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