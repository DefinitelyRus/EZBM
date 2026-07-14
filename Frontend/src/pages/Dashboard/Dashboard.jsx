import './Dashboard.css';

import Alert from '../../components/InventoryAlert';

function Dashboard() {
  return (
    <div id="dashboard-contents" className="d-flex flex-row gap-4">
      <div id="dashboard-content-left" className="d-flex col-9">
      <div id="top-text">
        <h2>Analytics Dashboard</h2>
        <h5>Here's how the store is doing today.</h5>
      </div>
      <div id="card-container" className="d-flex flex-row gap-4" >
        <div className="money-card-1 card" style={{ backgroundColor: '#FFFFFF', borderColor: '#1877F2' }}>
          <div className="card-body d-flex flex-column">
              <h5 className="card-title">Today's Total Sales</h5>
              <h1 className="card-text align-self-center">₱2,439.67</h1>
          </div>
        </div>
        <div className="money-card-2 card" style={{ backgroundColor: '#FFFFFF', borderColor: '#42B72A' }}>
          <div className="card-body d-flex flex-column">
              <h5 className="card-title">Today's Total Profits</h5>
              <h1 className="card-text align-self-center">₱1,247.20</h1>
          </div>
        </div>
      </div>
      <div id="inventory-alerts-container" className="card" style={{ backgroundColor: '#FFFFFF' }}>
        <div className="card-body">
          <div className="card-header d-flex flex-direction col" style={{ backgroundColor: '#FFFFFF' }}>
          <h4 className="card-title">Inventory Items</h4>
          <a id="inventory-link" href="/inventory" className="card-link">Manage Items</a>
          </div>
          <div id="alerts-container" className="d-flex flex-column gap-2">
              <Alert item="Pancit Canton" stock="2" />
              <Alert item="Fresh Milk" stock="12" />
              <Alert item="Coca Cola" stock="15" />
          </div>
        </div>
      </div>
      </div>
      <div id="recents-container" className="d-flex card" style={{ backgroundColor: '#FFFFFF' }}>
        <div className="card-body">
          <h5>Recent Transactions</h5>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;