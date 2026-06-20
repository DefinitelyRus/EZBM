import { Routes, Route, Navigate } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Dashboard from './components/Dashboard';

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
            <Route path="/inventory" element={<div>This is the Inventory Tab</div>} />
            <Route path="/pos" element={<div>This is the POS Tab</div>} />
            <Route path="/staff" element={<div>This is the Staff Tab</div>} />
            <Route path="/logs" element={<div>This is the Logs Tab</div>} />
          </Routes>
        </div>

      </div>
    </div>
  );
}

export default App;