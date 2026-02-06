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
        <div className="card" style={{
            background: 'rgba(255, 255, 255, 0.05)',
            backdropFilter: 'blur(10px)',
            border: '1px solid rgba(255, 255, 255, 0.1)',
            padding: '0.75rem 1rem'
        }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', overflowX: 'auto', whiteSpace: 'nowrap', pb: '5px' }} className="no-scrollbar">
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', borderRight: '1px solid rgba(255,255,255,0.1)', paddingRight: '1rem', flexShrink: 0 }}>
                    <Lightbulb size={18} color="#f1c40f" />
                    <span style={{ fontSize: '0.9rem', fontWeight: 600, color: '#f1c40f' }}>Insights:</span>
                </div>

                <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
                    {recs.map((rec, index) => (
                        <div key={index} style={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: '0.75rem',
                            padding: '0.5rem 1rem',
                            borderRadius: '50px',
                            background: rec.impact === 'Critical' ? 'rgba(231, 76, 60, 0.15)' : 'rgba(255, 255, 255, 0.05)',
                            border: `1px solid ${rec.impact === 'Critical' ? 'rgba(231, 76, 60, 0.3)' : 'rgba(255, 255, 255, 0.1)'}`,
                            flexShrink: 0,
                            animation: 'fadeIn 0.5s ease-out'
                        }}>
                            {getIcon(rec.category)}
                            <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'baseline' }}>
                                <span style={{ fontSize: '0.85rem', fontWeight: 600 }}>{rec.title}:</span>
                                <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>{rec.description}</span>
                            </div>
                            {rec.impact === 'Critical' && (
                                <span style={{
                                    fontSize: '0.65rem',
                                    fontWeight: 800,
                                    textTransform: 'uppercase',
                                    color: '#e74c3c',
                                    background: 'rgba(231, 76, 60, 0.2)',
                                    padding: '2px 6px',
                                    borderRadius: '4px'
                                }}>Urgent</span>
                            )}
                        </div>
                    ))}
                </div>
            </div>
            <style>
                {`
                .no-scrollbar::-webkit-scrollbar { display: none; }
                .no-scrollbar { -ms-overflow-style: none; scrollbar-width: none; }
                @keyframes fadeIn {
                    from { opacity: 0; transform: translateX(10px); }
                    to { opacity: 1; transform: translateX(0); }
                }
                `}
            </style>
        </div>
    );
};

export default Recommendations;
