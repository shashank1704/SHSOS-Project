import React, { useState, useEffect } from 'react';
import { Lightbulb, Info, TrendingDown, ShieldCheck } from 'lucide-react';
import api from '../services/api';

const Recommendations = () => {
    const [recs, setRecs] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchRecs = async () => {
            try {
                const response = await api.get('/api/recommendations');
                setRecs(response.data);
            } catch (error) {
                console.error('Error fetching recommendations:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchRecs();
    }, []);

    if (loading) return <div>Loading insights...</div>;
    if (recs.length === 0) return null;

    const getIcon = (category) => {
        switch (category) {
            case 'Energy': return <TrendingDown size={20} color="var(--primary)" />;
            case 'Water': return <Info size={20} color="#3498db" />;
            case 'Waste': return <ShieldCheck size={20} color="#2ecc71" />;
            default: return <Lightbulb size={20} color="#f1c40f" />;
        }
    };

    return (
        <div className="card" style={{ background: 'rgba(255, 255, 255, 0.05)', backdropFilter: 'blur(10px)', border: '1px solid rgba(255, 255, 255, 0.1)' }}>
            <div className="card-header" style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <Lightbulb size={20} color="#f1c40f" />
                <span>Smart Recommendations</span>
            </div>
            <div className="card-body">
                <div style={{ display: 'grid', gap: '1rem' }}>
                    {recs.map((rec, index) => (
                        <div key={index} style={{
                            padding: '1rem',
                            borderRadius: '12px',
                            background: 'rgba(255, 255, 255, 0.03)',
                            borderLeft: `4px solid ${rec.impact === 'Critical' ? '#e74c3c' : rec.impact === 'High' ? '#f39c12' : '#2ecc71'}`,
                            display: 'flex',
                            gap: '1rem',
                            alignItems: 'start'
                        }}>
                            <div style={{ marginTop: '0.2rem' }}>{getIcon(rec.category)}</div>
                            <div>
                                <h4 style={{ margin: 0, fontSize: '1rem', fontWeight: 600 }}>{rec.title}</h4>
                                <p style={{ margin: '0.25rem 0 0', fontSize: '0.9rem', color: 'var(--text-secondary)' }}>{rec.description}</p>
                                <div style={{ marginTop: '0.5rem', display: 'flex', gap: '0.5rem' }}>
                                    <span style={{ fontSize: '0.75rem', padding: '2px 8px', borderRadius: '4px', background: 'rgba(255,255,255,0.1)' }}>{rec.category}</span>
                                    <span style={{ fontSize: '0.75rem', padding: '2px 8px', borderRadius: '4px', background: 'rgba(255,255,255,0.1)', color: rec.impact === 'Critical' ? '#e74c3c' : '#bdc3c7' }}>{rec.impact} Impact</span>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default Recommendations;
