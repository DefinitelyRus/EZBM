import './Staff.css';

function Staff() {
  return (
    <div id="staff-contents" className="d-flex flex-row gap-4">
      <div id="staff-content-left" className="d-flex col-9">
      <div id="top-text">
        <h2>Staff Management</h2>
        <h5>Keep your staff information accurate and up to date.</h5>
      </div>
      
      <div className="table-responsive staff-table-container">
        <table className="staff-table">
          <colgroup>
            <col style={{ width: "9%" }} />   {/* ID */}
            <col style={{ width: "12%" }} />  {/* Username */}
            <col style={{ width: "17%" }} />  {/* Name */}
            <col style={{ width: "19%" }} />  {/* Email */}
            <col style={{ width: "12%" }} />  {/* Phone */}
            <col style={{ width: "10%" }} />  {/* Position */}
            <col style={{ width: "7%" }} />   {/* Pay Frequency */}
            <col style={{ width: "7%" }} />   {/* Pay Rate */}
            <col style={{ width: "7%" }} />   {/* Actions */}
          </colgroup>

          <thead>
            <tr>
              <th className="col-left">ID</th>
              <th className="col-center">Username</th>
              <th className="col-center">Name</th>
              <th className="col-center">Email</th>
              <th className="col-center">Phone</th>
              <th className="col-center">Position</th>
              <th className="col-center">Pay<br/>Frequency</th>
              <th className="col-center">Pay<br/>Rate</th>
              <th className="col-center">Actions</th>
            </tr>
          </thead>

        </table>
      </div>
             
      </div>
      <div id="add-new-container" className="d-flex card" style={{ backgroundColor: '#FFFFFF' }}>
        <div className="card-body">
            <h5>Add New Employee</h5>
        </div>
      </div>
    </div>
  );
}

export default Staff;