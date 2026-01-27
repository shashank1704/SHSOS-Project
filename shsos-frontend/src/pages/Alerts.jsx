import React, { useState, useEffect } from 'react';
import { Bell, AlertTriangle, Info, CheckCircle } from 'lucide-react';
import api from '../services/api';

const Alerts = () => {
    const [alerts, setAlerts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchAlerts = async () => {
            try {
                const response = await api.get('/api/alerts');
                setAlerts(response.data);
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
                                    <th>Department</th>
                                    <th>Message</th>
                                    <th>Time</th>
                                    <th>Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                {loading ? (
                                    <tr><td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>Loading alerts...</td></tr>
                                ) : alerts.length === 0 ? (
                                    <tr><td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>No active alerts.</td></tr>
                                ) : (
                                    alerts.map((alert) => (
                                        <tr key={alert.alertID}>
                                            <td>
                                                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                                    <span style={{
                                                        height: '10px',
                                                        width: '10px',
                                                        background: alert.severity === 'Critical' ? '#e74c3c' : alert.severity === 'High' ? '#f39c12' : '#3498db',
                                                        borderRadius: '50%'
                                                    }}></span>
                                                    <span style={{ fontWeight: 700, color: alert.severity === 'Critical' ? '#e74c3c' : alert.severity === 'High' ? '#f39c12' : '#3498db' }}>
                                                        {alert.severity}
                                                    </span>
                                                </div>
                                            </td>
                                            <td><strong>{alert.alertType}</strong></td>
                                            <td><span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{alert.departmentName}</span></td>
                                            <td>{alert.message}</td>
                                            <td style={{ color: 'var(--text-secondary)', fontSize: '0.85rem' }}>
                                                {new Date(alert.createdAt).toLocaleString()}
                                            </td>
                                            <td>
                                                <span style={{
                                                    padding: '2px 8px',
                                                    borderRadius: '12px',
                                                    fontSize: '0.75rem',
                                                    background: alert.isResolved ? 'rgba(46, 204, 113, 0.1)' : 'rgba(231, 76, 60, 0.1)',
                                                    color: alert.isResolved ? '#2ecc71' : '#e74c3c',
                                                    fontWeight: 600
                                                }}>
                                                    {alert.isResolved ? 'Resolved' : 'Active'}
                                                </span>
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
