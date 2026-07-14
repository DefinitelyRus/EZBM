import './AttendanceLogs.css';

function AttendanceLogs() {
  return (
    <div id="attendance-logs-contents" className="d-flex flex-row gap-4">
      <div id="attendance-logs-content-left" className="d-flex col-9">
      <div id="top-text">
        <h2>Attendance & Payroll Logs</h2>
       <h5>Keep attendance and payroll records organized and up to date.</h5>
      </div>
      
      <div>
        
      </div>

      </div>
      <div id="add-entry-container" className="d-flex card" style={{ backgroundColor: '#FFFFFF' }}>
        <div className="card-body">
            <h5>Add Attendance Entry</h5>
        </div>
      </div>
    </div>
  );
}

export default AttendanceLogs;