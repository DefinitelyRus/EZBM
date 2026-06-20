import './Dashboard.css';

import Alert from './InventoryAlert';

function Dashboard() {
  return (
    <div id="dashboard-contents" className="d-flex flex-row gap-4">
      <div id="dashboard-content-left" className="d-flex col-9">
      <div id='top-text'>
        <h2>Today's Overview</h2>
        <h4>Welcome back. Here’s what’s happening today</h4>
      </div>
      <div id="card-container" className="d-flex flex-row gap-4" >
        <div className="money-card card" style={{ backgroundColor: '#B7CAEB' }}>
          <div className="card-body d-flex flex-column">
              <h5 className="card-title">Today's Sales</h5>
              <h1 className="card-text align-self-center">₱2,439.67</h1>
          </div>
        </div>
        <div className="money-card card" style={{ backgroundColor: '#99AD5E' }}>
          <div className="card-body d-flex flex-column">
              <h5 className="card-title">Today's Profits</h5>
              <h1 className="card-text align-self-center">₱1,247.20</h1>
          </div>
        </div>
      </div>
      <div id="inventory-alerts-container" className="card" style={{ backgroundColor: '#EDE7D9' }}>
        <div className="card-body">
          <div className="card-header d-flex flex-direction col" style={{ backgroundColor: '#EDE7D9' }}>
          <h4 className="card-title">Inventory Items</h4>
          <a id="inventory-link" href="/inventory" className="card-link">Manage Items</a>
          </div>
          <div id="alerts-container" className="d-flex flex-column gap-2">
              <Alert item="Mousepad" stock="2" />
              <Alert item="Chasis" stock="12" />
              <Alert item="Headphones" stock="7" />
          </div>
        </div>
      </div>
      </div>
      <div id="recents-container" className="d-flex card" style={{ backgroundColor: '#EDE7D9' }}>
        <div className="card-body">
          <p>This is a filler thing, we can remove this entirely or put smtg else</p>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;