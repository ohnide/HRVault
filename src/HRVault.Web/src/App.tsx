import { useState } from "react";
import type { FormEvent } from "react";


import {
  Navigate,
  Route,
  Routes,
  useNavigate,
} from "react-router-dom";

import { api } from "./api/client";
import AppLayout from "./layouts/AppLayout";

import Dashboard from "./pages/Dashboard";

import Employees from "./pages/Employees";
import EmployeeDetails from "./pages/EmployeeDetails";
import EditEmployee from "./pages/EditEmployee";
import NewEmployee from "./pages/NewEmployee";

import Departments from "./pages/Departments";
import DepartmentDetails from "./pages/DepartmentDetails";
import EditDepartment from "./pages/EditDepartment";
import NewDepartment from "./pages/NewDepartment";

import Positions from "./pages/Positions";
import PositionDetails from "./pages/PositionDetails";
import EditPosition from "./pages/EditPosition";
import NewPosition from "./pages/NewPosition";

import Users from "./pages/Users";


import Roles from "./pages/Roles";
import DocumentTypes from "./pages/DocumentTypes";
import AbsenceTypes from "./pages/AbsenceTypes";
import Absences from "./pages/Absences";

import Vacations from "./pages/Vacations";
import Calendar from "./pages/Calendar";
import WorkSchedules from "./pages/WorkSchedules";


function Login() {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();

    setError("");
    setLoading(true);

    try {
      const response = await api.post("/Auth/login", {
        email,
        password,
      });

      const token = response.data.accessToken;

      localStorage.setItem(
        "hrvault_token",
        token
      );

      navigate("/dashboard");
    } catch (error: any) {
      console.error("LOGIN ERROR:", error);

      if (error.response) {
        console.error(
          "STATUS:",
          error.response.status
        );

        console.error(
          "DATA:",
          error.response.data
        );
      }

      setError(
        error.response?.data?.message ??
        error.response?.data?.title ??
        `Erro no login (${error.response?.status ?? "sem resposta"})`
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100">

      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">

        <div className="mb-8 text-center">

          <h1 className="text-3xl font-bold text-slate-900">
            HRVault
          </h1>

          <p className="mt-2 text-sm text-slate-500">
            Gestão de Recursos Humanos
          </p>

        </div>

        <form
          onSubmit={handleSubmit}
          className="space-y-5"
        >

          <div>

            <label className="mb-1 block text-sm font-medium text-slate-700">
              Email
            </label>

            <input
              type="email"
              value={email}
              onChange={(event) =>
                setEmail(event.target.value)
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="admin@hrvault.pt"
              required
            />

          </div>

          <div>

            <label className="mb-1 block text-sm font-medium text-slate-700">
              Password
            </label>

            <input
              type="password"
              value={password}
              onChange={(event) =>
                setPassword(event.target.value)
              }
              className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-500"
              placeholder="••••••••"
              required
            />

          </div>

          {error && (
            <div className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-600">
              {error}
            </div>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full rounded-lg bg-blue-600 px-4 py-3 font-semibold text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {loading
              ? "A entrar..."
              : "Entrar"}
          </button>

        </form>

      </div>

    </div>
  );
}

function App() {
  return (
    <Routes>

      <Route
        path="/login"
        element={<Login />}
      />

      <Route element={<AppLayout />}>

        <Route
          path="/dashboard"
          element={<Dashboard />}
        />

        <Route
          path="/employees"
          element={<Employees />}
        />

        <Route
          path="/absences"
          element={<Absences />}
        />

        <Route
          path="/departments"
          element={<Departments />}
        />

        <Route
          path="/positions"
          element={<Positions />}
        />

        <Route
          path="/users"
          element={<Users />}
        />

        <Route
          path="/roles"
          element={<Roles />}
        />

        <Route
          path="/settings/document-types"
          element={<DocumentTypes />}
        />

        <Route
          path="/settings/absence-types"
          element={<AbsenceTypes />}
        />
		
		<Route
		  path="/employees/new"
		  element={<NewEmployee />}
		/>
		
		<Route
		  path="/employees/:id/edit"
		  element={<EditEmployee />}
		/>
		
		<Route
		  path="/employees/:id"
		  element={<EmployeeDetails />}
		/>
		
		<Route
		  path="/departments/new"
		  element={<NewDepartment />}
		/>

		<Route
		  path="/departments/:id/edit"
		  element={<EditDepartment />}
		/>

		<Route
		  path="/departments/:id"
		  element={<DepartmentDetails />}
		/>

		<Route
		  path="/departments"
		  element={<Departments />}
		/>
		
		<Route
		  path="/positions/new"
		  element={<NewPosition />}
		/>
		
		<Route
		  path="/positions/:id/edit"
		  element={<EditPosition />}
		/>
		
		<Route
		  path="/positions/:id"
		  element={<PositionDetails />}
		/>
		
		<Route
		  path="/positions"
		  element={<Positions />}
		/>

		<Route
		  path="/vacations"
		  element={<Vacations />}
		/>
		
		<Route
		  path="/calendar"
		  element={<Calendar />}
		/>

		<Route
		  path="/work-schedules"
		  element={<WorkSchedules />}
		/>
	
      </Route>

      <Route
        path="*"
        element={
          <Navigate
            to="/dashboard"
            replace
          />
        }
      />

    </Routes>
  );
}

export default App;