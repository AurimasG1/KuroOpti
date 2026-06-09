import React, { useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";

const ResetPassword = () => {
    const [params] = useSearchParams();
    const token = params.get("token");

    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [status, setStatus] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setStatus("");

        try {
            const res = await fetch("http://localhost:5211/api/auth/reset-password", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    token: token,
                    newPassword: password,
                }),
            });

            if (res.ok) {
                setStatus("Slaptažodis sėkmingai pakeistas!");
                setTimeout(() => navigate("/login"), 1500);
            } else {
                setStatus("Klaida keičiant slaptažodį.");
            }
        } catch (err) {
            setStatus("Serveris nepasiekiamas.");
        }

        setLoading(false);
    };

    if (!token) {
        return (
            <div className="p-6 text-center text-white">
                <h1 className="text-2xl font-bold text-shadow mb-4">
                    Token nerastas
                </h1>
                <p className="text-white/80">Patikrinkite nuorodą iš el. pašto.</p>
            </div>
        );
    }

    return (
        <div className="p-6">
            <h1 className="text-2xl text-white font-bold text-center mb-4 text-shadow">
                Naujas slaptažodis
            </h1>

            <form className="flex flex-col gap-3" onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="password" className="input-label">
                        Įveskite naują slaptažodį
                    </label>
                    <input
                        id="password"
                        type="password"
                        className="input"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        placeholder="Naujas slaptažodis"
                    />
                </div>

                <button
                    type="submit"
                    disabled={loading}
                    className="primary-btn cursor-pointer mt-2"
                >
                    {loading ? "Keičiama..." : "Pakeisti slaptažodį"}
                </button>
            </form>

            {status && (
                <p className="text-center text-white mt-4 text-shadow">{status}</p>
            )}

            <p
                className="text-center text-white text-sm my-4 hover:text-lime-100 cursor-pointer text-shadow underline"
                onClick={() => navigate("/login")}
            >
                Grįžti į prisijungimą
            </p>
        </div>
    );
};

export default ResetPassword;
