import React, { useState } from "react";
import { FcGoogle } from "react-icons/fc";
import { FaEye, FaEyeSlash, FaFacebook, FaLinkedinIn } from "react-icons/fa";
import { signUp } from "../../services/authService.js";

const Signup = ({ handleSignIn }) => {
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const handleChange = (e) => {
    const { id, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [id]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    try {
      const cleanData = { 
        email: formData.email,
        password: formData.password
      };
      
      const response = await signUp(cleanData);
      alert("Paskyra sėkmingai sukurta! Dabar galite prisijungti.");
      handleSignIn();
    } catch (error) {
      alert("Registracija nepavyko: " + error.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <div className={"p-6"}>
        <h1 className="text-3xl text-shadow text-white font-bold text-center mb-4 ">
          Registracija
        </h1>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>

          {/* Email */}
          <div>
            <label htmlFor="email" className="input-label">
              Elektroninis paštas
            </label>
            <input 
              id="email"
              type="email"
              className="input"
              value={formData.email}
              onChange={handleChange}
              required 
            />
          </div>

          {/* Password */}
          <div>
            <label htmlFor="password" className="input-label">
              Slaptažodis
            </label>
            <div className="relative">
              <input
                className="input pr-8"
                id="password"
                type={showPassword ? "text" : "password"}
                value={formData.password}
                onChange={handleChange}
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

          <button className="primary-btn cursor-pointer" disabled={isLoading}>
            {isLoading ? "Creating Account..." : "Create Account"}
          </button>
        </form>

        <p
          className="text-center text-white text-sm my-3 hover:text-lime-100 cursor-pointer text-shadow"
          onClick={handleSignIn}
        >
          Jau turi paskyrą? Prisijunk!
        </p>
      </div>
    </>
  );
};

export default Signup;