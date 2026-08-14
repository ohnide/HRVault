import { useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../api/client";

function getCompanyIdFromToken(): string | null {
  const token = localStorage.getItem("hrvault_token");

  if (!token) {
    return null;
  }

  try {
    const payload = JSON.parse(
      atob(token.split(".")[1])
    );

    return payload.companyId ?? null;
  } catch {
    return null;
  }
}

export default function NewPosition() {
  const navigate = useNavigate();

  const [code, setCode] = useState("");
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [isActive, setIsActive] = useState(true);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

    setError("");

    const companyId = getCompanyIdFromToken();

    if (!companyId) {
      setError(
        "Não foi possível determinar a empresa do utilizador."
      );
      return;
    }

    try {
      setLoading(true);

      await api.post("/Positions", {
        companyId,
        code,
        name,
        description: description || null,
        isActive,
      });

      navigate("/positions");

    } catch (error: any) {
      console.error(
        "Erro ao criar cargo:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível criar o cargo."
      );

    } finally {
      setLoading(false);
    }
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

        <h2 className="text-3xl font-bold text-slate-900">
          Novo cargo
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Registar um novo cargo.
        </p>

      </div>

      <form
        onSubmit={handleSubmit}
        className="max-w-3xl rounded-xl bg-white p-8 shadow-sm"
      >

        <div className="grid grid-cols-1 gap-6">

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
              placeholder="CAR001"
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
              placeholder="Pasteleiro"
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
              rows={4}
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="Descrição do cargo..."
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Estado
            </label>

            <select
              value={isActive ? "true" : "false"}
              onChange={(event) =>
                setIsActive(
                  event.target.value === "true"
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            >
              <option value="true">
                Ativo
              </option>

              <option value="false">
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
              navigate("/positions")
            }
            className="rounded-lg border border-slate-300 px-5 py-2.5 text-sm font-medium text-slate-700 hover:bg-slate-50"
          >
            Cancelar
          </button>

          <button
            type="submit"
            disabled={loading}
            className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {loading
              ? "A guardar..."
              : "Criar cargo"}
          </button>

        </div>

      </form>

    </div>
  );
}

