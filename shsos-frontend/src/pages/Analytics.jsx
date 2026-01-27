import React, { useState, useEffect } from 'react';
import { BarChart3, TrendingUp, AlertTriangle, CheckCircle, Zap, Droplets, Trash2, Home, BarChart, PieChart, Activity, DollarSign } from 'lucide-react';
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    BarElement,
    RadialLinearScale,
    ArcElement,
    Title,
    Tooltip,
    Legend,
    Filler
} from 'chart.js';
import { Line, Bar, Doughnut, Radar } from 'react-chartjs-2';
import api from '../services/api';
import Recommendations from '../components/Recommendations';

ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    BarElement,
    RadialLinearScale,
    ArcElement,
    Title,
    Tooltip,
    Legend,
    Filler
);

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

    if (loading) return (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '60vh' }}>
            <div className="spinner"></div>
        </div>
    );
    if (!data) return <div className="p-5 text-center">Error loading analytics data. Please try again later.</div>;

    const labels = data.trends.energy.map(t => new Date(t.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));

    const resourceTrendData = {
        labels,
        datasets: [
            {
                label: 'Energy (kWh)',
                data: data.trends.energy.map(t => t.value),
                borderColor: '#ff6b00',
                backgroundColor: 'rgba(255, 107, 0, 0.1)',
                fill: true,
                tension: 0.4,
            },
            {
                label: 'Water (L)',
                data: data.trends.water.map(t => t.value),
                borderColor: '#3498db',
                backgroundColor: 'rgba(52, 152, 219, 0.1)',
                fill: true,
                tension: 0.4,
            }
        ]
    };

    const wasteDistributionData = {
        labels: Object.keys(data.distribution.waste),
        datasets: [{
            data: Object.values(data.distribution.waste),
            backgroundColor: ['#9b59b6', '#3498db', '#e67e22', '#2ecc71', '#e74c3c'],
            borderWidth: 0,
        }]
    };

    const deptEfficiencyData = {
        labels: Object.keys(data.distribution.energy),
        datasets: [
            {
                label: 'Energy Use',
                data: Object.values(data.distribution.energy),
                backgroundColor: 'rgba(255, 107, 0, 0.5)',
                borderColor: '#ff6b00',
                borderWidth: 1,
            },
            {
                label: 'Water Use',
                data: Object.values(data.distribution.water),
                backgroundColor: 'rgba(52, 152, 219, 0.5)',
                borderColor: '#3498db',
                borderWidth: 1,
            }
        ]
    };

    const deptComparisonData = {
        labels: Object.keys(data.distribution.energy),
        datasets: [
            {
                label: 'Energy Efficiency',
                data: Object.values(data.distribution.energy).map(v => 100 - (v / 1000)), // Synthetic score for demo
                backgroundColor: 'rgba(255, 107, 0, 0.2)',
                borderColor: '#ff6b00',
                pointBackgroundColor: '#ff6b00',
            },
            {
                label: 'Water Efficiency',
                data: Object.values(data.distribution.water).map(v => 100 - (v / 500)), // Synthetic score for demo
                backgroundColor: 'rgba(52, 152, 219, 0.2)',
                borderColor: '#3498db',
                pointBackgroundColor: '#3498db',
            }
        ]
    };

    const chartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { position: 'bottom', labels: { usePointStyle: true, padding: 20 } },
            tooltip: { backgroundColor: '#fff', titleColor: '#1a1a1a', bodyColor: '#666', borderColor: '#eee', borderWidth: 1, padding: 12 }
        },
        scales: {
            y: { beginAtZero: true, grid: { color: '#f0f0f0' } },
            x: { grid: { display: false } }
        }
    };

    const radarOptions = {
        ...chartOptions,
        scales: {
            r: {
                angleLines: { display: true, color: '#f0f0f0' },
                grid: { color: '#f0f0f0' },
                suggestedMin: 0,
                suggestedMax: 100,
                ticks: { display: false }
            }
        }
    };

    return (
        <div className="fade-in">
            {/* Header */}
            <div className="mb-4">
                <h1 style={{ fontWeight: 800, fontSize: '2rem', color: 'var(--text-primary)', marginBottom: '0.5rem' }}>📈 Deeper Analytics</h1>
                <p style={{ color: 'var(--text-secondary)' }}>Comprehensive breakdown of environmental and operational efficiency</p>
            </div>

            {/* Square Metrics Grid */}
            <div style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                gap: '1.5rem',
                marginBottom: '2rem'
            }}>
                <SquareStatCard
                    icon={<Zap size={24} />}
                    label="Energy Cost"
                    value={`$${data.metrics.totalEnergyCost.toLocaleString()}`}
                    color="#ff6b00"
                    bg="rgba(255, 107, 0, 0.1)"
                />
                <SquareStatCard
                    icon={<Trash2 size={24} />}
                    label="Waste Cost"
                    value={`$${data.metrics.totalWasteCost.toLocaleString()}`}
                    color="#9b59b6"
                    bg="rgba(155, 89, 182, 0.1)"
                />
                <SquareStatCard
                    icon={<Activity size={24} />}
                    label="Carbon Footprint"
                    value={`${data.metrics.totalCarbon.toLocaleString()}`}
                    unit="Kg"
                    color="#2ecc71"
                    bg="rgba(46, 204, 113, 0.1)"
                />
                <SquareStatCard
                    icon={<TrendingUp size={24} />}
                    label="Efficiency Rate"
                    value="94.2%"
                    color="#3498db"
                    bg="rgba(52, 152, 219, 0.1)"
                />
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
                {/* Main Trend Chart */}
                <div className="card" style={{ marginBottom: 0 }}>
                    <div className="card-header" style={{ display: 'flex', justifyContent: 'space-between' }}>
                        <span>📊 Resource Trends (30 Days)</span>
                    </div>
                    <div className="card-body" style={{ height: '400px' }}>
                        <Line data={resourceTrendData} options={chartOptions} />
                    </div>
                </div>

                {/* Radar Chart Benchmarking */}
                <div className="card" style={{ marginBottom: 0 }}>
                    <div className="card-header">🎯 Efficiency Benchmarking</div>
                    <div className="card-body" style={{ height: '400px' }}>
                        <Radar data={deptComparisonData} options={radarOptions} />
                    </div>
                </div>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
                {/* Waste Distribution */}
                <div className="card" style={{ marginBottom: 0 }}>
                    <div className="card-header">♻️ Waste Distribution</div>
                    <div className="card-body" style={{ height: '350px', display: 'flex', justifyContent: 'center' }}>
                        <Doughnut data={wasteDistributionData} options={{ ...chartOptions, cutout: '70%' }} />
                    </div>
                </div>

                {/* Department Comparison */}
                <div className="card" style={{ marginBottom: 0 }}>
                    <div className="card-header">🏥 Regional Distribution</div>
                    <div className="card-body" style={{ height: '350px' }}>
                        <Bar data={deptEfficiencyData} options={{ ...chartOptions, indexAxis: 'y' }} />
                    </div>
                </div>
            </div>

            <div className="row">
                <div className="col-12">
                    <Recommendations />
                </div>
            </div>
        </div>
    );
};

const SquareStatCard = ({ icon, label, value, unit, color, bg }) => (
    <div className="card" style={{
        aspectRatio: '1/1',
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        textAlign: 'center',
        padding: '1.5rem',
        marginBottom: 0,
        transition: 'transform 0.3s ease',
        cursor: 'default'
    }}
        onMouseEnter={e => e.currentTarget.style.transform = 'translateY(-5px)'}
        onMouseLeave={e => e.currentTarget.style.transform = 'translateY(0)'}
    >
        <div style={{
            background: bg,
            color: color,
            width: '60px',
            height: '60px',
            borderRadius: '18px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            marginBottom: '1.5rem'
        }}>
            {icon}
        </div>
        <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.5px', marginBottom: '0.5rem' }}>
            {label}
        </div>
        <div style={{ fontSize: '2rem', fontWeight: 800, color: 'var(--text-primary)', lineHeight: 1.2 }}>
            {value} {unit && <span style={{ fontSize: '1rem', fontWeight: 500, color: 'var(--text-secondary)' }}>{unit}</span>}
        </div>
    </div>
);

export default Analytics;
