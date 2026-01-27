import React, { useState, useEffect } from 'react';
import { BarChart3, TrendingUp, AlertTriangle, CheckCircle, Zap, Droplets, Trash2, Home } from 'lucide-react';
import api from '../services/api';
import Recommendations from '../components/Recommendations';

const Analytics = () => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchAnalytics = async () => {
            try {
                const response = await api.get('/api/analytics/summary');
                setData(response.data);
            } catch (error) {
                console.error('Error fetching analytics:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchAnalytics();
    }, []);

    if (loading) return <div className="p-5">Loading analytics data...</div>;
    if (!data) return <div className="p-5">Error loading analytics.</div>;

    return (
        <div className="fade-in">
            <div className="row mb-4" style={{ marginBottom: '2rem' }}>
                <div>
                    <h1 style={{ fontWeight: 700, color: 'var(--primary)' }}>📈 System Analytics</h1>
                    <p style={{ color: 'var(--text-secondary)' }}>Deep insights into hospital resource sustainability</p>
                </div>
            </div>

            <div className="row mb-4">
                <div className="col-lg-8">
                    <div className="card mb-4">
                        <div className="card-header">Resource Consumption Trends (30 Days)</div>
                        <div className="card-body">
                            {/* In a real app, we'd use Chart.js here. Placeholder for visualization */}
                            <div style={{ height: '300px', background: 'rgba(255,255,255,0.02)', borderRadius: '12px', display: 'flex', alignItems: 'flex-end', gap: '4px', padding: '20px' }}>
                                {data.trends.energy.slice(-20).map((t, i) => (
                                    <div key={i} style={{
                                        flex: 1,
                                        height: `${(t.value / Math.max(...data.trends.energy.map(x => x.value))) * 100}%`,
                                        background: 'var(--primary)',
                                        borderRadius: '2px 2px 0 0',
                                        opacity: 0.7 + (i / 40)
                                    }} title={`${t.date}: ${t.value} kWh`}></div>
                                ))}
                            </div>
                            <div style={{ display: 'flex', justifyContent: 'center', gap: '2rem', marginTop: '1rem' }}>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                    <div style={{ width: '12px', height: '12px', background: 'var(--primary)', borderRadius: '2px' }}></div>
                                    <span style={{ fontSize: '0.85rem' }}>Energy (kWh)</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md-4">
                            <div className="card" style={{ textAlign: 'center' }}>
                                <div className="card-body">
                                    <Zap size={24} color="var(--primary)" style={{ marginBottom: '0.5rem' }} />
                                    <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Total Energy Cost</div>
                                    <h3 style={{ margin: 0 }}>${data.metrics.totalEnergyCost.toLocaleString()}</h3>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card" style={{ textAlign: 'center' }}>
                                <div className="card-body">
                                    <Droplets size={24} color="#3498db" style={{ marginBottom: '0.5rem' }} />
                                    <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Water Usage</div>
                                    <h3 style={{ margin: 0 }}>{data.distribution.water["General Ward"] || 0} L</h3>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card" style={{ textAlign: 'center' }}>
                                <div className="card-body">
                                    <Trash2 size={24} color="#2ecc71" style={{ marginBottom: '0.5rem' }} />
                                    <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>Total Waste Cost</div>
                                    <h3 style={{ margin: 0 }}>${data.metrics.totalWasteCost.toLocaleString()}</h3>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="col-lg-4">
                    <Recommendations />

                    <div className="card mt-4">
                        <div className="card-header">Department Distribution</div>
                        <div className="card-body">
                            {Object.entries(data.distribution.energy).map(([dept, val], i) => (
                                <div key={i} style={{ marginBottom: '1rem' }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.25rem' }}>
                                        <span style={{ fontSize: '0.9rem' }}>{dept}</span>
                                        <span style={{ fontSize: '0.9rem', fontWeight: 600 }}>{val.toLocaleString()} kWh</span>
                                    </div>
                                    <div style={{ height: '6px', background: 'rgba(255,255,255,0.05)', borderRadius: '3px', overflow: 'hidden' }}>
                                        <div style={{
                                            height: '100%',
                                            width: `${(val / Object.values(data.distribution.energy).reduce((a, b) => a + b, 0)) * 100}%`,
                                            background: 'var(--primary)'
                                        }}></div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Analytics;
