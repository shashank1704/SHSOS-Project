import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';
import { UserPlus, User, Mail, Lock, AlertCircle, CheckCircle } from 'lucide-react';

const Register = () => {
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        fullName: '',
        password: '',
        confirmPassword: ''
    });
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);
    const [loading, setLoading] = useState(false);
    const { register } = useAuth();
    const navigate = useNavigate();

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        setLoading(true);
        const result = await register(formData);
        if (result.success) {
            setSuccess(true);
            setTimeout(() => navigate('/login'), 2000);
        } else {
            setError(result.message);
            setLoading(false);
        }
    };

    return (
        <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f8f9fa', padding: '2rem' }}>
            <div className="card fade-in" style={{ width: '100%', maxWidth: '450px', padding: '2rem', boxShadow: '0 10px 25px rgba(0,0,0,0.1)', borderRadius: '16px' }}>
                <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
                    <div style={{ background: 'rgba(255, 107, 0, 0.1)', width: '60px', height: '60px', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', margin: '0 auto 1rem' }}>
                        <UserPlus color="var(--primary)" size={30} />
                    </div>
                    <h2 style={{ fontWeight: 700, color: 'var(--text-primary)' }}>Create Account</h2>
                    <p style={{ color: 'var(--text-secondary)' }}>Join SHSOS for better resource management</p>
                </div>

                {error && (
                    <div style={{ background: '#fff1f0', border: '1px solid #ffa39e', color: '#cf1322', padding: '0.75rem', borderRadius: '8px', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem', fontSize: '0.875rem' }}>
                        <AlertCircle size={18} />
                        {error}
                    </div>
                )}

                {success && (
                    <div style={{ background: '#f6ffed', border: '1px solid #b7eb8f', color: '#389e0d', padding: '0.75rem', borderRadius: '8px', marginBottom: '1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem', fontSize: '0.875rem' }}>
                        <CheckCircle size={18} />
                        Registration successful! Redirecting to login...
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    <div className="form-group" style={{ marginBottom: '1rem' }}>
                        <label style={{ display: 'block', marginBottom: '0.4rem', fontSize: '0.875rem', fontWeight: 600 }}>Full Name</label>
                        <input
                            type="text"
                            name="fullName"
                            className="form-control"
                            placeholder="John Doe"
                            value={formData.fullName}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="form-group" style={{ marginBottom: '1rem' }}>
                        <label style={{ display: 'block', marginBottom: '0.4rem', fontSize: '0.875rem', fontWeight: 600 }}>Username</label>
                        <input
                            type="text"
                            name="username"
                            className="form-control"
                            placeholder="johndoe"
                            value={formData.username}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="form-group" style={{ marginBottom: '1rem' }}>
                        <label style={{ display: 'block', marginBottom: '0.4rem', fontSize: '0.875rem', fontWeight: 600 }}>Email Address</label>
                        <input
                            type="email"
                            name="email"
                            className="form-control"
                            placeholder="john@hospital.com"
                            value={formData.email}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginBottom: '1.5rem' }}>
                        <div className="form-group">
                            <label style={{ display: 'block', marginBottom: '0.4rem', fontSize: '0.875rem', fontWeight: 600 }}>Password</label>
                            <input
                                type="password"
                                name="password"
                                className="form-control"
                                placeholder="••••••••"
                                value={formData.password}
                                onChange={handleChange}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <label style={{ display: 'block', marginBottom: '0.4rem', fontSize: '0.875rem', fontWeight: 600 }}>Confirm</label>
                            <input
                                type="password"
                                name="confirmPassword"
                                className="form-control"
                                placeholder="••••••••"
                                value={formData.confirmPassword}
                                onChange={handleChange}
                                required
                            />
                        </div>
                    </div>

                    <button
                        type="submit"
                        className="btn btn-primary"
                        style={{ width: '100%', height: '48px', marginBottom: '1.5rem', fontSize: '1rem', fontWeight: 600 }}
                        disabled={loading || success}
                    >
                        {loading ? 'Creating Account...' : 'Register'}
                    </button>
                </form>

                <div style={{ textAlign: 'center', fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                    Already have an account? <Link to="/login" style={{ color: 'var(--primary)', fontWeight: 600, textDecoration: 'none' }}>Sign in</Link>
                </div>
            </div>
        </div>
    );
};

export default Register;
