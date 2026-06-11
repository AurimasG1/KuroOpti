import { useState } from 'react';

export default function RegionAnalytics({ regions, filtered, setFiltered }) {
    const [fuelType, setFuelType] = useState("all");
    const [sortField, setSortField] = useState("Region");
    const [sortDir, setSortDir] = useState("asc");

    const filterData = (type) => {
        setFuelType(type);

        if (type === "all") {
            setFiltered(regions);
            return;
        }

        const key =
            type === "petrol"
                ? "avgPetrol"
                : type === "diesel"
                    ? "avgDiesel"
                    : "avgLpg";

        setFiltered(regions.filter((r) => r[key] > 0));
    };

    const sortData = (field) => {
        const direction = sortDir === "asc" ? "desc" : "asc";
        setSortDir(direction);
        setSortField(field);

        const sorted = [...filtered].sort((a, b) => {
            if (direction === "asc") return a[field] > b[field] ? 1 : -1;
            return a[field] < b[field] ? 1 : -1;
        });

        setFiltered(sorted);
    };

    return (
        <div className="bg-slate-800 p-4 rounded-xl shadow-xl mt-4">
            <h2 className="text-xl font-bold text-lime-500 mb-3">
                Regioninė degalų kainų analitika
            </h2>

            {/* Filtrai */}
            <div className="flex flex-col sm:flex-row gap-3 mb-4">
                <select
                    value={fuelType}
                    onChange={(e) => filterData(e.target.value)}
                    className="bg-slate-700 p-2 rounded"
                >
                    <option value="all">Visi degalai</option>
                    <option value="petrol">Benzinas</option>
                    <option value="diesel">Dyzelinas</option>
                    <option value="lpg">Dujos</option>
                </select>

                <select
                    value={sortField}
                    onChange={(e) => sortData(e.target.value)}
                    className="bg-slate-700 p-2 rounded"
                >
                    <option value="Region">Regionas</option>
                    <option value="avgPetrol">Benzinas</option>
                    <option value="avgDiesel">Dyzelinas</option>
                    <option value="avgLpg">Dujos</option>
                    <option value="count">Degalinių skaičius</option>
                </select>
            </div>

            {/* Lentelė */}
            <table className="w-full text-sm">
                <thead className="text-slate-300 border-b border-slate-600">
                    <tr>
                        <th className="text-left py-2 cursor-pointer" onClick={() => sortData("Region")}>
                            Regionas
                        </th>
                        <th className="text-right cursor-pointer" onClick={() => sortData("avgPetrol")}>
                            Benzinas
                        </th>
                        <th className="text-right cursor-pointer" onClick={() => sortData("avgDiesel")}>
                            Dyzelinas
                        </th>
                        <th className="text-right cursor-pointer" onClick={() => sortData("avgLpg")}>
                            Dujos
                        </th>
                        <th className="text-right cursor-pointer" onClick={() => sortData("count")}>
                            Degalinės
                        </th>
                    </tr>
                </thead>

                <tbody>
                    {filtered.map((r) => (
                        <tr key={r.region} className="border-b border-slate-700">
                            <td className="py-2 text-white font-medium">{r.region}</td>

                            <td className="text-right text-lime-400 font-semibold">
                                {r.avgPetrol} €
                            </td>

                            <td className="text-right text-lime-400 font-semibold">
                                {r.avgDiesel} €
                            </td>

                            <td className="text-right text-lime-400 font-semibold">
                                {r.avgLpg} €
                            </td>

                            <td className="text-right text-slate-300 font-medium">
                                {r.count}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}