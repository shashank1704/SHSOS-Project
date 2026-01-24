import React from 'react';
import { NavLink } from 'react-router-dom';
import { LayoutDashboard, Zap, Droplets, Recycle, BarChart3, Bell, LogOut } from 'lucide-react';

const Sidebar = () => {
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
        <button className="btn btn-outline-primary" style={{ width: '100%', justifyContent: 'flex-start' }}>
          <LogOut size={18} /> Logout
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
