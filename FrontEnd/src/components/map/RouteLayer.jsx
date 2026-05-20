import { useEffect, useRef } from "react";
import { useMap } from "react-leaflet";
import L from "leaflet";
import "leaflet-routing-machine";

const RouteLayer = ({ start, end, addedWaypoints = [], onRouteFound }) => {
  const map = useMap();
  const routingControlRef = useRef(null);

  useEffect(() => {
    if (!map || !start || !end) return;

     
    if (routingControlRef.current) {
      map.removeControl(routingControlRef.current);
    }

     
    const points = [
      L.latLng(start[0], start[1]),
      ...addedWaypoints.map(wp => L.latLng(wp.lat, wp.lng)),
      L.latLng(end[0], end[1])
    ];

    const routingControl = L.Routing.control({
      waypoints: points,
      lineOptions: {
        styles: [{ color: "#6366f1", weight: 6, opacity: 0.8 }],
        extendToWaypoints: true,
        missingRouteTolerance: 0
      },
      addWaypoints: false,
      draggableWaypoints: false,
      fitSelectedRoutes: true,
      show: false,
      
      createMarker: (i, waypoint, n) => {
        if (i === 0 || i === n - 1) {
          return L.marker(waypoint.latLng);  
        }
        return null;  
      }
    }).addTo(map);

    routingControl.on('routesfound', (e) => {
      const coords = e.routes[0].coordinates;
      if (onRouteFound) onRouteFound(coords);
    });

    routingControlRef.current = routingControl;
return () => {
        if (map && routingControlRef.current) {
            try {
                map.removeControl(routingControlRef.current);
                routingControlRef.current = null; 
            } catch (error) {
                console.warn("Leaflet išsivalymo įspėjimas (galite ignoruoti):", error.message);
            }
        } 
    }; 
  }, [map, start, end, addedWaypoints]);  

  return null;
};

export default RouteLayer;