import React, { useState, useEffect } from 'react';
import { Recycle, Trash2, Edit2, ShieldCheck, ShieldAlert, Plus } from 'lucide-react';
import api from '../services/api';

const Waste = () => {
    const [data, setData] = useState([]);
    const [departments, setDepartments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [wasteCategory, setWasteCategory] = useState('');

    // Modal State
    const [showModal, setShowModal] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [formData, setFormData] = useState({
        wasteRecordID: 0,
        departmentID: '',
        wasteType: '',
        wasteCategory: 'General',
        wasteWeight: 0,
        segregationStatus: 'Properly Segregated',
        disposalMethod: 'Standard Disposal',
        disposalCost: 0,
        disinfectionCost: 0,
        complianceStatus: 'Compliant',
        collectionDate: new Date().toISOString().split('T')[0]
    });

    const fetchData = async () => {
        setLoading(true);
        try {
            const [wasteRes, deptRes] = await Promise.all([
                api.get('/api/waste', { params: { wasteCategory } }),
                api.get('/api/departments')
            ]);
            setData(wasteRes.data);
            setDepartments(deptRes.data);
        } catch (error) {
            console.error('Error fetching data:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, [wasteCategory]);

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this record?')) {
            try {
                await api.delete(`/api/waste/${id}`);
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
                collectionDate: item.collectionDate.split('T')[0]
            });
        } else {
            setIsEditing(false);
            setFormData({
                wasteRecordID: 0,
                departmentID: departments[0]?.departmentID || '',
                wasteType: '',
                wasteCategory: 'General',
                wasteWeight: 0,
                segregationStatus: 'Properly Segregated',
                disposalMethod: 'Standard Disposal',
                disposalCost: 0,
                disinfectionCost: 0,
                complianceStatus: 'Compliant',
                collectionDate: new Date().toISOString().split('T')[0]
            });
        }
        setShowModal(true);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (isEditing) {
                await api.put(`/api/waste/${formData.wasteRecordID}`, formData);
            } else {
                await api.post('/api/waste', formData);
            }
            setShowModal(false);
            fetchData();
        } catch (error) {
            console.error('Error saving waste record:', error);
            alert('Error saving record. Please check the logs.');
        }
    };

    return (
        <div className="fade-in">
            <div className="row mb-4" style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <h1 style={{ fontWeight: 700, color: 'var(--primary)' }}>♻️ Waste Management</h1>
                    <p style={{ color: 'var(--text-secondary)' }}>Segregation tracking and disposal compliance</p>
                </div>
                <button className="btn btn-primary" onClick={() => handleOpenModal()}><Plus size={18} /> Log New Collection</button>
            </div>

            <div className="card mb-4" style={{ marginBottom: '1.5rem' }}>
                <div className="card-body" style={{ display: 'flex', gap: '2rem', alignItems: 'center' }}>
                    <div style={{ flex: 1, display: 'flex', gap: '1rem', alignItems: 'center' }}>
                        <label className="form-label" style={{ marginBottom: 0 }}>Category Filter:</label>
                        <select
                            className="form-control"
                            style={{ width: '250px' }}
                            value={wasteCategory}
                            onChange={(e) => setWasteCategory(e.target.value)}
                        >
                            <option value="">All Categories</option>
                            <option value="Infectious">Infectious</option>
                            <option value="General">General</option>
                            <option value="Hazardous">Hazardous</option>
                            <option value="Recyclable">Recyclable</option>
                        </select>
                    </div>
                </div>
            </div>

            <div className="card">
                <div className="card-header">Waste Collection Logs</div>
                <div className="card-body" style={{ padding: 0 }}>
                    <div className="table-responsive">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Collection Date</th>
                                    <th>Department</th>
                                    <th>Waste Type</th>
                                    <th>Weight (Kg)</th>
                                    <th>Compliance</th>
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
                                        <tr key={item.wasteRecordID}>
                                            <td>{new Date(item.collectionDate).toLocaleDateString()}</td>
                                            <td><strong>{item.departments?.departmentName || item.departmentID}</strong></td>
                                            <td>
                                                <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{item.wasteCategory}</span>
                                                <div style={{ fontWeight: 600 }}>{item.wasteType}</div>
                                            </td>
                                            <td>{item.wasteWeight} Kg</td>
                                            <td>
                                                {item.complianceStatus === 'Compliant' ? (
                                                    <span style={{ color: '#27ae60', display: 'flex', alignItems: 'center', gap: '0.25rem', fontSize: '0.85rem' }}>
                                                        <ShieldCheck size={14} /> Compliant
                                                    </span>
                                                ) : (
                                                    <span style={{ color: '#e67e22', display: 'flex', alignItems: 'center', gap: '0.25rem', fontSize: '0.85rem' }}>
                                                        <ShieldAlert size={14} /> {item.complianceStatus}
                                                    </span>
                                                )}
                                            </td>
                                            <td>
                                                <div style={{ display: 'flex', gap: '0.5rem' }}>
                                                    <button className="btn btn-outline-primary" style={{ padding: '0.25rem 0.6rem', fontSize: '0.75rem' }} onClick={() => handleOpenModal(item)}><Edit2 size={14} /></button>
                                                    <button
                                                        className="btn btn-outline"
                                                        style={{ padding: '0.25rem 0.6rem', fontSize: '0.75rem', borderColor: '#e74c3c', color: '#e74c3c' }}
                                                        onClick={() => handleDelete(item.wasteRecordID)}
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
                    <div className="card" style={{ width: '600px', maxWidth: '95%', margin: 0 }}>
                        <div className="card-header">
                            <span>{isEditing ? 'Edit Waste Record' : 'Log New Waste Collection'}</span>
                            <button onClick={() => setShowModal(false)} style={{ background: 'none', border: 'none', fontSize: '1.5rem', cursor: 'pointer' }}>&times;</button>
                        </div>
                        <div className="card-body">
                            <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '1rem' }}>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                                    <div>
                                        <label className="form-label">Department</label>
                                        <select className="form-control" value={formData.departmentID} onChange={(e) => setFormData({ ...formData, departmentID: e.target.value })} required>
                                            <option value="">Select Department</option>
                                            {departments.map(d => (
                                                <option key={d.departmentID} value={d.departmentID}>{d.departmentName}</option>
                                            ))}
                                        </select>
                                    </div>
                                    <div>
                                        <label className="form-label">Collection Date</label>
                                        <input type="date" className="form-control" value={formData.collectionDate} onChange={(e) => setFormData({ ...formData, collectionDate: e.target.value })} required />
                                    </div>
                                </div>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                                    <div>
                                        <label className="form-label">Waste Type</label>
                                        <input type="text" className="form-control" value={formData.wasteType} onChange={(e) => setFormData({ ...formData, wasteType: e.target.value })} placeholder="e.g. Plastic, Syringes" required />
                                    </div>
                                    <div>
                                        <label className="form-label">Waste Category</label>
                                        <select className="form-control" value={formData.wasteCategory} onChange={(e) => setFormData({ ...formData, wasteCategory: e.target.value })}>
                                            <option value="General">General</option>
                                            <option value="Infectious">Infectious</option>
                                            <option value="Hazardous">Hazardous</option>
                                            <option value="Recyclable">Recyclable</option>
                                        </select>
                                    </div>
                                </div>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                                    <div>
                                        <label className="form-label">Weight (Kg)</label>
                                        <input type="number" step="0.1" className="form-control" value={formData.wasteWeight} onChange={(e) => setFormData({ ...formData, wasteWeight: e.target.value })} required />
                                    </div>
                                    <div>
                                        <label className="form-label">Compliance Status</label>
                                        <select className="form-control" value={formData.complianceStatus} onChange={(e) => setFormData({ ...formData, complianceStatus: e.target.value })}>
                                            <option value="Compliant">Compliant</option>
                                            <option value="Minor Violation">Minor Violation</option>
                                            <option value="Critical Violation">Critical Violation</option>
                                        </select>
                                    </div>
                                </div>
                                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                                    <div>
                                        <label className="form-label">Disposal Method</label>
                                        <input type="text" className="form-control" value={formData.disposalMethod} onChange={(e) => setFormData({ ...formData, disposalMethod: e.target.value })} />
                                    </div>
                                    <div>
                                        <label className="form-label">Segregation Status</label>
                                        <input type="text" className="form-control" value={formData.segregationStatus} onChange={(e) => setFormData({ ...formData, segregationStatus: e.target.value })} />
                                    </div>
                                </div>
                                <div style={{ marginTop: '1rem', display: 'flex', gap: '1rem' }}>
                                    <button type="submit" className="btn btn-primary" style={{ flex: 1 }}>{isEditing ? 'Update Log' : 'Save Log'}</button>
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

export default Waste;
