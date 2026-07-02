import './Attendance_Logs.css';

function Attendance_Logs() {
  return (
    <div id="attendance-logs-contents" className="d-flex flex-row gap-4">
      <div id="attendance-logs-content-left" className="d-flex col-9">
      <div id="top-text">
        <h2>Shift Attendance & Payroll Logs</h2>
      </div>
      <div id="card-container" className="d-flex flex-row gap-4" >
        
      </div>
      </div>
      <div id="recents-container" className="d-flex card" style={{ backgroundColor: '#FFFFFF' }}>
        <div className="card-body">
            <h5>Add Attendance Entry</h5>
        </div>
      </div>
    </div>
  );
}

export default Attendance_Logs;