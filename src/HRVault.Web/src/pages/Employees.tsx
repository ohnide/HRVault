import { useEffect, useState } from "react";
import { api } from "../api/client";
import { useNavigate } from "react-router-dom";

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
export default function Employees() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    loadEmployees();
  }, []);

  async function loadEmployees() {
    try {
      setLoading(true);
      setError("");

      const response = await api.get<Employee[]>(
        "/Employees"
      );

      setEmployees(response.data);
    } catch (error: any) {
      console.error(
        "Erro ao carregar funcionários:",
        error
      );

      setError(
        error.response?.data?.message ??
        "Não foi possível carregar os funcionários."
      );
    } finally {
      setLoading(false);
    }
  }
  
  function getStatusInfo(status: number) {
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

  return (
    <div>

      <div className="mb-6 flex items-center justify-between">

        <div>
          <h2 className="text-3xl font-bold text-slate-900">
            Funcionários
          </h2>

          <p className="mt-1 text-sm text-slate-500">
            Gestão dos funcionários da empresa.
          </p>
        </div>

        <button
		  onClick={() => navigate("/employees/new")}
		  className="rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white hover:bg-blue-700"
		>
		  + Novo funcionário
		</button>

      </div>

      {loading && (
        <div className="rounded-xl bg-white p-8 text-center shadow-sm">
          <p className="text-slate-500">
            A carregar funcionários...
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
          Nº
        </th>

        <th className="px-6 py-4 font-semibold text-slate-600">
          Nome
        </th>

        <th className="px-6 py-4 font-semibold text-slate-600">
          Email profissional
        </th>

        <th className="px-6 py-4 font-semibold text-slate-600">
          Telemóvel
        </th>

        <th className="px-6 py-4 font-semibold text-slate-600">
          Data entrada
        </th>

        <th className="px-6 py-4 font-semibold text-slate-600">
          Estado
        </th>

      </tr>
    </thead>

    <tbody className="divide-y">

  {employees.map((employee) => (
    <tr
      key={employee.id}
      onClick={() => navigate(`/employees/${employee.id}`)}
      className="cursor-pointer hover:bg-slate-50"
    >

      <td className="px-6 py-4 font-medium text-slate-700">
        {employee.employeeNumber}
      </td>

      <td className="px-6 py-4 font-medium text-slate-900">
        {employee.firstName} {employee.lastName}
      </td>

      <td className="px-6 py-4 text-slate-600">
        {employee.workEmail ?? "-"}
      </td>

      <td className="px-6 py-4 text-slate-600">
        {employee.mobilePhone ?? "-"}
      </td>

      <td className="px-6 py-4 text-slate-600">
        {new Date(employee.hireDate).toLocaleDateString("pt-PT")}
      </td>

      <td className="px-6 py-4">

        {(() => {
          const status = getStatusInfo(employee.status);

          return (
            <span
              className={`rounded-full px-3 py-1 text-xs font-medium ${status.className}`}
            >
              {status.label}
            </span>
          );
        })()}

      </td>

    </tr>
  ))}

  {employees.length === 0 && (
    <tr>
      <td
        colSpan={6}
        className="px-6 py-12 text-center text-slate-500"
      >
        Não existem funcionários.
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