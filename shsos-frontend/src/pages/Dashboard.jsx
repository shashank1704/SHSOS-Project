import React, { useState, useEffect } from 'react';
import { Zap, Droplets, Recycle, Globe, TrendingDown, Bell, Building2, BarChart3 } from 'lucide-react';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler } from 'chart.js';
import { Line } from 'react-chartjs-2';
import api from '../services/api';
import { useNavigate } from 'react-router-dom';
import Recommendations from '../components/Recommendations';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler);

const Dashboard = () => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await api.get('/api/dashboard/data');
                setData(response.data);
            } catch (err) {
                console.error('Error fetching dashboard data:', err);
                setError(err.response?.data?.error || err.message || 'Unknown error');
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    if (loading) return (
        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', height: '60vh' }}>
            <div className="spinner" style={{ width: '40px', height: '40px', border: '4px solid rgba(255,107,0,0.1)', borderTopColor: 'var(--primary)', borderRadius: '50%', animation: 'spin 1s linear infinite' }}></div>
            <p style={{ marginTop: '1rem', color: 'var(--text-secondary)' }}>Loading your dashboard...</p>
        </div>
    );

    if (error) return (
        <div className="fade-in" style={{ padding: '2rem', textAlign: 'center', color: 'var(--danger)' }}>
            <h3>Error loading dashboard data</h3>
            <p style={{ marginTop: '1rem', background: '#fff1f0', padding: '1rem', borderRadius: '8px', border: '1px solid #ffa39e' }}>
                {error}
            </p>
            <button className="btn btn-primary" style={{ marginTop: '1rem' }} onClick={() => window.location.reload()}>Retry</button>
        </div>
    );

    if (!data) return <div className="fade-in" style={{ padding: '2rem', textAlign: 'center' }}>No data returned from server.</div>;

    const energyChartData = {
        labels: data.energyTrendData.map(d => new Date(d.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })),
        datasets: [{
            label: 'Energy (kWh)',
            data: data.energyTrendData.map(d => d.consumption),
            borderColor: '#ff6b00',
            backgroundColor: 'rgba(255, 107, 0, 0.05)',
            tension: 0.4,
            fill: true,
            pointRadius: 0,
            pointHoverRadius: 6,
        }]
    };

    const waterChartData = {
        labels: data.waterTrendData.map(d => new Date(d.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })),
        datasets: [{
            label: 'Water (L)',
            data: data.waterTrendData.map(d => d.consumption),
            borderColor: '#3498db',
            backgroundColor: 'rgba(52, 152, 219, 0.05)',
            tension: 0.4,
            fill: true,
            pointRadius: 0,
            pointHoverRadius: 6,
        }]
    };

    const wasteChartData = {
        labels: data.wasteTrendData.map(d => new Date(d.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })),
        datasets: [{
            label: 'Waste (Kg)',
            data: data.wasteTrendData.map(d => d.weight),
            borderColor: '#9b59b6',
            backgroundColor: 'rgba(155, 89, 182, 0.05)',
            tension: 0.4,
            fill: true,
            pointRadius: 0,
            pointHoverRadius: 6,
        }]
    };

    const chartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { display: false },
            tooltip: {
                mode: 'index',
                intersect: false,
                backgroundColor: 'rgba(255, 255, 255, 0.9)',
                titleColor: '#2c3e50',
                bodyColor: '#2c3e50',
                borderColor: '#e1e4e8',
                borderWidth: 1,
                padding: 10,
                displayColors: true,
            }
        },
        scales: {
            y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.05)', drawBorder: false } },
            x: { grid: { display: false }, ticks: { maxTicksLimit: 7 } }
        }
    };

    return (
        <div className="fade-in">
            {/* Page Header */}
            <div className="row mb-4">
                <div style={{ background: 'white', borderRadius: '20px', padding: '2rem', border: '1px solid var(--border-color)', display: 'flex', justifyContent: 'space-between', alignItems: 'center', boxShadow: 'var(--shadow-sm)' }}>
                    <div>
                        <h1 style={{ fontWeight: 800, color: 'var(--text-primary)', marginBottom: '0.25rem', letterSpacing: '-0.5px' }}>🌱 Sustainability Overview</h1>
                        <p style={{ color: 'var(--text-secondary)', marginBottom: 0 }}>Real-time tracking of hospital environmental impact</p>
                    </div>
                    <div style={{ display: 'flex', gap: '2rem', alignItems: 'center' }}>
                        <div style={{ textAlign: 'right' }}>
                            <div style={{ fontSize: '0.75rem', fontWeight: 600, color: 'var(--text-secondary)', textTransform: 'uppercase', marginBottom: '0.25rem' }}>Overall Score</div>
                            <div style={{ fontSize: '2.25rem', fontWeight: 800, color: 'var(--success)', lineHeight: 1 }}>
                                {Math.round(data.sustainabilityScores.reduce((acc, s) => acc + s.score, 0) / data.sustainabilityScores.length)}
                                <span style={{ fontSize: '1rem', color: 'var(--text-secondary)', fontWeight: 500 }}>/100</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Recommendations Section */}
            <div className="row mb-4">
                <div className="col-12">
                    <Recommendations />
                </div>
            </div>

            {/* Stats Grid */}
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: '1.5rem', marginBottom: '2rem' }}>
                <StatCard icon={<Zap color="#ff6b00" />} label="Total Energy" value={data.totalEnergy.toLocaleString()} unit="kWh" bg="rgba(255, 107, 0, 0.1)" trend="+2.4%" />
                <StatCard icon={<Droplets color="#3498db" />} label="Total Water" value={data.totalWater.toLocaleString()} unit="L" bg="rgba(52, 152, 219, 0.1)" trend="-1.2%" />
                <StatCard icon={<Recycle color="#9b59b6" />} label="Waste Generated" value={data.totalWaste.toLocaleString()} unit="Kg" bg="rgba(155, 89, 182, 0.1)" trend="+0.8%" />
                <StatCard icon={<Globe color="#2ecc71" />} label="Carbon Footprint" value={data.totalCarbon.toLocaleString()} unit="Kg" bg="rgba(46, 204, 113, 0.1)" trend="-3.5%" />
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
                {/* Main Trends */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
                    <div className="card" style={{ marginBottom: 0 }}>
                        <div className="card-header" style={{ borderBottom: 'none', paddingBottom: 0 }}>
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', width: '100%' }}>
                                <span style={{ fontWeight: 700 }}>📈 Resource Consumption Trends</span>
                                <div style={{ display: 'flex', gap: '0.5rem' }}>
                                    <span style={{ fontSize: '0.75rem', padding: '0.25rem 0.75rem', background: 'rgba(255, 107, 0, 0.1)', color: '#ff6b00', borderRadius: '20px', fontWeight: 600 }}>Energy</span>
                                    <span style={{ fontSize: '0.75rem', padding: '0.25rem 0.75rem', background: 'rgba(52, 152, 219, 0.1)', color: '#3498db', borderRadius: '20px', fontWeight: 600 }}>Water</span>
                                </div>
                            </div>
                        </div>
                        <div className="card-body" style={{ height: '300px' }}>
                            <Line data={energyChartData} options={chartOptions} />
                        </div>
                    </div>

                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
                        <div className="card" style={{ marginBottom: 0 }}>
                            <div className="card-header" style={{ fontSize: '0.9rem', borderBottom: 'none' }}>💧 Water Usage History</div>
                            <div className="card-body" style={{ height: '180px', padding: '0 1rem 1rem' }}>
                                <Line data={waterChartData} options={chartOptions} />
                            </div>
                        </div>
                        <div className="card" style={{ marginBottom: 0 }}>
                            <div className="card-header" style={{ fontSize: '0.9rem', borderBottom: 'none' }}>♻️ Waste Generation</div>
                            <div className="card-body" style={{ height: '180px', padding: '0 1rem 1rem' }}>
                                <Line data={wasteChartData} options={chartOptions} />
                            </div>
                        </div>
                    </div>
                </div>

                {/* Sidebar Info */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
                    <div className="card" style={{ marginBottom: 0 }}>
                        <div className="card-header" style={{ fontWeight: 700 }}>🏥 Unit Efficiency</div>
                        <div className="card-body" style={{ padding: '1.5rem' }}>
                            {data.sustainabilityScores.map((dept) => (
                                <div key={dept.name} style={{ marginBottom: '1.5rem' }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem', alignItems: 'center' }}>
                                        <span style={{ fontSize: '0.875rem', fontWeight: 600, color: 'var(--text-primary)' }}>{dept.name}</span>
                                        <span style={{ fontSize: '0.875rem', color: 'var(--primary)', fontWeight: 700 }}>{dept.score}%</span>
                                    </div>
                                    <div style={{ background: '#f1f2f6', height: '8px', borderRadius: '4px', overflow: 'hidden' }}>
                                        <div style={{ background: 'var(--primary)', height: '100%', width: `${dept.score}%`, borderRadius: '4px', transition: 'width 1.5s cubic-bezier(0.4, 0, 0.2, 1)' }}></div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>

                    <div className="card" style={{ marginBottom: 0, background: 'var(--primary)', color: 'white' }}>
                        <div className="card-body" style={{ padding: '1.5rem' }}>
                            <div style={{ fontSize: '0.875rem', opacity: 0.9, marginBottom: '0.5rem' }}>Predicted Monthly Cost</div>
                            <div style={{ fontSize: '1.75rem', fontWeight: 800, marginBottom: '1rem' }}>₹{data.predictedMonthlyCost.toLocaleString()}</div>
                            <button className="btn" style={{ background: 'white', color: 'var(--primary)', width: '100%', fontWeight: 700, fontSize: '0.875rem' }} onClick={() => navigate('/analytics')}>View Detailed Analysis</button>
                        </div>
                    </div>
                </div>
            </div>

            {/* Bottom Row */}
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
                <div className="card">
                    <div className="card-header" style={{ fontWeight: 700, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <span>🔔 Recent Alerts</span>
                        <button style={{ background: 'none', border: 'none', color: 'var(--primary)', fontSize: '0.875rem', fontWeight: 600, cursor: 'pointer' }} onClick={() => navigate('/alerts')}>View All</button>
                    </div>
                    <div className="card-body" style={{ padding: 0 }}>
                        {data.activeAlerts.length > 0 ? (
                            data.activeAlerts.map((alert, idx) => (
                                <div key={alert.alertID} style={{ padding: '1rem 1.5rem', display: 'flex', gap: '1rem', borderBottom: idx === data.activeAlerts.length - 1 ? 'none' : '1px solid var(--border-color)' }}>
                                    <div style={{
                                        width: '40px', height: '40px', borderRadius: '50%', flexShrink: 0, display: 'flex', alignItems: 'center', justifyContent: 'center',
                                        background: alert.severity === 'Critical' ? '#fff1f0' : '#fff7e6',
                                        color: alert.severity === 'Critical' ? '#cf1322' : '#d46b08'
                                    }}>
                                        <Bell size={20} />
                                    </div>
                                    <div>
                                        <div style={{ fontWeight: 600, fontSize: '0.9rem', marginBottom: '0.25rem' }}>{alert.message}</div>
                                        <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{new Date(alert.createdAt).toLocaleString()} • {alert.alertType}</div>
                                    </div>
                                </div>
                            ))
                        ) : (
                            <div style={{ padding: '2rem', textAlign: 'center', color: 'var(--text-secondary)' }}>No active alerts</div>
                        )}
                    </div>
                </div>

                <div className="card">
                    <div className="card-header" style={{ fontWeight: 700 }}>⚡ Quick Actions</div>
                    <div className="card-body" style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                        <QuickAction icon={<Zap size={20} />} label="Add Energy Reading" color="#ff6b00" onClick={() => navigate('/energy')} />
                        <QuickAction icon={<Droplets size={20} />} label="Log Water Usage" color="#3498db" onClick={() => navigate('/water')} />
                        <QuickAction icon={<Recycle size={20} />} label="Report Waste" color="#9b59b6" onClick={() => navigate('/waste')} />
                        <QuickAction icon={<BarChart3 size={20} />} label="Generate Report" color="#2ecc71" onClick={() => navigate('/analytics')} />
                    </div>
                </div>
            </div>
        </div>
    );
};

const StatCard = ({ icon, label, value, unit, bg, trend }) => (
    <div className="card" style={{ marginBottom: 0, border: '1px solid var(--border-color)' }}>
        <div className="card-body" style={{ padding: '1.5rem' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '1rem' }}>
                <div style={{ background: bg, width: '48px', height: '48px', borderRadius: '14px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    {icon}
                </div>
                {trend && (
                    <div style={{ fontSize: '0.75rem', fontWeight: 700, padding: '0.25rem 0.5rem', borderRadius: '6px', background: trend.startsWith('+') ? '#f6ffed' : '#fff1f0', color: trend.startsWith('+') ? '#389e0d' : '#cf1322' }}>
                        {trend}
                    </div>
                )}
            </div>
            <div>
                <div style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', fontWeight: 500, marginBottom: '0.25rem' }}>{label}</div>
                <div style={{ fontSize: '1.5rem', fontWeight: 800, color: 'var(--text-primary)' }}>
                    {value} <small style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', fontWeight: 400 }}>{unit}</small>
                </div>
            </div>
        </div>
    </div>
);

const QuickAction = ({ icon, label, color, onClick }) => (
    <div
        onClick={onClick}
        style={{
            padding: '1rem', borderRadius: '12px', background: '#f8f9fa', border: '1px solid #e1e4e8', cursor: 'pointer', transition: 'all 0.2s ease', display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '0.75rem', textAlign: 'center'
        }}
        onMouseEnter={(e) => { e.currentTarget.style.borderColor = color; e.currentTarget.style.background = 'white'; e.currentTarget.style.boxShadow = 'var(--shadow-sm)'; }}
        onMouseLeave={(e) => { e.currentTarget.style.borderColor = '#e1e4e8'; e.currentTarget.style.background = '#f8f9fa'; e.currentTarget.style.boxShadow = 'none'; }}
    >
        <div style={{ color }}>{icon}</div>
        <div style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-primary)' }}>{label}</div>
    </div>
);

export default Dashboard;

