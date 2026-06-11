import React, { useState, useEffect, useRef } from "react";
import { toast } from "react-hot-toast";

export default function UsersManagement({ apiBaseUrl }) {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [editingUser, setEditingUser] = useState(null);
  const [formData, setFormData] = useState({ email: "", role: "" });

  const [isAddingUser, setIsAddingUser] = useState(false);
  const [newUserEmail, setNewUserEmail] = useState("");
  const [newUserPassword, setNewUserPassword] = useState("");
  const [newUserAdminCode, setNewUserAdminCode] = useState("");

  const userFormRef = useRef(null);

  const scrollToForm = () => {
    setTimeout(() => {
      userFormRef.current?.scrollIntoView({
        behavior: "smooth",
        block: "start",
      });
    }, 100);
  };

  const fetchUsers = async () => {
    try {
      setLoading(true);
      setError("");
      let token = localStorage.getItem("token");
      const response = await fetch(`${apiBaseUrl}/users`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (response.status === 401) {
        console.warn("Tokenas pasibaige...");
        setUsers([]);
        return;
      }
      if (!response.ok) throw new Error("Nepavyko užkrauti vartotojų");

      const data = await response.json();
      setUsers(data);
    } catch (error) {
      console.error("Error fetching users:", error);
      setError(error.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleCreateUser = async (e) => {
    e.preventDefault();
    try {
      const authUrl = apiBaseUrl.replace("/admin", "/auth/register");
      const response = await fetch(authUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          Email: newUserEmail,
          Password: newUserPassword,
          AdminCode: newUserAdminCode || null,
        }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Nepavyko sukurti vartotojo");
      }

      toast.success("Vartotojas sėkmingai pridėtas!");
      setNewUserEmail("");
      setNewUserPassword("");
      setNewUserAdminCode("");
      setIsAddingUser(false);
      fetchUsers();
    } catch (error) {
      console.error("Error creating user:", error);
      toast.error("Klaida: " + error.message);
    }
  };

  const handleDelete = async (id, email) => {
    if (
      !window.confirm(
        `Ar tikrai norite ištrinti vartotoją "${email}"? Ir visi su juo susiję duomenys bus ištrinti.`,
      )
    )
      return;
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(`${apiBaseUrl}/users/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!response.ok) throw new Error("Nepavyko ištrinti vartotojo");

      toast.success("Vartotojas ištrintas sėkmingai!");
      fetchUsers();
    } catch (error) {
      console.error("Error deleting user:", error);
      toast.error("Klaida: " + error.message);
    }
  };

  const startEdit = (user) => {
    setEditingUser(user.id);
    setFormData({ email: user.email, role: user.role });
    scrollToForm();
  };

  const handleFormClose = () => {
    setEditingUser(null);
    setFormData({ email: "", role: "" });
  };

  const handleSaveEdit = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("token");
      const payload = {
        Id: editingUser,
        Email: formData.email,
        Role: formData.role,
      };

      const response = await fetch(`${apiBaseUrl}/users/${editingUser}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(
          errorText || `Serveris grąžino kodą ${response.status}`,
        );
      }

      toast.success("Pakeitimai sėkmingai išsaugoti!");
      setEditingUser(null);
      fetchUsers();
    } catch (error) {
      console.error("Error updating user:", error);
      toast.error("Nepavyko išsaugoti: " + error.message);
    }
  };

  if (loading && users.length === 0)
    return (
      <p
        aria-live="polite"
        className="text-center pr-4 text-white drop-shadow-md font-bold bg-transparent"
      >
        Kraunama vartotojų informacija...
      </p>
    );

  if (error)
    return (
      <p
        className="text-center pr-4 text-red-400 drop-shadow-md font-bold bg-transparent"
        role="alert"
      >
        Klaida: {error}
      </p>
    );

  return (
    <div className="bg-transparent">
      <div className="flex justify-between items-center mb-4">
        {!isAddingUser && !editingUser && (
          <button
            type="button"
            onClick={() => {
              setIsAddingUser(true);
              scrollToForm();
            }}
            className="bg-lime-800 hover:bg-lime-700 text-white border-lime-400 border px-4 py-2 rounded text-sm font-bold transition cursor-pointer shadow-md focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
          >
            Pridėti vartotoją
          </button>
        )}
      </div>

      {isAddingUser && (
        <form
          ref={userFormRef}
          onSubmit={handleCreateUser}
          className="bg-gray-900/95 backdrop-blur-md border border-gray-700 p-5 rounded-xl space-y-4 shadow-xl mb-6 text-white"
          aria-label="Naujo vartotojo kūrimo forma"
        >
          <h3 className="text-md font-bold text-lime-400">
            Sukurti Naują Vartotoją
          </h3>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label
                htmlFor="new-email"
                className="block text-xs font-semibold text-gray-200 mb-1"
              >
                Elektroninis paštas *
              </label>
              <input
                id="new-email"
                type="email"
                value={newUserEmail}
                onChange={(e) => setNewUserEmail(e.target.value)}
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
                placeholder="example@mail.com"
                required
                aria-required="true"
              />
            </div>
            <div>
              <label
                htmlFor="new-password"
                className="block text-xs font-semibold text-gray-200 mb-1"
              >
                Slaptažodis *
              </label>
              <input
                id="new-password"
                type="password"
                value={newUserPassword}
                onChange={(e) => setNewUserPassword(e.target.value)}
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
                placeholder="••••••••"
                required
                aria-required="true"
              />
            </div>
            <div>
              <label
                htmlFor="admin-code"
                className="block text-xs font-semibold text-gray-200 mb-1"
              >
                Administratoriaus kodas (tik Admin-ui)
              </label>
              <input
                id="admin-code"
                type="password"
                value={newUserAdminCode}
                onChange={(e) => setNewUserAdminCode(e.target.value)}
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
                placeholder="AdminCode"
              />
            </div>
          </div>
          <div className="flex gap-2 justify-end pt-2">
            <button
              type="button"
              onClick={() => setIsAddingUser(false)}
              className="bg-amber-400 hover:bg-amber-300 text-black px-4 py-1.5 rounded text-sm font-bold transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
            >
              Atšaukti
            </button>
            <button
              type="submit"
              className="bg-lime-600 hover:bg-lime-500 text-white px-4 py-1.5 rounded text-sm font-bold transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
            >
              Sukurti Vartotoją
            </button>
          </div>
        </form>
      )}

      {editingUser && (
        <form
          ref={userFormRef}
          onSubmit={handleSaveEdit}
          className="bg-gray-900/95 backdrop-blur-md border border-gray-700 p-5 rounded-xl space-y-4 shadow-xl mb-6 text-white"
          aria-label="Vartotojo informacijos redagavimo forma"
        >
          <h3 className="text-md font-bold text-lime-400">
            Redaguoti Vartotojo informaciją
          </h3>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label
                htmlFor="edit-email"
                className="block text-xs font-semibold text-gray-200 mb-1"
              >
                Elektroninis paštas *
              </label>
              <input
                id="edit-email"
                type="email"
                value={formData.email}
                onChange={(e) =>
                  setFormData({ ...formData, email: e.target.value })
                }
                className="w-full border border-gray-600 rounded px-3 py-1.5 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
                required
                aria-required="true"
              />
            </div>
            <div>
              <label
                htmlFor="edit-role"
                className="block text-xs font-semibold text-gray-200 mb-1"
              >
                Vartotojo rolė
              </label>
              <select
                id="edit-role"
                value={formData.role}
                onChange={(e) =>
                  setFormData({ ...formData, role: e.target.value })
                }
                className="w-full border border-gray-600 rounded px-3 py-2 text-sm bg-gray-800 text-white focus:outline-none focus:ring-2 focus:ring-lime-500"
              >
                <option value="user">User</option>
                <option value="admin">Admin</option>
              </select>
            </div>
          </div>
          <div className="flex gap-2 justify-end pt-2">
            <button
              type="button"
              onClick={handleFormClose}
              className="bg-amber-400 hover:bg-amber-300 text-black px-4 py-1.5 rounded text-sm font-bold transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
            >
              Atšaukti
            </button>
            <button
              type="submit"
              className="bg-lime-600 hover:bg-lime-500 text-white px-4 py-1.5 rounded text-sm font-bold transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-white"
            >
              Saugoti pakeitimus
            </button>
          </div>
        </form>
      )}

      <div className="overflow-x-auto rounded-xl border border-gray-300 shadow-md bg-white/95 backdrop-blur-sm">
        <table className="min-w-full divide-y divide-gray-300 text-sm">
          <thead className="bg-gray-200/90 text-gray-900 font-extrabold text-left border-b border-gray-300">
            <tr>
              <th scope="col" className="px-4 py-3 w-28">
                Vartotojo ID
              </th>
              <th scope="col" className="px-4 py-3">
                El. paštas
              </th>
              <th scope="col" className="px-4 py-3 w-32">
                Rolė
              </th>
              <th scope="col" className="px-4 py-3 text-center w-36">
                Veiksmai
              </th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200 text-gray-800">
            {users.map((user) => (
              <tr
                key={user.id}
                className="hover:bg-lime-50/50 transition-colors"
              >
                <td
                  className="px-4 py-3 font-mono text-xs text-gray-800 font-semibold truncate max-w-[110px]"
                  title={user.id}
                >
                  {user.id}
                </td>
                <td className="px-4 py-3 font-bold text-gray-900">
                  {user.email}
                </td>
                <td className="px-4 py-3">
                  <span
                    className={`px-2.5 py-0.5 rounded-full text-xs font-black tracking-wider uppercase ${
                      user.role === "admin"
                        ? "bg-purple-200 text-purple-900 border border-purple-300"
                        : "bg-slate-200 text-slate-900 border border-slate-300"
                    }`}
                  >
                    {user.role}
                  </span>
                </td>
                <td className="px-4 py-3 text-center">
                  <div className="flex gap-2 justify-center">
                    <button
                      onClick={() => startEdit(user)}
                      className="text-blue-700 hover:text-blue-900 text-xs font-bold px-2 py-1 hover:bg-blue-100 rounded transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-blue-700"
                      aria-label={`Redaguoti vartotoją ${user.email}`}
                    >
                      Redaguoti
                    </button>
                    <button
                      onClick={() => handleDelete(user.id, user.email)}
                      className="text-red-700 hover:text-red-900 text-xs font-bold px-2 py-1 hover:bg-red-100 rounded transition cursor-pointer focus-visible:outline focus-visible:outline-2 focus-visible:outline-red-700"
                      aria-label={`Ištrinti vartotoją ${user.email}`}
                    >
                      Ištrinti
                    </button>
                  </div>
                </td>
              </tr>
            ))}
            {users.length === 0 && (
              <tr>
                <td
                  colSpan="4"
                  className="text-center py-6 text-gray-600 font-medium italic"
                >
                  Nėra registruotų vartotojų.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
