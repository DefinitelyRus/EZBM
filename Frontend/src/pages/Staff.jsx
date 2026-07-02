import './Staff.css';

function Staff() {
  return (
    <div id="staff-contents" className="d-flex flex-row gap-4">
      <div id="staff-content-left" className="d-flex col-9">
      <div id="top-text">
        <h2>Staff Management</h2>
      </div>
      <div id="card-container" className="d-flex flex-row gap-4" >
        
      </div>
      </div>
      <div id="recents-container" className="d-flex card" style={{ backgroundColor: '#FFFFFF' }}>
        <div className="card-body">
            <h5>Add New Employee</h5>
        </div>
      </div>
    </div>
  );
}

export default Staff;