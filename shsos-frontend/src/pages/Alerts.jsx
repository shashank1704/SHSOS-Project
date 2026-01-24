import React, { useState, useEffect } from 'react';
import { Bell, AlertTriangle, Info, CheckCircle } from 'lucide-react';
import api from '../services/api';

const Alerts = () => {
    const [alerts, setAlerts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchAlerts = async () => {
            try {
                const response = await api.get('/api/dashboard/data'); // Using dashboard data for now
                setAlerts(response.data.activeAlerts);
            } catch (error) {
                console.error('Error fetching alerts:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchAlerts();
    }, []);

    return (
        <div className="fade-in">
            <div className="row mb-4" style={{ marginBottom: '2rem' }}>
                <div>
                    <h1 style={{ fontWeight: 700, color: 'var(--primary)' }}>🔔 System Notifications</h1>
                    <p style={{ color: 'var(--text-secondary)' }}>Critical alerts and operational messages</p>
                </div>
            </div>

            <div className="card">
                <div className="card-header">Active Notifications</div>
                <div className="card-body" style={{ padding: 0 }}>
                    <div className="table-responsive">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Severity</th>
                                    <th>Type</th>
                                    <th>Message</th>
                                    <th>Time</th>
                                </tr>
                            </thead>
                            <tbody>
                                {loading ? (
                                    <tr><td colSpan="4" style={{ textAlign: 'center', padding: '2rem' }}>Loading alerts...</td></tr>
                                ) : alerts.length === 0 ? (
                                    <tr><td colSpan="4" style={{ textAlign: 'center', padding: '2rem' }}>No active alerts.</td></tr>
                                ) : (
                                    alerts.map((alert) => (
                                        <tr key={alert.alertID}>
                                            <td>
                                                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                                    <span style={{
                                                        height: '10px',
                                                        width: '10px',
                                                        background: alert.severity === 'Critical' ? '#e74c3c' : '#f1c40f',
                                                        borderRadius: '50%'
                                                    }}></span>
                                                    <span style={{ fontWeight: 700, color: alert.severity === 'Critical' ? '#e74c3c' : '#f1c40f' }}>
                                                        {alert.severity}
                                                    </span>
                                                </div>
                                            </td>
                                            <td><strong>{alert.alertType}</strong></td>
                                            <td>{alert.message}</td>
                                            <td style={{ color: 'var(--text-secondary)' }}>
                                                {new Date(alert.createdAt).toLocaleString()}
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Alerts;
