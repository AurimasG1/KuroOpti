import React, { useState } from "react";
import { FcGoogle } from "react-icons/fc";
import { FaEye, FaEyeSlash, FaFacebook, FaLinkedinIn } from "react-icons/fa";
import { login, forgotPassword } from "../../services/authService.js";
import { useNavigate } from "react-router-dom";

const Login = ({ handleSignIn, onLoginSuccess }) => {
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [isForgotPassword, setIsForgotPassword] = useState(false);
  const [loading, setLoading] = useState(false); 
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      console.log("1. Mygtukas paspaustas, siunčiam...");
      const data = await login(email, password);
      console.log("2. Atsakymas iš BackEnd:", data);

      // Tokeno issaugojimas localStorage
      const jwtToken = data?.AccessToken || data?.accessToken || data?.token || data?.user?.token;
      if (jwtToken) {
        localStorage.setItem("token", jwtToken);
        console.log("JWT tokenas išsaugotas localStorage:", jwtToken);
      } else {
        console.warn("Įspėjimas: JWT tokenas nerastas!");
      }

      const backendRole = data?.user?.role || data?.user?.Role || "user";
      localStorage.setItem("accessToken", data.accessToken);
     

      const userData =
        data && data.user && data.user.email
          ? { username: data.user.email, role: backendRole } 
          : { userName: email, role: backendRole }; 

      console.log("3. Paruoštas userData objektas:", userData);
      console.log("4. Ar onLoginSuccess yra funkcija?", typeof onLoginSuccess);

      if (typeof onLoginSuccess === "function") {
        console.log("5. Iškviečiame onLoginSuccess iš LoginForm!");
        onLoginSuccess(userData);
        alert("Prisijungta sėkmingai!");

        if (userData.role === "admin") {
          navigate("/admin");
        } else {
          navigate("/MapPage");
        }
      } else {
        console.error(
          "KLAIDA: onLoginSuccess nėra funkcija LoginForm komponente!",
        );
      }
    } catch (error) {
      console.error("Klaida try/catch bloke:", error);
      alert("Klaida: " + error.message);
    }
  };

  const handleForgotPasswordSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await forgotPassword(email);
      alert(`Instrukcijos išsiųstos į el. paštą: ${email}`);
      setIsForgotPassword(false);
    } catch (error) {
      alert("Klaida: " + error.message);
    } finally {
      setLoading(false);
    }
  };

  if (isForgotPassword) {
    return (
      <div className="p-6">
        <h1 className="text-2xl text-white font-bold text-center mb-4 text-shadow">
          Atkurti slaptažodį
        </h1>
        <form
          className="flex flex-col gap-3"
          onSubmit={handleForgotPasswordSubmit}
        >
          <div>
            <label htmlFor="resetEmail" className="input-label">
              Įveskite savo el. paštą
            </label>
            <input
              id="resetEmail"
              type="email"
              className="input"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              placeholder="pavyzdys@gmail.com"
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="primary-btn cursor-pointer mt-2"
          >
            {loading ? "Siunčiama..." : "Siųsti nuorodą"}
          </button>
        </form>

        <p
          className="text-center text-white text-sm my-4 hover:text-lime-100 cursor-pointer text-shadow underline"
          onClick={() => setIsForgotPassword(false)}
        >
          Grįžti į prisijungimą
        </p>
      </div>
    );
  }

  return (
    <>
      <div className={"p-6"}>
        <h1 className="text-3xl text-white font-bold text-center mb-4 text-shadow">
          Login
        </h1>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          {/* Email */}
          <div>
            <label htmlFor="email" className="input-label">
              Email
            </label>
            <input
              id="email"
              type="email"
              className="input"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          {/* Password */}
          <div>
            <label htmlFor="password" className="input-label">
              Password
            </label>
            <div className="relative">
              <input
                className="input pr-8"
                id="password"
                type={showPassword ? "text" : "password"}
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />

              {showPassword ? (
                <FaEye
                  className="text-white absolute top-1/2 right-3 -translate-y-1/2 cursor-pointer"
                  onClick={() => setShowPassword(!showPassword)}
                />
              ) : (
                <FaEyeSlash
                  className="text-white absolute top-1/2 right-3 -translate-y-1/2 cursor-pointer"
                  onClick={() => setShowPassword(!showPassword)}
                />
              )}
            </div>
          </div>
          {/* Forgot Password */}
          <div className="text-right">
            <span
              onClick={() => setIsForgotPassword(true)}
              className="text-xs text-white/80 hover:text-white cursor-pointer underline text-shadow"
            >
              Pamiršai slaptažodį?
            </span>
          </div>

          <button className="primary-btn cursor-pointer mt-1">Submit</button>
        </form>

        <p
          className="text-center text-white text-sm my-3 hover:text-lime-100 cursor-pointer text-shadow"
          onClick={handleSignIn}
        >
          Neturi paskyros? Užsiregistruok!
        </p>
      </div>
    </>
  );
};

export default Login;
