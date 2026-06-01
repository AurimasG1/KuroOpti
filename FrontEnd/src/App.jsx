import React, { useState } from "react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
  useNavigate
} from "react-router-dom";
import Navbar from "./components/common/Navbar.jsx";
import HomePage from "./pages/HomePage.jsx";
import LoginPopup from "./pages/LoginPopup.jsx";
import MapPage from "./pages/MapPage.jsx";
import ContactPage from "./pages/ContactPage.jsx";
import Footer from "./components/common/Footer.jsx";
import BgImage from "./assets/images/sunrise.jpg";
import ChatBotPage from "./pages/ChatBotPage.jsx";
import AdminPage from "./pages/AdminPage.jsx";

const ProtectedRoute = ({ children, user }) => {
  if (!user) {
    return <Navigate to="/" replace />;
  }
  return children;
};

const AdminProtectedRoute = ({ children, user }) => {
  if (!user || user.role !== "admin") {
    return <Navigate to="/" replace />;
  }
  return children;
};

const App = () => {
 const [loginPopup, setLoginPopup] = useState(false);
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem("app_user");
    try {
      return savedUser ? JSON.parse(savedUser) : null;
    } catch (error) {
      console.error("Nepavyko nuskaityti vartotojo iš localStorage:", error);
      return null;
    }
  }); 

  const navigate = useNavigate();

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
    // <Router
    //   future={{
    //     v7_startTransition: true,
    //     v7_relativeSplatPath: true,
    //   }}
    // >
      <div style={bgImageStyle} className="relative">
        <Navbar
          handleLoginPopup={handleLoginPopup}
          user={user}
          setUser={setUser}
        />

        <Routes>
          {/* Home Page */}
          <Route
            path="/"
            element={<HomePage handleLoginPopup={handleLoginPopup} user={user}/>}
          />


          {/* Žemėlapio puslapis */}
          <Route
            path="/MapPage"
            element={
              <ProtectedRoute user={user}>
                <MapPage />
              </ProtectedRoute>
            } 
          />

          {/* Contact Page */}
          <Route path="/ContactPage" element={<ContactPage />} />

          {/*mano ProtectedAdminRoute kodas*/}
          <Route path="/admin"
            element=
            {
              <AdminProtectedRoute user={user}>
                <AdminPage />
              </AdminProtectedRoute>
            }
          />
        </Routes>

        {/* ChatBot */}
        <ChatBotPage />
        
        {/* Footer */}
        <Footer />

        {/* Login Popup */}
        <LoginPopup
          show={loginPopup}
          onClose={handleLoginPopup}
          onLoginSuccess={(userData) => {
            console.log("Vartotojas sėkmingai išsaugotas App state:", userData); 
            localStorage.setItem("app_user", JSON.stringify(userData));
            
            setUser(userData);

            if (userData && userData.role === "admin"){
              setTimeout(() => {
              navigate("/admin");
              }, 50);
            }
          }}
        />
      </div>
    // </Router>
  );
};

export default App;
