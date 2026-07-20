import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import AppLayout from './components/AppLayout.jsx';
import Home from './pages/Home.jsx';
import Login from './pages/Login.jsx';
import Register from './pages/Register.jsx';
import Upload from './pages/Upload.jsx';
import MyFiles from './pages/MyFiles.jsx';
import Download from './pages/Download.jsx';
import './styles/app.css';

function PrivateRoute({ children }) {
  return localStorage.getItem('token') ? children : <Navigate to="/login" replace />;
}

createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/upload" element={<PrivateRoute><Upload /></PrivateRoute>} />
          <Route path="/files" element={<PrivateRoute><MyFiles /></PrivateRoute>} />
          <Route path="/download/:token" element={<Download />} />
        </Route>
      </Routes>
    </BrowserRouter>
  </React.StrictMode>
);
