import React from 'react';
import Sidebar from './Sidebar';

const Layout = ({ children }) => {
    return (
        <div className="wrapper">
            <Sidebar />
            <main className="main-content">
                {children}
                <footer style={{ marginTop: '4rem', paddingTop: '2rem', borderTop: '1px solid var(--border-color)', color: 'var(--text-secondary)', fontSize: '0.875rem', textAlign: 'center' }}>
                    © 2026 Smart Hospital Sustainability System • Optimizing Resources for a Greener Future
                </footer>
            </main>
        </div>
    );
};

export default Layout;
