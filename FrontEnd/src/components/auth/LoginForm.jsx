import React, { useState } from "react";
import { FcGoogle } from "react-icons/fc";
import { FaEye, FaEyeSlash, FaFacebook, FaLinkedinIn } from "react-icons/fa";
import { login } from "../../services/authService.js"


const Login = ({ handleSignIn, onLoginSuccess }) => {
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      console.log("Siunčiame duomenis:", { email, password });
      const data = await login(email, password);

      localStorage.setItem("token", data.accessToken);

      console.log("Atsakymas iš BackEnd:", data);

      onLoginSuccess({ email: data.user?.email, token: data.accessToken, });

    } catch (error) {
      alert("Klaida: " + error.message);
    }
  };

  return (
    <>
      <div className={"p-6"}>
        <h1 className="text-3xl text-white font-bold text-center mb-4 text-shadow">
          Login
        </h1>
        <form className="flex flex-col gap-3 " onSubmit={handleSubmit}>
          <div>
            <label for="email" className="input-label">
              email
            </label>
            <input id="email" type="text" className="input" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div>
            <label for="password" className="input-label">
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
                  className="text-white absolute top-1/2 right-3 -translate-y-1/2 cursor-pointer "
                  onClick={() => setShowPassword(!showPassword)}
                />
              ) : (
                <FaEyeSlash
                  className="text-white absolute top-1/2 right-3 -translate-y-1/2 cursor-pointer "
                  onClick={() => setShowPassword(!showPassword)}
                />
              )}
            </div>
          </div>
          <button className="primary-btn cursor-pointer">Submit</button>
        </form>

        {/* <p className="text-center text-white text-sm my-3">or login with</p>
        <div className="flex gap-6 justify-center ">
          <div className="bg-white w-9 h-9 rounded-full flex items-center justify-center shadow-custom-inset hover:scale-110 transition-all duration-300 ">
            <FcGoogle className="text-3xl cursor-pointer" />
          </div>
          <div className="bg-blue-600 w-9 h-9 rounded-full flex items-center justify-center shadow-custom-inset hover:scale-110 transition-all duration-300">
            <FaLinkedinIn className="text-2xl text-white cursor-pointer" />
          </div>
          <div className="bg-blue-600 w-9 h-9 rounded-full flex items-center justify-center shadow-custom-inset hover:scale-110 transition-all duration-300">
            <FaFacebook className="text-2xl text-white items-center cursor-pointer" />
          </div>
        </div> */}
        <p
          className="text-center text-white text-sm my-3 hover:text-lime-100 cursor-pointer text-shadow"
          onClick={handleSignIn}
        >
          No Account? create Signup here
        </p>
      </div>
    </>
  );
};

export default Login;