import React, { useState } from "react";
import UsersManagement from "./UsersManagement";
import GasStationsManagement from "./GasStationsManagement";

const API_BASE_URL = "http://localhost:5211/api/admin";

export default function AdminPage() {
  const [activeTab, setActiveTab] = useState("users");

  return (
    <div className="p-6 max-w-5xl mx-auto">
      <h1 className="text-5xl font-bold text-lime-800 mb-6 text-center hover:text-lime-700 transition">
        Admin Page
      </h1>

      <div className="flex gap-3 justify-center mb-6 border-b border-gray-200 pb-4">
        <button
          type="button"
          onClick={() => setActiveTab("users")}
          className={`px-4 py-2 rounded text-sm font-medium transition cursor-pointer ${
            activeTab === "users"
              ? "bg-lime-800 text-white shadow-sm hover:bg-lime-700"
              : "bg-gray-100 text-gray-600 hover:bg-gray-200"
          }`}
        >
          Users Management
        </button>

        <button
          type="button"
          onClick={() => setActiveTab("gasStations")}
          className={`px-4 py-2 rounded text-sm font-medium transition cursor-pointer ${
            activeTab === "gasStations"
              ? "bg-lime-800 text-white shadow-sm hover:bg-lime-700"
              : "bg-gray-100 text-gray-600 hover:bg-gray-200"
          }`}
        >
          Gas Stations Management
        </button>
      </div>
      
      {activeTab === "users" && <UsersManagement apiBaseUrl={API_BASE_URL} />}
      {activeTab === "gasStations" && <GasStationsManagement />}
    </div>
  );
}
