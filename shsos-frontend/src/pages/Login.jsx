import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';
import { LogIn, User, Lock, AlertCircle } from 'lucide-react';

const Login = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const { login } = useAuth();
    const navigate = useNavigate();

    console.log("Login Component Rendering");

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);
        console.log("Attempting login with username:", username);
        const result = await login(username, password);
        if (result.success) {
            navigate('/');
        } else {
            setError(result.message);
            setLoading(false);
        }
    };

    return (
        <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f8f9fa' }}>
            <div className="card fade-in" style={{ width: '100%', maxWidth: '400px', padding: '2rem', boxShadow: '0 10px 25px rgba(0,0,0,0.1)', borderRadius: '16px' }}>
                <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
                    <div style={{ background: 'rgba(255, 107, 0, 0.1)', width: '60px', height: '60px', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1rem' }}>
                        <LogIn color="var(--primary)" size={30} />
                    </div>
                    <h2 style={{ fontWeight: 700, color: 'var(--text-primary)' }}>Welcome Back</h2>
                    <p style={{ color: 'var(--text-secondary)' }}>Sign in to SHSOS Dashboard</p>
                </div>

                {error && (
                    <div style={{ background: '#fff1f0', border: '1px solid #ffa39e', color: '#cf1322', padding: '0.75rem', borderRadius: '8px', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem', fontSize: '0.875rem' }}>
                        <AlertCircle size={18} />
                        {error}
                    </div>
                )}

                <div style={{ background: '#e6f7ff', border: '1px solid #91d5ff', padding: '1rem', borderRadius: '8px', marginBottom: '1.5rem', fontSize: '0.8125rem', color: '#0050b3' }}>
                    <div style={{ fontWeight: 600, marginBottom: '0.25rem' }}>Demo Credentials:</div>
                    <div>Username: <strong>admin</strong></div>
                    <div>Password: <strong>admin@123</strong></div>
                </div>

                <form onSubmit={handleSubmit}>
                    <div className="form-group" style={{ marginBottom: '1.25rem' }}>
                        <label style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.875rem', fontWeight: 600 }}>Username</label>
                        <div style={{ position: 'relative' }}>
                            <span style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-secondary)' }}>
                                <User size={18} />
                            </span>
                            <input
                                type="text"
                                className="form-control"
                                style={{ paddingLeft: '40px', height: '48px' }}
                                placeholder="Enter your username"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </div>
                    </div>

                    <div className="form-group" style={{ marginBottom: '1.5rem' }}>
                        <label style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.875rem', fontWeight: 600 }}>Password</label>
                        <div style={{ position: 'relative' }}>
                            <span style={{ position: 'absolute', left: '12px', top: '50%', transform: 'translateY(-50%)', color: 'var(--text-secondary)' }}>
                                <Lock size={18} />
                            </span>
                            <input
                                type="password"
                                className="form-control"
                                style={{ paddingLeft: '40px', height: '48px' }}
                                placeholder="Enter your password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>
                    </div>

                    <button
                        type="submit"
                        className="btn btn-primary"
                        style={{ width: '100%', height: '48px', marginBottom: '1.5rem', fontSize: '1rem', fontWeight: 600 }}
                        disabled={loading}
                    >
                        {loading ? 'Signing in...' : 'Sign In'}
                    </button>
                </form>

            </div>
        </div>
    );
};

export default Login;
