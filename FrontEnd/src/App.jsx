import React, { useState } from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Navbar from "./components/common/Navbar.jsx";
import HomePage from "./pages/HomePage.jsx";
import LoginPopup from "./pages/LoginPopup.jsx";
import MapPage from "./pages/MapPage.jsx";
import ContactPage from "./pages/ContactPage.jsx";
import Footer from "./components/common/Footer.jsx"
import BgImage from "./assets/images/sunrise.jpg";
import ChatBotPage from "./pages/ChatBotPage.jsx";

const ProtectedRoute = ({ children, user }) => {
  if (!user) {
    return <Navigate to="/" replace />;
  }
  return children;
};

const App = () => {
  const [loginPopup, setLoginPopup] = useState(false);

  const [user, setUser] = useState(null);

  const handleLoginPopup = () => {
    setLoginPopup(!loginPopup);
  };

  const bgImageStyle = {
    width: "100%",
    minHeight: "100vh",
    backgroundImage: `url(${BgImage})`,
    backgroundSize: "cover",
    backgroundPosition: "center",
    backgroundRepeat: "no-repeat",
  };

  return (
    <Router
      future={{
        v7_startTransition: true,
        v7_relativeSplatPath: true,
      }}
    >
      <div style={bgImageStyle} className="relative">
        <Navbar handleLoginPopup={handleLoginPopup} />

        <Routes>
          {/* Pagrindinis puslapis */}
          <Route
            path="/"
            element={<HomePage handleLoginPopup={handleLoginPopup} />}
          />

          {/* Žemėlapio puslapis */}
          <Route
            path="/MapPage"
            element={
              // <ProtectedRoute user={user}>
              <MapPage />
              // </ProtectedRoute>
            }
          />

          {/* Kontaktų puslapis */}
          <Route path="/ContactPage" element={<ContactPage />} />
        </Routes>

        {/* ChatBot */}
        {/* <ChatBotPage /> */}

        {/* Footer */}
        <Footer />

        {/* Login Popup */}
        <LoginPopup
          show={loginPopup}
          onClose={handleLoginPopup}
          setUser={setUser}
          onLoginSuccess={setUser}




        />
      </div>
    </Router>
  );
};

export default App;