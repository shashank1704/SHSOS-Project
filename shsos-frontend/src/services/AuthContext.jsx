import React, { createContext, useState, useContext, useEffect } from 'react';
import api from './api';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const initializeAuth = async () => {
            try {
                const storedUser = localStorage.getItem('shsos_user');
                const token = localStorage.getItem('shsos_token');

                if (storedUser && token && storedUser !== "undefined" && storedUser !== "null") {
                    // VERIFY TOKEN VALIDITY BY CALLING A PROTECTED ENDPOINT
                    try {
                        await api.get('/api/departments'); // Lightweight check
                        const parsedUser = JSON.parse(storedUser);
                        if (parsedUser && typeof parsedUser === 'object') {
                            setUser(parsedUser);
                        } else {
                            throw new Error("Invalid user data format");
                        }
                    } catch (tokenErr) {
                        console.warn("Session invalid or expired. Clearing data.");
                        localStorage.removeItem('shsos_user');
                        localStorage.removeItem('shsos_token');
                        setUser(null);
                    }
                } else {
                    setUser(null);
                }
            } catch (e) {
                console.error("Auth Initialization Error:", e);
                setUser(null);
            } finally {
                setLoading(false);
            }
        };
        initializeAuth();
    }, []);

    const login = async (username, password) => {
        try {
            const response = await api.post('/api/auth/login', { username, password });
            const { token, user } = response.data;
            localStorage.setItem('shsos_token', token);
            localStorage.setItem('shsos_user', JSON.stringify(user));
            setUser(user);
            return { success: true };
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Login failed'
            };
        }
    };


    const logout = () => {
        localStorage.removeItem('shsos_token');
        localStorage.removeItem('shsos_user');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, login, logout, loading }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
