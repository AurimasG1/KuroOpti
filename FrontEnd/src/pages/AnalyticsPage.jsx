import { useEffect, useState } from 'react';
import RegionAnalytics from "../components/analytics/RegionAnalytics";
import { getRegionPrices } from '../services/api.js';

export default function AnalyticsPage() {
    const [regions, setRegions] = useState([]);
    const [filtered, setFiltered] = useState([]);

    useEffect(() => {
        load();
    }, []);

    const load = async () => {
        const data = await getRegionPrices();
        setRegions(data);
        setFiltered(data); // ← svarbiausia dalis
    };

    return (
        <div className="p-4">
            <RegionAnalytics
                regions={regions}
                filtered={filtered}
                setFiltered={setFiltered}
            />
        </div>
    );
}