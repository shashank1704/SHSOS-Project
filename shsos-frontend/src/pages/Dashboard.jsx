import React, { useState, useEffect } from 'react';
import { Zap, Droplets, Recycle, Globe, TrendingDown, Bell, Building2 } from 'lucide-react';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler } from 'chart.js';
import { Line } from 'react-chartjs-2';
import api from '../services/api';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler);

const Dashboard = () => {
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

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

    if (loading) return <div className="fade-in" style={{ padding: '2rem', textAlign: 'center' }}>Loading dashboard...</div>;
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

    const chartData = {
        labels: data.energyTrendData.map(d => new Date(d.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })),
        datasets: [{
            label: 'Consumption (kWh)',
            data: data.energyTrendData.map(d => d.consumption),
            borderColor: '#ff6b00',
            backgroundColor: 'rgba(255, 107, 0, 0.05)',
            tension: 0.4,
            fill: true,
            pointBackgroundColor: '#ff6b00',
            pointBorderColor: '#fff',
            pointBorderWidth: 2,
            pointRadius: 4,
        }]
    };

    const chartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
            y: { beginAtZero: true, grid: { color: 'rgba(0,0,0,0.05)' } },
            x: { grid: { display: false } }
        }
    };

    return (
        <div className="fade-in">
            {/* Page Header */}
            <div className="row mb-4" style={{ marginBottom: '2rem' }}>
                <div style={{ background: 'white', borderRadius: '16px', padding: '2rem', border: '1px solid var(--border-color)', display: 'flex', justifyContent: 'space-between', alignItems: 'center', boxShadow: 'var(--shadow-sm)' }}>
                    <div>
                        <h1 style={{ fontWeight: 700, color: 'var(--primary)', marginBottom: '0.25rem' }}>🌱 Sustainability Overview</h1>
                        <p style={{ color: 'var(--text-secondary)', marginBottom: 0 }}>Real-time tracking of environmental impact and resource efficiency</p>
                    </div>
                    <div style={{ textAlign: 'right' }}>
                        <div style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', marginBottom: '0.5rem' }}>Overall Score</div>
                        <div style={{ fontSize: '2rem', fontWeight: 700, color: 'var(--success)' }}>
                            {Math.round(data.sustainabilityScores.reduce((acc, s) => acc + s.score, 0) / data.sustainabilityScores.length)}
                            <span style={{ fontSize: '1rem', color: 'var(--text-secondary)' }}>/100</span>
                        </div>
                    </div>
                </div>
            </div>

            {/* Stats Grid */}
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: '1.5rem', marginBottom: '2rem' }}>
                <StatCard icon={<Zap color="#ff6b00" />} label="Total Energy" value={data.totalEnergy} unit="kWh" bg="rgba(255, 107, 0, 0.1)" />
                <StatCard icon={<Droplets color="#3498db" />} label="Total Water" value={data.totalWater} unit="L" bg="rgba(52, 152, 219, 0.1)" />
                <StatCard icon={<Recycle color="#9b59b6" />} label="Waste Generated" value={data.totalWaste} unit="Kg" bg="rgba(155, 89, 182, 0.1)" />
                <StatCard icon={<Globe color="#2ecc71" />} label="Carbon Footprint" value={data.totalCarbon} unit="Kg" bg="rgba(46, 204, 113, 0.1)" />
            </div>

            {/* Secondary Insights */}
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '1.5rem', marginBottom: '2rem' }}>
                <InsightCard icon={<TrendingDown size={24} />} title="Predicted Monthly Cost" value={`₹${data.predictedMonthlyCost}`} />
                <InsightCard icon={<Bell size={24} />} title="Active Alerts" value={data.activeAlerts.length} subValue="Requiring Attention" />
                <InsightCard icon={<Building2 size={24} />} title="Monitored Units" value={data.sustainabilityScores.length} subValue="Active Units" />
            </div>

            {/* Charts Section */}
            <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
                <div className="card">
                    <div className="card-header">
                        <span>📈 Energy Consumption Trend</span>
                        <span style={{ fontSize: '0.8rem', padding: '0.2rem 0.5rem', background: 'rgba(255, 107, 0, 0.1)', color: 'var(--primary)', borderRadius: '4px' }}>Last 30 Days</span>
                    </div>
                    <div className="card-body" style={{ height: '350px' }}>
                        <Line data={chartData} options={chartOptions} />
                    </div>
                </div>

                <div className="card">
                    <div className="card-header">🏥 Unit Efficiency Scores</div>
                    <div className="card-body">
                        {data.sustainabilityScores.map((dept) => (
                            <div key={dept.name} style={{ marginBottom: '1.25rem' }}>
                                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem', alignItems: 'center' }}>
                                    <span style={{ fontSize: '0.9rem', fontWeight: 600 }}>{dept.name}</span>
                                    <span style={{ fontSize: '0.875rem', color: 'var(--primary)', fontWeight: 700 }}>{dept.score}%</span>
                                </div>
                                <div style={{ background: '#f1f2f6', height: '10px', borderRadius: '5px', overflow: 'hidden' }}>
                                    <div style={{ background: 'var(--primary)', height: '100%', width: `${dept.score}%`, borderRadius: '5px', transition: 'width 1s ease-in-out' }}></div>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};

const StatCard = ({ icon, label, value, unit, bg }) => (
    <div className="stat-card">
        <div className="stat-icon" style={{ background: bg }}>{icon}</div>
        <div className="stat-info">
            <span className="label">{label}</span>
            <span className="value">{value}<small style={{ fontSize: '0.8rem', fontWeight: 500 }}> {unit}</small></span>
        </div>
    </div>
);

const InsightCard = ({ icon, title, value, subValue }) => (
    <div className="card" style={{ marginBottom: 0 }}>
        <div className="card-body" style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
            <div style={{ fontSize: '1.5rem', color: 'var(--primary)' }}>{icon}</div>
            <div>
                <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '1px' }}>{title}</div>
                <div style={{ fontSize: '1.25rem', fontWeight: 700 }}>{value} {subValue && <span style={{ fontSize: '0.875rem', fontWeight: 400, color: 'var(--text-secondary)' }}>{subValue}</span>}</div>
            </div>
        </div>
    </div>
);

export default Dashboard;
