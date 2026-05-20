import React from "react";
import { MapContainer, TileLayer } from "react-leaflet";
import MarkerClusterGroup from "react-leaflet-cluster"; 
import L from "leaflet"; 
import "leaflet/dist/leaflet.css";
import 'leaflet.markercluster/dist/MarkerCluster.css';
import 'leaflet.markercluster/dist/MarkerCluster.Default.css';

import RouteLayer from "./RouteLayer";
import StationMarker from "./StationMarker";


delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
});

const MapSection = ({
  stations = [],
  routePoints = null,
  addedWaypoints = [],
  onRouteFound,
  onAddToRoute,
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
        <TileLayer
          attribution='&copy; OpenStreetMap contributors'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />

        {routePoints && (
          <RouteLayer
            start={routePoints.start}
            end={routePoints.end}
            addedWaypoints={addedWaypoints}
            onRouteFound={onRouteFound}
          />
        )}

        <MarkerClusterGroup
        
          key={stations.length === 0 ? 'empty' : 'loaded'} 
          chunkedLoading
          maxClusterRadius={50}
        >
          {stations.map((station) => (
            <StationMarker 
              key={station.id} 
              station={station} 
              onAddToRoute={onAddToRoute} 
            />
          ))}
        </MarkerClusterGroup>
      </MapContainer>
    </div>
  );
};

export default MapSection;