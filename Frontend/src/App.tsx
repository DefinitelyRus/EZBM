import { Routes, Route, Navigate } from 'react-router-dom';
import Sidebar from './components/sidebar';
import Dashboard from './pages/Dashboard';
import Inventory from './pages/Inventory';
import Checkout from './pages/Checkout';
import Staff from './pages/Staff';
import Attendance_Logs from './pages/Attendance_Logs';

function App() {
  return (
    <div id="main-pages" className="container-fluid">
      <div className="row">

        {/* SIDE BAR */}
        <Sidebar />

        {/* MAIN CONTENT */}
        <div className="col">
          <Routes>
            <Route path="/" element={<Navigate to="/dashboard" />} />
            <Route path="/dashboard" element={<div><Dashboard/></div>} />
            <Route path="/inventory" element={<div><Inventory/></div>} />
            <Route path="/pos" element={<div><Checkout/></div>} />
            <Route path="/staff" element={<div><Staff/></div>} />
            <Route path="/logs" element={<div><Attendance_Logs/></div>} />
          </Routes>
        </div>

      </div>
    </div>
  );
}

export default App;