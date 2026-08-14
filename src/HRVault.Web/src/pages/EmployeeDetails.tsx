import { useEffect, useState } from "react";
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

export default function EmployeeDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [employee, setEmployee] =
    useState<Employee | null>(null);

  const [departments, setDepartments] =
    useState<Department[]>([]);

  const [loading, setLoading] =
    useState(true);

  const [error, setError] =
    useState("");
	
  const [positions, setPositions] =
    useState<Position[]>([]);

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

    setEmployee(
      employeeResponse.data
    );

    setDepartments(
      departmentsResponse.data
    );

    setPositions(
      positionsResponse.data
    );

  } catch (error: any) {
    console.error(
      "Erro ao carregar funcionário:",
      error
    );

    if (error.response?.status === 404) {
      setError(
        "Funcionário não encontrado."
      );
    } else {
      setError(
        error.response?.data?.message ??
          "Não foi possível carregar o funcionário."
      );
    }

  } finally {
    setLoading(false);
  }
}
  function getStatusInfo(
    status: number
  ) {
    switch (status) {
      case 1:
        return {
          label: "Ativo",
          className:
            "bg-green-100 text-green-700",
        };

      case 2:
        return {
          label: "Inativo",
          className:
            "bg-slate-100 text-slate-600",
        };

      case 3:
        return {
          label: "Suspenso",
          className:
            "bg-yellow-100 text-yellow-700",
        };

      case 4:
        return {
          label: "Terminado",
          className:
            "bg-red-100 text-red-700",
        };

      default:
        return {
          label: "Desconhecido",
          className:
            "bg-slate-100 text-slate-600",
        };
    }
  }

  function getDepartmentName(
    departmentId?: string | null
  ) {
    if (!departmentId) {
      return "-";
    }

    const department =
      departments.find(
        (item) =>
          item.id === departmentId
      );

    return department?.name ??
      "Departamento não encontrado";
  }

	function getPositionName(
	  positionId?: string | null
	) {
	  if (!positionId) {
		return "-";
	  }

	  const position =
		positions.find(
		  (item) => item.id === positionId
		);

	  if (!position) {
		return "Cargo não encontrado";
	  }

	  return `${position.code} - ${position.name}`;
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

  if (error) {
    return (
      <div>
        <button
          onClick={() =>
            navigate("/employees")
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <div className="rounded-xl border border-red-200 bg-red-50 p-5 text-red-700">
          {error}
        </div>
      </div>
    );
  }

  if (!employee) {
    return null;
  }

  const status =
    getStatusInfo(employee.status);

  return (
    <div>

      <div className="mb-6">

        <button
          onClick={() =>
            navigate("/employees")
          }
          className="mb-4 text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          ← Voltar para funcionários
        </button>

        <div className="flex flex-col justify-between gap-4 md:flex-row md:items-center">

          <div>

            <div className="flex items-center gap-3">

              <h2 className="text-3xl font-bold text-slate-900">
                {employee.firstName}{" "}
                {employee.lastName}
              </h2>

              <span
                className={`rounded-full px-3 py-1 text-xs font-medium ${status.className}`}
              >
                {status.label}
              </span>

            </div>

            <p className="mt-1 text-sm text-slate-500">
              Funcionário{" "}
              {employee.employeeNumber}
            </p>

          </div>

          <button
            onClick={() =>
              navigate(
                `/employees/${employee.id}/edit`
              )
            }
            className="rounded-lg bg-blue-600 px-5 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
          >
            Editar
          </button>

        </div>

      </div>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">

        <section className="rounded-xl bg-white p-6 shadow-sm">

          <h3 className="mb-5 text-lg font-semibold text-slate-900">
            Dados profissionais
          </h3>

          <div className="space-y-4">

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Número de funcionário
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {employee.employeeNumber}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Email profissional
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {employee.workEmail ?? "-"}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Telemóvel
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {employee.mobilePhone ?? "-"}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Data de entrada
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {new Date(
                  employee.hireDate
                ).toLocaleDateString("pt-PT")}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Data de saída
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {employee.terminationDate
                  ? new Date(
                      employee.terminationDate
                    ).toLocaleDateString("pt-PT")
                  : "-"}
              </p>
            </div>

          </div>

        </section>

        <section className="rounded-xl bg-white p-6 shadow-sm">

          <h3 className="mb-5 text-lg font-semibold text-slate-900">
            Dados pessoais
          </h3>

          <div className="space-y-4">

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Nome completo
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {employee.firstName}{" "}
                {employee.lastName}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Email pessoal
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {employee.personalEmail ?? "-"}
              </p>
            </div>

          </div>

        </section>

        <section className="rounded-xl bg-white p-6 shadow-sm">

          <h3 className="mb-5 text-lg font-semibold text-slate-900">
            Organização
          </h3>

          <div className="space-y-4">

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Departamento
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {getDepartmentName(
                  employee.departmentId
                )}
              </p>
            </div>

            <div>
              <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                Cargo
              </p>

              <p className="mt-1 text-sm text-slate-800">
                {getPositionName(
				  employee.positionId
				  )}
              </p>
            </div>

          </div>

        </section>

      </div>

    </div>
  );
}
