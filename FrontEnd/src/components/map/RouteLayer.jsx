import { useEffect, useRef } from "react";
import { useMap } from "react-leaflet";
import L from "leaflet";
import "leaflet-routing-machine";

const RouteLayer = ({ start, end, addedWaypoints = [], onRouteFound }) => {
  const map = useMap();
  const routingControlRef = useRef(null);

  useEffect(() => {
    if (!map || !start || !end) return;

    const points = [
      L.latLng(start[0], start[1]),
      ...addedWaypoints
        .map(wp => {
          const lat = parseFloat(wp.Latitude || wp.latitude);
          const lng = parseFloat(wp.Longitude || wp.longitude);
          return (!isNaN(lat) && !isNaN(lng)) ? L.latLng(lat, lng) : null;
        })
        .filter(wp => wp !== null),
      L.latLng(end[0], end[1])
    ];

    if (routingControlRef.current) {
      try {
        map.removeControl(routingControlRef.current);
      } catch (e) {
      }
    }

    const control = L.Routing.control({
      waypoints: points,
      lineOptions: {
        styles: [{ color: "#5c6bc0", weight: 6 }]
      },
      addWaypoints: false,
      routeWhileDragging: false,
      show: false
    });

    control.on("routesfound", (e) => {
      const routes = e.routes;
      if (routes && routes[0] && onRouteFound) {
        onRouteFound(routes[0].coordinates);
      }
    });

    routingControlRef.current = control;

    const timer = setTimeout(() => {
      if (map && routingControlRef.current) {
        try {
          routingControlRef.current.addTo(map);
        } catch (err) {
          console.warn("Nepavyko pridėti maršruto kontrolerio:", err.message);
        }
      }
    }, 100);

    return () => {
      clearTimeout(timer);

      if (routingControlRef.current) {
        try {
          if (map && map._container) {
            map.removeControl(routingControlRef.current);
          }
        } catch (error) {
        } finally {
          routingControlRef.current = null;
        }
      }
    };
  }, [map, start, end, addedWaypoints, onRouteFound]);

  return null;
};

export default RouteLayer;