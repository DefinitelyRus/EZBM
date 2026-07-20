import { useState, useEffect } from "react";
import './Dashboard.css';

import DataTable from "../../components/DataTable/DataTable"

/* ---------- TABLE ---------- */
  const LowStockColumns = [
    {
    key: "name",
    label: "Product",
    filterLabel: "Name",
    width: "15%",
    className: "col-left",
    render: (row) =>
      [row.firstName, row.lastName]
        .filter(Boolean)
        .join(" "),
    },
    {
      key: "username",
      label: "Stock",
      width: "15%",
      className: "col-center",
    },
    {
      key: "email",
      label: "Target",
      width: "22%",
      className: "col-center",
    },
    {
      key: "phoneNumber",
      label: "Treshold",
      width: "14%",
      className: "col-center",
    }
  ];

function Dashboard() {
  return (
    <div id="dashboard-contents" className="d-flex flex-row gap-4">
      <div id="dashboard-content-left" className="d-flex flex-direction row">
        <div id="top-text">
          <h2>Analytics Dashboard</h2>
          <h5>Here's how the store is doing today.</h5>
        </div>

         <div className="dashboard-panel-content">
          <div className="upper-cards">
            <div className="card dashboard-summary-card">
              <div className="card-body">
                <div className="summary-header">
                  <h5>KEY METRICS SUMMARY</h5>
                </div>

                <div className="summary-divider"></div>

                <div className="summary-item">
                  <div>
                    <h6 className="summary-title">Today's Sales:</h6>
                  </div>

                  <h3 className="summary-value">
                    ₱24,530.00
                  </h3>
                </div>

                <div className="summary-divider-2"></div>

                <div className="summary-item">
                  <div>
                    <h6 className="summary-title">Today's Profit:</h6>
                  </div>

                  <h3 className="summary-value profit" style={{ color: "var(--md-success)" }}>
                    ₱8,942.50
                  </h3>
                </div>

                <div className="summary-divider-2"></div>

                <div className="summary-item">
                  <div>
                    <h6 className="summary-title">Net Profit (All Time):</h6>
                  </div>

                  <h3 className="summary-value profit" style={{ color: "#C62828" }}>
                    ₱8,942.50
                  </h3>
                </div>
              </div>
            </div>

            <div className="card dashboard-summary-card">
              <div className="card-body">
                <div className="summary-header">
                  <h5>7-DAY SALES VOLUME TREND</h5>
                </div>

                <div className="summary-divider"></div>

              </div>
            </div>
          </div>

          <div className="lower-card">
            <div className="card dashboard-summary-card-lower">
              <div className="card-body">
                <div className="summary-header">
                  <h4>Recent Transactions</h4>
                </div>

                <div className="summary-divider"></div>

              </div>
            </div>
          </div>
            
        </div>
      </div>

      <div id="recents-container" >
        <div className="card-body">
          <div className="d-flex flex-direction row panel-header">
            <h4>Low Stock Warnings</h4>
          </div>

          <div className="summary-divider"></div>

          <div>
            <DataTable
                columns={LowStockColumns}
                data={[]}
             />
          </div>
          
        <div className="recents-panel-content">
        </div>
          
        </div>
      </div>
    </div>
  );
}

export default Dashboard;