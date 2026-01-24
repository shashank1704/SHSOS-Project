import React, { useState, useEffect } from 'react';
import { Zap, Plus, Filter, Trash2, Edit2 } from 'lucide-react';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';
import { Bar } from 'react-chartjs-2';
import api from '../services/api';

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

const Energy = () => {
    const [data, setData] = useState([]);
    const [departments, setDepartments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [departmentId, setDepartmentId] = useState('');
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');

    // Modal State
    const [showModal, setShowModal] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [formData, setFormData] = useState({
        energyConsumptionID: 0,
        departmentID: '',
        consumptionDate: new Date().toISOString().split('T')[0],
        readingTime: '00:00:00',
        meterReadingStart: 0,
        unitsConsumedkWh: 0,
        unitCost: 10,
        usageCategory: 'Medical Equipment',
        peakHourFlag: false
    });

    const fetchData = async () => {
        setLoading(true);
        try {
            const [energyRes, deptRes] = await Promise.all([
                api.get('/api/energy', { params: { departmentId, startDate, endDate } }),
                api.get('/api/departments')
            ]);
            setData(energyRes.data);
            setDepartments(deptRes.data);
        } catch (error) {
            console.error('Error fetching data:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, [departmentId, startDate, endDate]);

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this record?')) {
            try {
                await api.delete(`/api/energy/${id}`);
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
                energyConsumptionID: 0,
                departmentID: departments[0]?.departmentID || '',
                consumptionDate: new Date().toISOString().split('T')[0],
                readingTime: '00:00:00',
                unitsConsumedkWh: 0,
                unitCost: 10,
                usageCategory: 'Medical Equipment',
                peakHourFlag: false
            });
        }
        setShowModal(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (isEditing) {
                await api.put(`/api/energy/${formData.energyConsumptionID}`, formData);
            } else {
                await api.post('/api/energy', formData);
            }
            setShowModal(false);
            fetchData();
        } catch (error) {
            console.error('Error saving energy record:', error);
            alert('Error saving record. Please check the logs.');
        }
    };

    const chartData = {
        labels: data.slice(0, 20).reverse().map(d => new Date(d.consumptionDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })),
        datasets: [{
            label: 'Units Consumed (kWh)',
            data: data.slice(0, 20).reverse().map(d => d.unitsConsumedkWh),
            backgroundColor: 'rgba(255, 107, 0, 0.7)',
            borderRadius: 4
        }]
    };

    return (
        <div className="fade-in">
            <div className="row mb-4" style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <h1 style={{ fontWeight: 700, color: 'var(--primary)' }}>⚡ Energy Management</h1>
                    <p style={{ color: 'var(--text-secondary)' }}>Daily consumption tracking and resource optimization</p>
                </div>
                <button className="btn btn-primary" onClick={() => handleOpenModal()}><Plus size={18} /> Add New Entry</button>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
                <div className="card">
                    <div className="card-header">Consumption Trend</div>
                    <div className="card-body" style={{ height: '300px' }}>
                        {data.length > 0 ? <Bar data={chartData} options={{ responsive: true, maintainAspectRatio: false }} /> : <p>No data for chart</p>}
                    </div>
                </div>

                <div className="card">
                    <div className="card-header">Filters</div>
                    <div className="card-body">
                        <div style={{ display: 'grid', gap: '1rem' }}>
                            <div>
                                <label className="form-label">Department</label>
                                <select className="form-control" value={departmentId} onChange={(e) => setDepartmentId(e.target.value)}>
                                    <option value="">All Departments</option>
                                    {departments.map(d => (
                                        <option key={d.departmentID} value={d.departmentID}>{d.departmentName}</option>
                                    ))}
                                </select>
                            </div>
                            <div>
                                <label className="form-label">Start Date</label>
                                <input type="date" className="form-control" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
                            </div>
                            <div>
                                <label className="form-label">End Date</label>
                                <input type="date" className="form-control" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div className="card">
                <div className="card-header">Historical Records</div>
                <div className="card-body" style={{ padding: 0 }}>
                    <div className="table-responsive">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Date</th>
                                    <th>Department</th>
                                    <th>Reading (kWh)</th>
                                    <th>Total Cost</th>
                                    <th>Status</th>
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
                                        <tr key={item.energyConsumptionID}>
                                            <td>{new Date(item.consumptionDate).toLocaleDateString()}</td>
                                            <td><strong>{item.departments?.departmentName || item.departmentID}</strong></td>
                                            <td>{item.unitsConsumedkWh}</td>
                                            <td>₹{Math.round(item.totalCost)}</td>
                                            <td>
                                                <span style={{
                                                    padding: '0.2rem 0.5rem',
                                                    borderRadius: '4px',
                                                    fontSize: '0.75rem',
                                                    background: item.peakHourFlag ? 'rgba(231, 76, 60, 0.1)' : 'rgba(39, 174, 96, 0.1)',
                                                    color: item.peakHourFlag ? '#e74c3c' : '#27ae60'
                                                }}>
                                                    {item.peakHourFlag ? 'Peak' : 'Normal'}
                                                </span>
                                            </td>
                                            <td>
                                                <div style={{ display: 'flex', gap: '0.5rem' }}>
                                                    <button className="btn btn-outline-primary" style={{ padding: '0.25rem 0.6rem', fontSize: '0.75rem' }} onClick={() => handleOpenModal(item)}><Edit2 size={14} /></button>
                                                    <button
                                                        className="btn btn-outline"
                                                        style={{ padding: '0.25rem 0.6rem', fontSize: '0.75rem', borderColor: '#e74c3c', color: '#e74c3c' }}
                                                        onClick={() => handleDelete(item.energyConsumptionID)}
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
                            <span>{isEditing ? 'Edit Energy Record' : 'Add New Energy Record'}</span>
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
                                        <label className="form-label">Consumption (kWh)</label>
                                        <input type="number" className="form-control" value={formData.unitsConsumedkWh} onChange={(e) => setFormData({ ...formData, unitsConsumedkWh: e.target.value })} required />
                                    </div>
                                </div>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                                    <div>
                                        <label className="form-label">Unit Cost (₹)</label>
                                        <input type="number" step="0.01" className="form-control" value={formData.unitCost} onChange={(e) => setFormData({ ...formData, unitCost: e.target.value })} required />
                                    </div>
                                    <div>
                                        <label className="form-label">Category</label>
                                        <select className="form-control" value={formData.usageCategory} onChange={(e) => setFormData({ ...formData, usageCategory: e.target.value })}>
                                            <option value="Medical Equipment">Medical Equipment</option>
                                            <option value="HVAC">HVAC</option>
                                            <option value="Lighting">Lighting</option>
                                            <option value="IT Systems">IT Systems</option>
                                            <option value="Others">Others</option>
                                        </select>
                                    </div>
                                </div>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                                    <input type="checkbox" checked={formData.peakHourFlag} onChange={(e) => setFormData({ ...formData, peakHourFlag: e.target.checked })} />
                                    <label className="form-label" style={{ marginBottom: 0 }}>Peak Hour Consumption</label>
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

export default Energy;
