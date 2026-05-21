import React, { useState } from "react";
import { FcGoogle } from "react-icons/fc";
import { FaEye, FaEyeSlash, FaFacebook, FaLinkedinIn } from "react-icons/fa";
import { signUp } from "../../services/authService.js";

const Signup = ({ handleSignIn }) => {
  const [showPassword, setShowPassword] = useState(false);
  const [selectedUsingFor, setSelectedUsingFor] = useState('Personal');
  const [selectPartner, setSelectPartner] = useState('viada');
  const [selectedFuel, setSelectedFuel] = useState('gasoline');
  const [isLoading, setIsLoading] = useState(false);

  const [formData, setFormData] = useState({
    username: "",
    email: "",
    password: "",
  });

  const handleSelectUsingFor = (e) => {
    setSelectedUsingFor(e.target.value);
  };

  const handleSelectPartner = (e) => {
    setSelectPartner(e.target.value);
  };

  const handleSelectFuel = (e) => {
    setSelectedFuel(e.target.value);
  }

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
      const fullData = { 
        ...formData, 
        usingFor: selectedUsingFor,
        partner: selectedUsingFor === 'Company' ? selectPartner : null,
        fuel: selectedFuel
      };
      
      const response = await signUp(fullData);
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
        <h1 className="text-3xl text-shadow text-white font-bold text-center mb-4">
          Create Your Account
        </h1>
        <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
          
          {/* Username */}
          <div>
            <label htmlFor="username" className="input-label">
              Username
            </label>
            <input
              id="username"
              type="text"
              className="input"
              value={formData.username}
              onChange={handleChange}
              required
            />
          </div>

          {/* Email */}
          <div>
            <label htmlFor="email" className="input-label">
              Email
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
              Password
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
 
          <div className="flex flex-col"> 
            <div>
              <span className="input-label">Naudosiu:</span>
            </div>

              {/* Using For */}
            <div className="flex flex-row items-center justify-around cursor-pointer"> 
              <label htmlFor="personal" className="input-label flex gap-3">
                <input
                  id="personal"
                  name="UsingFor"
                  type="radio"
                  value="Personal"
                  className="gap-2"
                  checked={selectedUsingFor === 'Personal'}
                  onChange={handleSelectUsingFor}
                  required
                /> Asmeniškai
              </label>
               
              <label htmlFor="company" className="input-label flex gap-3">
                <input
                  id="company"
                  name="UsingFor"
                  type="radio"
                  value="Company"
                  checked={selectedUsingFor === 'Company'}
                  onChange={handleSelectUsingFor}
                  required
                /> Įmonė
              </label>
            </div>

            {/* Select Partner */}
            {selectedUsingFor === 'Company' && (
            <div className="text-white flex flex-row items-center justify-between cursor-pointer text-nowrap gap-3">
              <label htmlFor="partner" >Pasirinkite partnerį: </label>

              <select name="selectPartner" id="partner" className="input" value={selectPartner} onChange={handleSelectPartner}>
                <option value="viada" className="text-gray-800">Viada</option>
                <option value="circleK" className="text-gray-800">Circle K</option>
                <option value="emsi" className="text-gray-800">Emsi</option>
                <option value="balticPetroleum" className="text-gray-800">Baltic Petroleum</option>
                <option value="alausa" className="text-gray-800">Alauša</option>
              </select>
              
            </div>
            )}
          </div>

          {/* Select Fuel */}
              
            <div className="text-white flex flex-row items-center justify-between cursor-pointer text-nowrap gap-3">
              <label htmlFor="fuel" className="cursor-pointer">Pasirinkite kuro tipą: </label>

              <select name="selectedFuel" id="fuel" className="input" value={selectedFuel} onChange={handleSelectFuel}>
                <option value="gasoline" className="text-gray-800">Benzinas</option>
                <option value="diesel" className="text-gray-800">Dyzelis</option>
                <option value="gas" className="text-gray-800">Dujos</option>                
              </select>
              
            </div>
          

          <button className="primary-btn cursor-pointer" disabled={isLoading}>
            {isLoading ? "Creating Account..." : "Create Account"}
          </button>
        </form>
        
        {/* Social Login */}
        {/* <p className="text-center text-white text-sm my-3">or login with</p>
        <div className="flex gap-6 justify-center">
          <div className="bg-white w-9 h-9 rounded-full flex items-center justify-center shadow-custom-inset hover:scale-110 transition-all duration-300">
            <FcGoogle className="text-3xl cursor-pointer" />
          </div>
          <div className="bg-blue-600 w-9 h-9 rounded-full flex items-center justify-center shadow-custom-inset hover:scale-110 transition-all duration-300">
            <FaLinkedinIn className="text-2xl text-white cursor-pointer" />
          </div>
          <div className="bg-blue-600 w-9 h-9 rounded-full flex items-center justify-center shadow-custom-inset hover:scale-110 transition-all duration-300">
            <FaFacebook className="text-2xl text-white cursor-pointer" />
          </div>
        </div> */}

        <p
          className="text-center text-white text-sm my-3 hover:text-lime-100 cursor-pointer text-shadow"
          onClick={handleSignIn}
        >
          Already have an Account? Log in
        </p>
      </div>
    </>
  );
};

export default Signup;