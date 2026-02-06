import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';
import { LayoutDashboard, Zap, Droplets, Recycle, BarChart3, Bell, LogOut, User } from 'lucide-react';

const Sidebar = () => {
  const { user, logout } = useAuth();

  const menuItems = [
    { name: 'Dashboard', path: '/', icon: <LayoutDashboard size={20} /> },
    { name: 'Energy', path: '/energy', icon: <Zap size={20} /> },
    { name: 'Water', path: '/water', icon: <Droplets size={20} /> },
    { name: 'Waste', path: '/waste', icon: <Recycle size={20} /> },
    { name: 'Analytics', path: '/analytics', icon: <BarChart3 size={20} /> },
    { name: 'Alerts', path: '/alerts', icon: <Bell size={20} /> },
  ];

  return (
    <aside className="sidebar">
      <div className="sidebar-brand">
        <span>🌱</span> SHSOS
      </div>

      {user && (
        <div style={{ padding: '0 1.5rem 1.5rem', display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <div style={{ width: '40px', height: '40px', borderRadius: '12px', background: 'rgba(255, 107, 0, 0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--primary)' }}>
            <User size={20} />
          </div>
          <div style={{ overflow: 'hidden' }}>
            <div style={{ fontSize: '0.875rem', fontWeight: 600, color: 'var(--text-primary)', whiteSpace: 'nowrap', textOverflow: 'ellipsis' }}>{user.fullName || user.username}</div>
            <div style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>{user.role}</div>
          </div>
        </div>
      )}

      <nav className="sidebar-menu">
        {menuItems.map((item) => (
          <div key={item.name} className="nav-item">
            <NavLink
              to={item.path}
              className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
            >
              {item.icon} {item.name}
            </NavLink>
          </div>
        ))}
      </nav>
      <div className="sidebar-footer" style={{ padding: '1.5rem', borderTop: '1px solid var(--border-color)' }}>
        <button
          className="btn btn-outline-primary"
          style={{ width: '100%', justifyContent: 'flex-start' }}
          onClick={logout}
        >
          <LogOut size={18} /> Logout
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
