import React from "react";
import { FaFacebookF, FaInstagram, FaLinkedin } from "react-icons/fa";
import { Link } from "react-router-dom";

const Footer = () => {
  let currentDate = new Date();
  let year = currentDate.getFullYear();

  return (
    <footer className="bg-gray-800 px-4 py-8 md:px-16 lg:px-28">
      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div>
          <h2 className="text-lg font-bold mb-4 text-white">About Us</h2>
          <p className="text-gray-300">
            We are team dedicated to providing the best products and services to
            our customers
          </p>
        </div>

        <div>
          <h2 className="text-lg font-bold mb-4 text-white">Quick Links</h2>
          <ul>
            <li>
              <Link to="/" className="hover:underline text-gray-300">
                Home
              </Link>
            </li>
            <li>
              <Link to="/MapPage" className="hover:underline text-gray-300">
                Map
              </Link>
            </li>
            <li>
              <Link to="/ContactPage" className="hover:underline text-gray-300">
                Contact
              </Link>
            </li>
          </ul>
        </div>

        <div>
          <h2 className="text-lg font-bold mb-4 text-white">Follow Us</h2>
          <ul className="flex space-x-4 text-white">
            <li>
              <FaFacebookF className="text-blue-500" />
              <a href="https://www.facebook.com/">Facebook</a>
            </li>
            <li>
              <FaLinkedin className="text-sky-700" />
              <a href="https://www.linkedin.com/">LinkedIn</a>
            </li>
            <li>
              <FaInstagram className="text-orange-500" />
              <a href="https://www.instagram.com/">Instagram</a>
            </li>
          </ul>
        </div>
      </div>
      <div className="flex flex-row items-center justify-between border-t border-gray-600 pt-6 text-gray-300 font-semibold text-center mt-6">
        <p></p>
        <p className="">
          © {year}
        </p>
        <p>Version: 0.1.4</p>
      </div>
    </footer>
  );
};

export default Footer;
