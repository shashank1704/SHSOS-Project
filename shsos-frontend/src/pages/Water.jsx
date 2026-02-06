import React, { useState, useEffect } from 'react';
import { Droplets, Trash2, Edit2, AlertCircle, Plus, Zap } from 'lucide-react';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler } from 'chart.js';
import { Line } from 'react-chartjs-2';
import api from '../services/api';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler);

const Water = () => {
    const [data, setData] = useState([]);
    const [departments, setDepartments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [leakageOnly, setLeakageOnly] = useState(false);

    // Modal State
    const [showModal, setShowModal] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [formData, setFormData] = useState({
        consumptionID: 0,
        departmentID: '',
        consumptionDate: new Date().toISOString().split('T')[0],
        readingTime: '00:00:00',
        readingEnd: 0,
        unitsConsumedLiters: 0,
        unitCost: 0,
        leakageDetected: false,
        weatherCategory: 'Clear',
        weatherCondition: 'Normal',
        remarks: ''
    });

    const fetchData = async () => {
        setLoading(true);
        try {
            const [waterRes, deptRes] = await Promise.all([
                api.get('/api/water', { params: { leakageOnly } }),
                api.get('/api/departments')
            ]);
            setData(waterRes.data);
            setDepartments(deptRes.data);
        } catch (error) {
            console.error('Error fetching data:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, [leakageOnly]);

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this record?')) {
            try {
                await api.delete(`/api/water/${id}`);
                fetchData();
            } catch (error) {
                console.error('Error deleting record:', error);
            }
        }
    };

    const handleOpenModal = (item = null) => {
        if (item) {
            setIsEditing(true);
            setFormData({
                ...item,
                consumptionDate: item.consumptionDate.split('T')[0]
            });
        } else {
            setIsEditing(false);
            setFormData({
                consumptionID: 0,
                departmentID: departments[0]?.departmentID || '',
                consumptionDate: new Date().toISOString().split('T')[0],
                readingTime: '00:00:00',
                readingEnd: 0,
                unitsConsumedLiters: 0,
                unitCost: 0,
                leakageDetected: false,
                weatherCategory: 'Clear',
                weatherCondition: 'Normal',
                remarks: ''
            });
        }
        setShowModal(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const payload = { ...formData };
        delete payload.departments; // Strip navigation property for API call

        try {
            if (isEditing) {
                await api.put(`/api/water/${payload.consumptionID}`, payload);
            } else {
                await api.post('/api/water', payload);
            }
            setShowModal(false);
            fetchData();
        } catch (error) {
            console.error('Error saving water record:', error);
            alert('Error saving record. Please check the logs.');
        }
    };

    const chartData = {
        labels: data.slice(0, 15).reverse().map(d => new Date(d.consumptionDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })),
        datasets: [{
            label: 'Consumption (Liters)',
            data: data.slice(0, 15).reverse().map(d => d.unitsConsumedLiters),
            borderColor: '#3498db',
            backgroundColor: 'rgba(52, 152, 219, 0.1)',
            tension: 0.4,
            fill: true,
            pointBackgroundColor: '#3498db',
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
            <div className="row mb-4" style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <h1 style={{ fontWeight: 700, color: 'var(--primary)' }}>💧 Water Management</h1>
                    <p style={{ color: 'var(--text-secondary)' }}>Resource monitoring and leakage detection</p>
                </div>
                <button className="btn btn-primary" onClick={() => handleOpenModal()}><Plus size={18} /> Add New Entry</button>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
                <div className="card">
                    <div className="card-header">Consumption Trend</div>
                    <div className="card-body" style={{ height: '300px' }}>
                        {data.length > 0 ? <Line data={chartData} options={chartOptions} /> : <p style={{ textAlign: 'center', padding: '2rem', color: 'var(--text-secondary)' }}>No data for chart</p>}
                    </div>
                </div>

                <div className="card">
                    <div className="card-header">Filters & Options</div>
                    <div className="card-body">
                        <div style={{ display: 'grid', gap: '1.5rem' }}>
                            <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                                <label className="form-label" style={{ marginBottom: 0 }}>Leakage Filter</label>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', padding: '0.75rem', background: '#f8fafc', borderRadius: '8px', border: '1px solid var(--border-color)' }}>
                                    <input
                                        type="checkbox"
                                        id="leakageFilter"
                                        checked={leakageOnly}
                                        onChange={(e) => setLeakageOnly(e.target.checked)}
                                        style={{ width: '18px', height: '18px', cursor: 'pointer' }}
                                    />
                                    <label htmlFor="leakageFilter" style={{ cursor: 'pointer', fontSize: '0.9rem', fontWeight: 500 }}>Show Leakage Only</label>
                                </div>
                            </div>

                            <div>
                                <label className="form-label">Department Lookup</label>
                                <select className="form-control" onChange={(e) => {
                                    // This is just a UI placeholder for now as the current API handles leakageOnly
                                    console.log('Selected dept:', e.target.value);
                                }}>
                                    <option value="">All Departments</option>
                                    {departments.map(d => (
                                        <option key={d.departmentID} value={d.departmentID}>{d.departmentName}</option>
                                    ))}
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div className="card">
                <div className="card-header">Water Consumption History</div>
                <div className="card-body" style={{ padding: 0 }}>
                    <div className="table-responsive">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Department</th>
                                    <th>Consumption (L)</th>
                                    <th>Leakage</th>
                                    <th>Recorded At</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {loading ? (
                                    <tr><td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>Loading...</td></tr>
                                ) : data.length === 0 ? (
                                    <tr><td colSpan="6" style={{ textAlign: 'center', padding: '2rem' }}>No records found.</td></tr>
                                ) : (
                                    data.map((item) => (
                                        <tr key={item.consumptionID}>
                                            <td>{new Date(item.consumptionDate).toLocaleDateString()}</td>
                                            <td><strong>{item.departments?.departmentName || item.departmentID}</strong></td>
                                            <td>{item.unitsConsumedLiters} L</td>
                                            <td>
                                                {item.leakageDetected ? (
                                                    <span style={{ color: '#e74c3c', display: 'flex', alignItems: 'center', gap: '0.25rem', fontWeight: 600 }}>
                                                        <Zap size={14} /> Yes
                                                    </span>
                                                ) : (
                                                    <span style={{ color: '#27ae60' }}>No</span>
                                                )}
                                            </td>
                                            <td style={{ color: 'var(--text-secondary)', fontSize: '0.85rem' }}>
                                                {new Date(item.recordedAt).toLocaleString()}
                                            </td>
                                            <td>
                                                <div style={{ display: 'flex', gap: '0.5rem' }}>
                                                    <button className="btn btn-outline-primary" style={{ padding: '0.25rem 0.6rem', fontSize: '0.75rem' }} onClick={() => handleOpenModal(item)}><Edit2 size={14} /></button>
                                                    <button
                                                        className="btn btn-outline"
                                                        style={{ padding: '0.25rem 0.6rem', fontSize: '0.75rem', borderColor: '#e74c3c', color: '#e74c3c' }}
                                                        onClick={() => handleDelete(item.consumptionID)}
                                                    >
                                                        <Trash2 size={14} />
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            {/* CRUD Modal */}
            {showModal && (
                <div style={{ position: 'fixed', top: 0, left: 0, width: '100%', height: '100%', background: 'rgba(0,0,0,0.5)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 2000 }}>
                    <div className="card" style={{ width: '500px', maxWidth: '90%', margin: 0 }}>
                        <div className="card-header">
                            <span>{isEditing ? 'Edit Water Record' : 'Add New Water Record'}</span>
                            <button onClick={() => setShowModal(false)} style={{ background: 'none', border: 'none', fontSize: '1.5rem', cursor: 'pointer' }}>&times;</button>
                        </div>
                        <div className="card-body">
                            <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '1rem' }}>
                                <div>
                                    <label className="form-label">Department</label>
                                    <select className="form-control" value={formData.departmentID} onChange={(e) => setFormData({ ...formData, departmentID: e.target.value })} required>
                                        <option value="">Select Department</option>
                                        {departments.map(d => (
                                            <option key={d.departmentID} value={d.departmentID}>{d.departmentName}</option>
                                        ))}
                                    </select>
                                </div>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                                    <div>
                                        <label className="form-label">Date</label>
                                        <input type="date" className="form-control" value={formData.consumptionDate} onChange={(e) => setFormData({ ...formData, consumptionDate: e.target.value })} required />
                                    </div>
                                    <div>
                                        <label className="form-label">Consumption (Liters)</label>
                                        <input type="number" className="form-control" value={formData.unitsConsumedLiters} onChange={(e) => setFormData({ ...formData, unitsConsumedLiters: e.target.value })} required />
                                    </div>
                                </div>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                                    <div>
                                        <label className="form-label">Weather Category</label>
                                        <select className="form-control" value={formData.weatherCategory} onChange={(e) => setFormData({ ...formData, weatherCategory: e.target.value })}>
                                            <option value="Clear">Clear</option>
                                            <option value="Cloudy">Cloudy</option>
                                            <option value="Rainy">Rainy</option>
                                            <option value="Stormy">Stormy</option>
                                        </select>
                                    </div>
                                    <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', marginTop: '1.5rem' }}>
                                        <input type="checkbox" checked={formData.leakageDetected} onChange={(e) => setFormData({ ...formData, leakageDetected: e.target.checked })} />
                                        <label className="form-label" style={{ marginBottom: 0 }}>Leakage Detected</label>
                                    </div>
                                </div>
                                <div>
                                    <label className="form-label">Remarks</label>
                                    <textarea className="form-control" value={formData.remarks} onChange={(e) => setFormData({ ...formData, remarks: e.target.value })} rows="2"></textarea>
                                </div>
                                <div style={{ marginTop: '1rem', display: 'flex', gap: '1rem' }}>
                                    <button type="submit" className="btn btn-primary" style={{ flex: 1 }}>{isEditing ? 'Update Record' : 'Save Record'}</button>
                                    <button type="button" className="btn btn-outline" style={{ flex: 1 }} onClick={() => setShowModal(false)}>Cancel</button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Water;
