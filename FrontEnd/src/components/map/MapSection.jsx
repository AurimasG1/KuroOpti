import React, { useRef, useEffect } from "react";
import { MapContainer, TileLayer, useMap } from "react-leaflet";
import MarkerClusterGroup from "react-leaflet-cluster";
import L from "leaflet";
import polyline from "@mapbox/polyline";

import "leaflet/dist/leaflet.css";
import 'leaflet.markercluster/dist/MarkerCluster.css';
import 'leaflet.markercluster/dist/MarkerCluster.Default.css';

import RouteLayer from "./RouteLayer";
import StationMarker from "./StationMarker";

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png', // <-- PATAISYTA
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

// ---------------------------
// Polyline Renderer Component
// ---------------------------
const PolylineRenderer = ({ polyline: encoded }) => {
  const map = useMap();
  const lineRef = useRef(null);

  useEffect(() => {
    // Jei polyline išvalytas → pašalinam seną liniją
    if (!encoded) {
      if (lineRef.current) {
        map.removeLayer(lineRef.current);
        lineRef.current = null;
      }
      return;
    }

    try {
      const decoded = polyline.decode(encoded).map(([lat, lng]) => ({
        lat,
        lng,
      }));

      if (lineRef.current) {
        map.removeLayer(lineRef.current);
      }

      const newLine = L.polyline(decoded, {
        color: "#5c6bc0",
        weight: 6,
      }).addTo(map);

      lineRef.current = newLine;

      map.fitBounds(newLine.getBounds());
    } catch (err) {
      console.error("Nepavyko nupiešti polyline:", err);
    }

    return () => {
      if (lineRef.current) {
        map.removeLayer(lineRef.current);
        lineRef.current = null;
      }
    };
  }, [encoded, map]);

  return null;
};

// ---------------------------
// MAIN MAP SECTION
// ---------------------------
const MapSection = ({
  stations = [],
  routePoints = null,
  addedWaypoints = [],
  polyline = null, // <-- nauja
  onRouteFound,
  onAddToRoute,
  onToggleRoute,
}) => {
  const defaultCenter = [54.8985, 23.9036];

  return (
    <div className="h-full w-full">
      <MapContainer
        center={defaultCenter}
        zoom={7}
        className="h-full w-full"
        scrollWheelZoom={true}
      >
        {/* Background map */}
        <TileLayer
          attribution="&copy; OpenStreetMap contributors"
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />

        {/* 1. Jei yra polyline iš istorijos → piešiam jį */}
        {polyline && <PolylineRenderer polyline={polyline} />}

        {/* 2. Jei yra routePoints → gyvas maršrutas */}
        {routePoints && !polyline && (
          <RouteLayer
            start={routePoints.start}
            end={routePoints.end}
            addedWaypoints={addedWaypoints}
            onRouteFound={onRouteFound}
          />
        )}

        {/* 3. Degalinės */}
        <MarkerClusterGroup chunkedLoading maxClusterRadius={50}>
          {stations.map((station) => (
            <StationMarker
              key={station.Id || station.id}
              station={station}
              addedWaypoints={addedWaypoints}
              onToggleRoute={onToggleRoute}
            />
          ))}
        </MarkerClusterGroup>
      </MapContainer>
    </div>
  );
};

export default MapSection;