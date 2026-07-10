import { useEffect, useState } from 'react';
import RegionAnalytics from "../components/analytics/RegionAnalytics";
import { getRegionPrices } from '../services/api.js';

export default function AnalyticsPage() {
    const [regions, setRegions] = useState([]);
    const [filtered, setFiltered] = useState([]);

    useEffect(() => {
        let cancelled = false;

        const load = async () => {
            try {
                const data = await getRegionPrices();

                if (!cancelled) {
                    setRegions(data);
                    setFiltered(data);
                }
            } catch (error) {
                console.error(
                    "Failed to load region prices:",
                    error,
                );
            }
        };

        void load();

        return () => {
            cancelled = true;
        };
    }, []);

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