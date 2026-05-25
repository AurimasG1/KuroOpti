<<<<<<< HEAD
=======

>>>>>>> main
const API_URL = 'http://localhost:5211/api/auth';

export const login = async (email, password) => {
	const response = await fetch(`${API_URL}/login`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ email, password }),
	});

	if (!response.ok) throw new Error('Prisijungimas nepavyko');

	return response.json();
};

export const signUp = async userData => {
	const response = await fetch(`${API_URL}/register`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify(userData),
	});

	if (!response.ok) throw new Error('Registracija nepavyko');

	return response.json();
};
