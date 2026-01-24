import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import Energy from './pages/Energy';
import Water from './pages/Water';
import Waste from './pages/Waste';
import Analytics from './pages/Analytics';
import Alerts from './pages/Alerts';

function App() {
  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/energy" element={<Energy />} />
          <Route path="/water" element={<Water />} />
          <Route path="/waste" element={<Waste />} />
          <Route path="/analytics" element={<Analytics />} />
          <Route path="/alerts" element={<Alerts />} />
        </Routes>
      </Layout>
    </Router>
  );
}

export default App;
