import './Attendance_Logs.css';

function Attendance_Logs() {
  return (
    <div id="attendance-logs-contents" className="d-flex flex-row gap-4">
      <div id="attendance-logs-content-left" className="d-flex col-9">
      <div id="top-text">
        <h2>Shift Attendance & Payroll Logs</h2>
       <h5>Keep attendance and payroll records organized and up to date.</h5>
      </div>
      
      <div
        className="btn-group"
        role="group"
        aria-label="Attendance tabs"
      >
        <input
          type="radio"
          className="btn-check"
          name="btnradio"
          id="btnradio1"
          autoComplete="off"
          defaultChecked
        />
        <label className="btn btn-outline-primary" htmlFor="btnradio1">
          Attendance Ledger
        </label>

        <input
          type="radio"
          className="btn-check"
          name="btnradio"
          id="btnradio2"
          autoComplete="off"
        />
        <label className="btn btn-outline-primary" htmlFor="btnradio2">
          Payroll Records
        </label>

        <input
          type="radio"
          className="btn-check"
          name="btnradio"
          id="btnradio3"
          autoComplete="off"
        />
        <label className="btn btn-outline-primary" htmlFor="btnradio3">
          Action Audit Logs
        </label>
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

export default Attendance_Logs;