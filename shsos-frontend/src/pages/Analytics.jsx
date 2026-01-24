import React, { useState, useEffect } from 'react';
import { BarChart3, TrendingUp, AlertTriangle, CheckCircle } from 'lucide-react';
import api from '../services/api';

const Analytics = () => {
    return (
        <div className="fade-in">
            <div className="row mb-4" style={{ marginBottom: '2rem' }}>
                <div>
                    <h1 style={{ fontWeight: 700, color: 'var(--primary)' }}>📈 System Analytics</h1>
                    <p style={{ color: 'var(--text-secondary)' }}>Deep insights into hospital resource sustainability</p>
                </div>
            </div>

            <div className="card">
                <div className="card-body" style={{ textAlign: 'center', padding: '4rem' }}>
                    <BarChart3 size={64} color="var(--primary)" style={{ marginBottom: '1rem', opacity: 0.5 }} />
                    <h3>Advanced Analytics Coming Soon</h3>
                    <p style={{ color: 'var(--text-secondary)' }}>We are integrating deeper data visualizations for department-wise comparisons.</p>
                </div>
            </div>
        </div>
    );
};

export default Analytics;
