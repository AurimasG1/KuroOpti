import React, { useState, useEffect } from "react";

export default function UsersManagement({ apiBaseUrl }) {
  const [users, setUsers] = useState([]);
  const [editingUser, setEditingUser] = useState(null);
  const [formData, setFormData] = useState({ email: "", role: "" });

  const [isAddingUser, setIsAddingUser] = useState(false);
  const [newUserEmail, setNewUserEmail] = useState("");
  const [newUserPassword, setNewUserPassword] = useState("");
  const [newUserAdminCode, setNewUserAdminCode] = useState("");

  const fetchUsers = async () => {
    try {
      let token = localStorage.getItem("token");
      const response = await fetch(`${apiBaseUrl}/users`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (response.status === 401) {
        console.warn("Tokenas pasibaige...");
        setUsers([]);
        return;
      }
      if (!response.ok) throw new Error("Failed to fetch users");

      const data = await response.json();
      setUsers(data);
    } catch (error) {
      console.error("Error fetching users:", error);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  // Naujo vartotojo kurimas
  const handleCreateUser = async (e) => {
    e.preventDefault();
    try {
      const authUrl = apiBaseUrl.replace("/admin", "/auth/register");
      const response = await fetch(authUrl, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
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

      alert("Vartotojas sėkmingai pridėtas!");

      setNewUserEmail("");
      setNewUserPassword("");
      setNewUserAdminCode("");
      setIsAddingUser(false);

      fetchUsers(); 
    } catch (error) {
      console.error("Error creating user:", error);
      alert("Klaida: " + error.message);
    }
  };

  const handleDelete = async (id) => {
    if (
      window.confirm(
        "Warning: Deleting this user will also delete everything. Proceed?",
      )
    ) {
      try {
        const token = localStorage.getItem("token");
        await fetch(`${apiBaseUrl}/users/${id}`, {
          method: "DELETE",
          headers: { Authorization: `Bearer ${token}` },
        });
        fetchUsers();
      } catch (error) {
        console.error("Error deleting user:", error);
      }
    }
  };

  const startEdit = (user) => {
    setEditingUser(user.id);
    setFormData({ email: user.email, role: user.role });
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

      console.log("Siunčiami duomenys redagavimui:", payload);

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

      alert("Pakeitimai sėkmingai išsaugoti!");
      setEditingUser(null);
      fetchUsers(); 
    } catch (error) {
      console.error("Error updating user:", error);
      alert("Nepavyko išsaugoti: " + error.message);
    }
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-2xl font-bold text-gray-900 underline">
          Users List
        </h2>
        <button
          type="button"
          onClick={() => setIsAddingUser(!isAddingUser)}
          className="bg-lime-800 hover:bg-lime-700 text-white px-4 py-2 rounded text-sm font-medium transition cursor-pointer"
        >
          {isAddingUser ? "Close form" : "Add new user"}
        </button>
      </div>

      {/* Naujo vartotojo pridejimo forma */}
      {isAddingUser && (
        <form
          onSubmit={handleCreateUser}
          className="mb-6 p-4 bg-lime-50/50 rounded-lg border border-lime-200 shadow-sm"
        >
          <h2 className="text-md font-semibold mb-3 text-lime-800">
            Create New User :
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-3">
            <input
              type="email"
              value={newUserEmail}
              onChange={(e) => setNewUserEmail(e.target.value)}
              className="p-2 border border-gray-300 rounded text-sm bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="User Email"
              required
            />
            <input
              type="password"
              value={newUserPassword}
              onChange={(e) => setNewUserPassword(e.target.value)}
              className="p-2 border border-gray-300 rounded text-sm bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="Temporary Password"
              required
            />
            <input
              type="password"
              value={newUserAdminCode}
              onChange={(e) => setNewUserAdminCode(e.target.value)}
              className="p-2 border border-gray-300 rounded text-sm bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="Admin Code (Optional)"
            />
          </div>
          <div className="flex gap-2 justify-end">
            <button
              type="submit"
              className="bg-lime-800 hover:bg-lime-700 text-white px-4 py-1.5 rounded text-sm font-medium transition cursor-pointer"
            >
              Create User
            </button>
          </div>
        </form>
      )}

      {/* Vartotojo redagavimas */}
      {editingUser && (
        <form
          onSubmit={handleSaveEdit}
          className="mb-6 p-4 bg-gray-50 rounded-lg border border-gray-200 shadow-sm"
        >
          <h2 className="text-md font-semibold mb-3 text-gray-700">
            Modify User Permissions / Email
          </h2>
          <div className="flex gap-4 mb-3">
            <input
              type="email"
              value={formData.email}
              onChange={(e) =>
                setFormData({ ...formData, email: e.target.value })
              }
              className="p-2 border rounded w-full text-sm focus:outline-none focus:ring-1 focus:ring-blue-500"
              required
            />
            <select
              value={formData.role}
              onChange={(e) =>
                setFormData({ ...formData, role: e.target.value })
              }
              className="p-2 border rounded w-full text-sm bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
            >
              <option value="user">User</option>
              <option value="admin">Admin</option>
            </select>
          </div>
          <div className="flex gap-2 justify-end">
            <button
              type="submit"
              className="bg-lime-800 hover:bg-lime-600 text-white px-4 py-1.5 rounded text-sm font-medium transition"
            >
              Save Changes
            </button>
            <button
              type="button"
              onClick={() => setEditingUser(null)}
              className="bg-yellow-300 hover:bg-yellow-400 text-gray-700 px-4 py-1.5 rounded text-sm font-medium transition"
            >
              Cancel
            </button>
          </div>
        </form>
      )}

      {/* USERS TABLE */}
      <div className="overflow-x-auto bg-white shadow rounded-lg border border-gray-200">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50 text-gray-500 text-xs uppercase font-medium">
            <tr>
              <th className="py-3 px-6 text-left">User ID</th>
              <th className="py-3 px-6 text-left">Email Address</th>
              <th className="py-3 px-6 text-left">Role</th>
              <th className="py-3 px-6 text-center">Actions</th>
            </tr>
          </thead>
          <tbody className="text-gray-600 text-sm divide-y divide-gray-100">
            {users.map((user) => (
              <tr key={user.id} className="hover:bg-gray-50/70 transition">
                <td className="py-3 px-6 text-left font-mono text-xs text-gray-400">
                  {user.id}
                </td>
                <td className="py-3 px-6 text-left font-medium text-gray-700">
                  {user.email}
                </td>
                <td className="py-3 px-6 text-left">
                  <span
                    className={`px-2.5 py-0.5 rounded-full text-xs font-semibold ${user.role === "admin" ? "bg-purple-100 text-purple-700" : "bg-slate-100 text-slate-700"}`}
                  >
                    {user.role}
                  </span>
                </td>
                <td className="py-3 px-6 text-center">
                  <div className="flex justify-center gap-2">
                    <button
                      onClick={() => startEdit(user)}
                      className="text-blue-600 hover:text-blue-800 text-xs font-semibold px-2 py-1 hover:bg-blue-50 rounded transition"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(user.id)}
                      className="text-red-600 hover:text-red-800 text-xs font-semibold px-2 py-1 hover:bg-red-50 rounded transition"
                    >
                      Delete
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
