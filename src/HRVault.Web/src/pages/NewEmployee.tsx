import { useEffect, useState } from "react";
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

interface Department {
  id: string;
  companyId: string;
  name: string;
  description?: string | null;
  parentDepartmentId?: string | null;
}

export default function NewEmployee() {
  const navigate = useNavigate();

  const [employeeNumber, setEmployeeNumber] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [workEmail, setWorkEmail] = useState("");
  const [personalEmail, setPersonalEmail] = useState("");
  const [mobilePhone, setMobilePhone] = useState("");
  const [hireDate, setHireDate] = useState("");
  const [departments, setDepartments] = useState<Department[]>([]);
  const [departmentId, setDepartmentId] = useState("");

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

      await api.post("/Employees", {
        companyId,
        employeeNumber,
        firstName,
        lastName,
        workEmail: workEmail || null,
        personalEmail: personalEmail || null,
        mobilePhone: mobilePhone || null,
		departmentId: departmentId || null,
        hireDate,
      });

      navigate("/employees");
    } catch (error: any) {
      console.error(
        "Erro ao criar funcionário:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
        error.response?.data?.title ??
        "Não foi possível criar o funcionário."
      );
    } finally {
      setLoading(false);
    }
  }

	useEffect(() => {
	  loadDepartments();
	}, []);

	async function loadDepartments() {
	  try {
		const response = await api.get<Department[]>(
		  "/Departments"
		);

		setDepartments(response.data);
	  } catch (error) {
		console.error(
		  "Erro ao carregar departamentos:",
		  error
		);
	  }
	}

  return (
    <div>

      <div className="mb-6">

        <button
          type="button"
          onClick={() => navigate("/employees")}
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <h2 className="text-3xl font-bold text-slate-900">
          Novo funcionário
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          Registar um novo funcionário.
        </p>

      </div>

      <form
        onSubmit={handleSubmit}
        className="max-w-4xl rounded-xl bg-white p-8 shadow-sm"
      >

        <div className="grid grid-cols-1 gap-6 md:grid-cols-2">

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Número de funcionário
            </label>

            <input
              type="text"
              value={employeeNumber}
              onChange={(event) =>
                setEmployeeNumber(event.target.value)
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="EMP001"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Data de entrada
            </label>

            <input
              type="date"
              value={hireDate}
              onChange={(event) =>
                setHireDate(event.target.value)
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Nome
            </label>

            <input
              type="text"
              value={firstName}
              onChange={(event) =>
                setFirstName(event.target.value)
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="João"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Apelido
            </label>

            <input
              type="text"
              value={lastName}
              onChange={(event) =>
                setLastName(event.target.value)
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="Silva"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Email profissional
            </label>

            <input
              type="email"
              value={workEmail}
              onChange={(event) =>
                setWorkEmail(event.target.value)
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="joao@empresa.pt"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Email pessoal
            </label>

            <input
              type="email"
              value={personalEmail}
              onChange={(event) =>
                setPersonalEmail(event.target.value)
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="joao@gmail.com"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Telemóvel
            </label>

            <input
              type="tel"
              value={mobilePhone}
              onChange={(event) =>
                setMobilePhone(event.target.value)
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="912345678"
            />
          </div>

<div>
  <label className="mb-1 block text-sm font-medium text-slate-700">
    Departamento
  </label>

  <select
    value={departmentId}
    onChange={(event) =>
      setDepartmentId(event.target.value)
    }
    className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
  >
    <option value="">
      Sem departamento
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
            onClick={() => navigate("/employees")}
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
              : "Criar funcionário"}
          </button>

        </div>

      </form>

    </div>
  );
}