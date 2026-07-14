import { Routes, Route, Navigate } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Dashboard from './pages/Dashboard/Dashboard';
import Inventory from './pages/Inventory/Inventory';
import Checkout from './pages/Checkout/Checkout';
import Staff from './pages/Staff/Staff';
import AttendanceLogs from './pages/AttendanceLogs/AttendanceLogs';

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
            <Route path="/logs" element={<div><AttendanceLogs/></div>} />
          </Routes>
        </div>

      </div>
    </div>
  );
}

export default App;