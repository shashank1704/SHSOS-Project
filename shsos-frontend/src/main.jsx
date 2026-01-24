import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'

window.onerror = function (msg, url, lineNo, columnNo, error) {
  const root = document.getElementById('root');
  if (root) {
    root.innerHTML = `<div style="padding: 40px; color: #721c24; background: #f8d7da; border: 1px solid #f5c6cb; border-radius: 8px; font-family: sans-serif; margin: 20px;">
      <h2 style="margin-top: 0;">Application Error</h2>
      <p><strong>${msg}</strong></p>
      <div style="font-family: monospace; font-size: 13px; margin-top: 20px; white-space: pre-wrap; background: rgba(0,0,0,0.05); padding: 15px; border-radius: 4px;">${error?.stack || 'No stack trace available'}</div>
      <button onclick="window.location.reload()" style="margin-top: 20px; padding: 10px 20px; background: #721c24; color: white; border: none; border-radius: 4px; cursor: pointer;">Reload Application</button>
    </div>`;
  }
  return false;
};

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
