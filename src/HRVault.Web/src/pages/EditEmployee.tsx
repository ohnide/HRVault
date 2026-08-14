import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../api/client";

interface Employee {
  id: string;
  companyId: string;
  departmentId?: string | null;
  positionId?: string | null;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  workEmail?: string | null;
  personalEmail?: string | null;
  mobilePhone?: string | null;
  hireDate: string;
  terminationDate?: string | null;
  status: number;
}

interface Department {
  id: string;
  companyId: string;
  name: string;
  description?: string | null;
  parentDepartmentId?: string | null;
}

interface Position {
  id: string;
  companyId: string;
  code: string;
  name: string;
  description?: string | null;
  isActive: boolean;
}

export default function EditEmployee() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [employee, setEmployee] =
    useState<Employee | null>(null);

  const [employeeNumber, setEmployeeNumber] =
    useState("");

  const [firstName, setFirstName] =
    useState("");

  const [lastName, setLastName] =
    useState("");

  const [workEmail, setWorkEmail] =
    useState("");

  const [personalEmail, setPersonalEmail] =
    useState("");

  const [mobilePhone, setMobilePhone] =
    useState("");

  const [hireDate, setHireDate] =
    useState("");

  const [terminationDate, setTerminationDate] =
    useState("");

  const [departmentId, setDepartmentId] =
    useState("");

  const [positionId, setPositionId] =
    useState("");

  const [status, setStatus] =
    useState(1);

  const [departments, setDepartments] =
    useState<Department[]>([]);

  const [positions, setPositions] =
    useState<Position[]>([]);

  const [loading, setLoading] =
    useState(true);

  const [saving, setSaving] =
    useState(false);

  const [error, setError] =
    useState("");

  useEffect(() => {
    if (!id) {
      setError("Funcionário inválido.");
      setLoading(false);
      return;
    }

    loadEmployee(id);
  }, [id]);

  async function loadEmployee(
    employeeId: string
  ) {
    try {
      setLoading(true);
      setError("");

      const [
        employeeResponse,
        departmentsResponse,
        positionsResponse,
      ] = await Promise.all([
        api.get<Employee>(
          `/Employees/${employeeId}`
        ),

        api.get<Department[]>(
          "/Departments"
        ),

        api.get<Position[]>(
          "/Positions"
        ),
      ]);

      const data =
        employeeResponse.data;

      setEmployee(data);

      setDepartments(
        departmentsResponse.data
      );

      setPositions(
        positionsResponse.data
      );

      setEmployeeNumber(
        data.employeeNumber
      );

      setFirstName(
        data.firstName
      );

      setLastName(
        data.lastName
      );

      setWorkEmail(
        data.workEmail ?? ""
      );

      setPersonalEmail(
        data.personalEmail ?? ""
      );

      setMobilePhone(
        data.mobilePhone ?? ""
      );

      setHireDate(
        data.hireDate
      );

      setTerminationDate(
        data.terminationDate ?? ""
      );

      setDepartmentId(
        data.departmentId ?? ""
      );

      setPositionId(
        data.positionId ?? ""
      );

      setStatus(
        data.status
      );

    } catch (error: any) {
      console.error(
        "Erro ao carregar funcionário:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          "Não foi possível carregar o funcionário."
      );

    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(
    event: FormEvent
  ) {
    event.preventDefault();

    if (!employee) {
      return;
    }

    try {
      setSaving(true);
      setError("");

      await api.put(
        `/Employees/${employee.id}`,
        {
          id: employee.id,
          companyId: employee.companyId,

          departmentId:
            departmentId || null,

          positionId:
            positionId || null,

          employeeNumber,
          firstName,
          lastName,

          workEmail:
            workEmail || null,

          personalEmail:
            personalEmail || null,

          mobilePhone:
            mobilePhone || null,

          hireDate,

          terminationDate:
            terminationDate || null,

          status,
        }
      );

      navigate(
        `/employees/${employee.id}`
      );

    } catch (error: any) {
      console.error(
        "Erro ao atualizar funcionário:",
        error
      );

      console.error(
        "Resposta:",
        error.response?.data
      );

      setError(
        error.response?.data?.message ??
          error.response?.data?.title ??
          "Não foi possível atualizar o funcionário."
      );

    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="rounded-xl bg-white p-8 text-center shadow-sm">
        <p className="text-slate-500">
          A carregar funcionário...
        </p>
      </div>
    );
  }

  if (!employee) {
    return (
      <div>

        <button
          type="button"
          onClick={() =>
            navigate("/employees")
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <div className="rounded-xl border border-red-200 bg-red-50 p-5 text-red-700">
          {error ||
            "Funcionário não encontrado."}
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
              `/employees/${employee.id}`
            )
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionário
        </button>

        <h2 className="text-3xl font-bold text-slate-900">
          Editar funcionário
        </h2>

        <p className="mt-1 text-sm text-slate-500">
          {employee.firstName}{" "}
          {employee.lastName}
        </p>

      </div>

      <form
        onSubmit={handleSubmit}
        className="max-w-5xl rounded-xl bg-white p-8 shadow-sm"
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
                setEmployeeNumber(
                  event.target.value
                )
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Estado
            </label>

            <select
              value={status}
              onChange={(event) =>
                setStatus(
                  Number(
                    event.target.value
                  )
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            >
              <option value={1}>
                Ativo
              </option>

              <option value={2}>
                Inativo
              </option>

              <option value={3}>
                Suspenso
              </option>

              <option value={4}>
                Terminado
              </option>
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Nome
            </label>

            <input
              type="text"
              value={firstName}
              onChange={(event) =>
                setFirstName(
                  event.target.value
                )
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
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
                setLastName(
                  event.target.value
                )
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Departamento
            </label>

            <select
              value={departmentId}
              onChange={(event) =>
                setDepartmentId(
                  event.target.value
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            >
              <option value="">
                Sem departamento
              </option>

              {departments.map(
                (department) => (
                  <option
                    key={department.id}
                    value={department.id}
                  >
                    {department.name}
                  </option>
                )
              )}
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Cargo
            </label>

            <select
              value={positionId}
              onChange={(event) =>
                setPositionId(
                  event.target.value
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            >
              <option value="">
                Sem cargo
              </option>

              {positions
                .filter(
                  (position) =>
                    position.isActive
                )
                .map(
                  (position) => (
                    <option
                      key={position.id}
                      value={position.id}
                    >
                      {position.code} -{" "}
                      {position.name}
                    </option>
                  )
                )}
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Email profissional
            </label>

            <input
              type="email"
              value={workEmail}
              onChange={(event) =>
                setWorkEmail(
                  event.target.value
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
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
                setPersonalEmail(
                  event.target.value
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
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
                setMobilePhone(
                  event.target.value
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
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
                setHireDate(
                  event.target.value
                )
              }
              required
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">
              Data de saída
            </label>

            <input
              type="date"
              value={terminationDate}
              onChange={(event) =>
                setTerminationDate(
                  event.target.value
                )
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
            />
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
                `/employees/${employee.id}`
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