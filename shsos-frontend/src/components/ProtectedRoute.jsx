import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';

const ProtectedRoute = ({ children }) => {
    const { user, loading } = useAuth();
    const location = useLocation();

    console.log("ProtectedRoute - User:", user, "Loading:", loading);

    if (loading) {
        return (
            <div style={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', height: '100vh', background: '#f8f9fa' }}>
                <div className="spinner" style={{ width: '40px', height: '40px', border: '4px solid #eee', borderTopColor: 'var(--primary)', borderRadius: '50%', animation: 'spin 1s linear infinite' }}></div>
                <p style={{ marginTop: '1rem', color: 'var(--text-secondary)' }}>Verifying session...</p>
            </div>
        );
    }

    if (!user || Object.keys(user).length === 0) {
        console.warn("Unauthorized access attempt redirected to login.");
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return children;
};

export default ProtectedRoute;
