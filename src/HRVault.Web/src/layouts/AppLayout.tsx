import { useEffect, useState } from "react";
import {
  NavLink,
  Outlet,
  useLocation,
  useNavigate,
} from "react-router-dom";

const mainMenuItems = [
  {
    label: "Dashboard",
    path: "/dashboard",
    icon: "⌂",
  },
  {
    label: "Funcionários",
    path: "/employees",
    icon: "👥",
  },
  {
    label: "Ausências",
    path: "/absences",
    icon: "📆",
  },
  {
  label: "Férias",
  path: "/vacations",
  icon: "🏖️",
  },
  {
    label: "Calendário",
    path: "/calendar",
    icon: "🗓️",
  },
];

const settingsMenuItems = [
  {
    label: "Departamentos",
    path: "/departments",
    icon: "🏢",
  },
  {
    label: "Cargos",
    path: "/positions",
    icon: "💼",
  },
  {
    label: "Horários",
    path: "/work-schedules",
    icon: "🕒",
  },
  {
    label: "Tipos de documentos",
    path: "/settings/document-types",
    icon: "📄",
  },
  {
    label: "Tipos de ausência",
    path: "/settings/absence-types",
    icon: "📅",
  },
  {
    label: "Utilizadores",
    path: "/users",
    icon: "👤",
  },
  {
    label: "Roles",
    path: "/roles",
    icon: "🔐",
  },
];

export default function AppLayout() {
  const navigate = useNavigate();
  const location = useLocation();

  const isSettingsRoute =
    settingsMenuItems.some((item) =>
      location.pathname.startsWith(
        item.path
      )
    );

  const [settingsOpen, setSettingsOpen] =
    useState(isSettingsRoute);

  useEffect(() => {
    if (isSettingsRoute) {
      setSettingsOpen(true);
    }
  }, [isSettingsRoute]);

  function logout() {
    localStorage.removeItem(
      "hrvault_token"
    );

    navigate("/login");
  }

  return (
    <div className="min-h-screen bg-slate-100">
      {/* Sidebar */}
      <aside className="fixed inset-y-0 left-0 z-30 flex w-64 flex-col bg-slate-900 text-white">
        {/* Logo */}
        <div className="border-b border-slate-800 px-6 py-5">
          <h1 className="text-2xl font-bold">
            HRVault
          </h1>

          <p className="mt-1 text-xs text-slate-400">
            Gestão de Recursos Humanos
          </p>
        </div>

        {/* Menu */}
        <nav className="flex-1 px-3 py-5">
          <p className="mb-3 px-3 text-xs font-semibold uppercase tracking-wider text-slate-500">
            Menu
          </p>

          <div className="space-y-1">
            {mainMenuItems.map(
              (item) => (
                <NavLink
                  key={item.path}
                  to={item.path}
                  className={({
                    isActive,
                  }) =>
                    `flex items-center gap-3 rounded-lg px-3 py-3 text-sm font-medium transition ${
                      isActive
                        ? "bg-blue-600 text-white"
                        : "text-slate-300 hover:bg-slate-800 hover:text-white"
                    }`
                  }
                >
                  <span className="w-6 text-center">
                    {item.icon}
                  </span>

                  {item.label}
                </NavLink>
              )
            )}

            {/* Definições */}
            <div className="pt-1">
              <button
                type="button"
                onClick={() =>
                  setSettingsOpen(
                    (current) =>
                      !current
                  )
                }
                className={`flex w-full items-center gap-3 rounded-lg px-3 py-3 text-sm font-medium transition ${
                  isSettingsRoute
                    ? "bg-slate-800 text-white"
                    : "text-slate-300 hover:bg-slate-800 hover:text-white"
                }`}
                aria-expanded={
                  settingsOpen
                }
              >
                <span className="w-6 text-center">
                  ⚙️
                </span>

                <span className="flex-1 text-left">
                  Definições
                </span>

                <span
                  className={`text-xs text-slate-400 transition-transform ${
                    settingsOpen
                      ? "rotate-180"
                      : ""
                  }`}
                >
                  ▼
                </span>
              </button>

              {settingsOpen && (
                <div className="mt-1 space-y-1 border-l border-slate-700 pl-3">
                  {settingsMenuItems.map(
                    (item) => (
                      <NavLink
                        key={
                          item.path
                        }
                        to={
                          item.path
                        }
                        className={({
                          isActive,
                        }) =>
                          `flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition ${
                            isActive
                              ? "bg-blue-600 text-white"
                              : "text-slate-400 hover:bg-slate-800 hover:text-white"
                          }`
                        }
                      >
                        <span className="w-5 text-center text-xs">
                          {
                            item.icon
                          }
                        </span>

                        {
                          item.label
                        }
                      </NavLink>
                    )
                  )}
                </div>
              )}
            </div>
          </div>
        </nav>

        {/* Logout */}
        <div className="border-t border-slate-800 p-3">
          <button
            onClick={logout}
            className="flex w-full items-center gap-3 rounded-lg px-3 py-3 text-sm font-medium text-slate-300 hover:bg-slate-800 hover:text-white"
          >
            <span className="w-6 text-center">
              ↪
            </span>

            Terminar sessão
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className="ml-64 flex min-h-screen min-w-0 flex-col">
        {/* Header */}
        <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b bg-white px-8">
          <div>
            <span className="text-sm text-slate-500">
              Sistema de Recursos Humanos
            </span>
          </div>

          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 items-center justify-center rounded-full bg-blue-100 font-semibold text-blue-700">
              A
            </div>

            <div className="text-right">
              <p className="text-sm font-semibold text-slate-800">
                Administrador
              </p>

              <p className="text-xs text-slate-500">
                Administrador
              </p>
            </div>
          </div>
        </header>

        {/* Content */}
        <main className="flex-1 p-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
