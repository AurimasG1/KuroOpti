import ReactDOM from "react-dom/client";
import App from "./App";
import "./styles/index.css";
import { BrowserRouter as Router } from "react-router-dom";
import { StrictMode } from 'react';

ReactDOM.createRoot(document.getElementById("root")).render(
  <StrictMode>
    <Router
      future={{
        v7_startTransition: true,
        v7_relativeSplatPath: true,
      }}
    >
      <App />
    </Router>
  </StrictMode>,
);
