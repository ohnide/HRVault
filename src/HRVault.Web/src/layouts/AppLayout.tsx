import { NavLink, Outlet, useNavigate } from "react-router-dom";

const menuItems = [
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

  function logout() {
    localStorage.removeItem("hrvault_token");
    navigate("/login");
  }

  return (
    <div className="flex min-h-screen bg-slate-100">

      {/* Sidebar */}
      <aside className="flex w-64 flex-col bg-slate-900 text-white">

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

            {menuItems.map((item) => (
              <NavLink
                key={item.path}
                to={item.path}
                className={({ isActive }) =>
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
            ))}

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
      <div className="flex min-w-0 flex-1 flex-col">

        {/* Header */}
        <header className="flex h-16 items-center justify-between border-b bg-white px-8">

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