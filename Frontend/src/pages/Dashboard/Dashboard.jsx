import { useState, useEffect } from "react";
import { DashboardAPI } from "../../api/dashboard";
import "./Dashboard.css";

import DataTable from "../../components/DataTable/DataTable";

function Dashboard() {

  /* ---------- STATES ---------- */

  // Recent Sales
  const [recentSales, setRecentSales] = useState([]);

  // Dashboard Analytics
  const [analytics, setAnalytics] = useState({
    totalSales: 0,
    grossProfit: 0,
    netProfit: 0,
    salesVolumeTrends: [],
    popularProducts: [],
    cashierLeaderboard: [],
    lowStockAlerts: [],
  });

  /* ---------- EFFECTS ---------- */

  useEffect(() => {
    console.log("DashboardAPI:", DashboardAPI);

    loadAnalytics();
    loadRecentSales();
  }, []);

  // Load Dashboard Analytics
  const loadAnalytics = async () => {
    try {
      const data = await DashboardAPI.getAnalytics();
      setAnalytics(data);
    } catch (err) {
      console.error(err);
    }
  };

  /* ---------- DASHBOARD CRUD ---------- */

  // Load Recent Sales
  const loadRecentSales = async () => {
  try {
    console.log("Loading sales...");
    const sales = await DashboardAPI.getRecentSales();
    console.log("Sales:", sales);

    const sorted = [...sales].sort(
      (a, b) => new Date(b.createdAt) - new Date(a.createdAt)
    );

    const latest = sorted.slice(0, 5);

    const fullSales = [];

    for (const sale of latest) {
      console.log("Loading sale", sale.id);

      const result = await DashboardAPI.getSale(sale.id);

      console.log(result);

      fullSales.push(result);
    }

    setRecentSales(fullSales);
  } catch (err) {
    console.error(err);
  }
};

  /* ---------- TABLE ---------- */

  const LowStockColumns = [
    {
      key: "name",
      label: "Product",
      width: "45%",
      className: "col-left",
    },
    {
      key: "quantity",
      label: "Stock",
      width: "18%",
      className: "col-center",
    },
    {
      key: "targetStock",
      label: "Target",
      width: "18%",
      className: "col-center",
    },
  ];

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
                    ₱{analytics.totalSales.toLocaleString(undefined, {
                      minimumFractionDigits: 2,
                      maximumFractionDigits: 2,
                    })}
                  </h3>
                </div>

                <div className="summary-divider-2"></div>

                <div className="summary-item">
                  <div>
                    <h6 className="summary-title">Today's Profit:</h6>
                  </div>

                  <h3
                    className="summary-value profit"
                    style={{ color: "var(--md-success)" }}
                  >
                    ₱{analytics.grossProfit.toLocaleString(undefined, {
                      minimumFractionDigits: 2,
                      maximumFractionDigits: 2,
                    })}
                  </h3>
                </div>

                <div className="summary-divider-2"></div>

                <div className="summary-item">
                  <div>
                    <h6 className="summary-title">Net Profit (All Time):</h6>
                  </div>

                  <h3
                    className="summary-value profit"
                    style={{ color: "#C62828" }}
                  >
                    ₱{analytics.netProfit.toLocaleString(undefined, {
                      minimumFractionDigits: 2,
                      maximumFractionDigits: 2,
                    })}
                  </h3>
                </div>
              </div>
            </div>
          </div>

          <div className="lower-card">
            <div className="card dashboard-summary-card-lower">
              <div className="card-body">
                <div className="summary-header">
                  <h5>RECENT TRANSACTIONS</h5>
                </div>

                <div className="summary-divider"></div>

              </div>
            </div>
          </div>
            
        </div>
      </div>

      <div id="low-stock-container" >
        <div className="card-body">
          <div className="d-flex flex-direction row panel-header">
            <h4>Low Stock Warnings</h4>
          </div>

          <div className="summary-divider"></div>

          <div className="stock-panel-content">
            <DataTable
              columns={LowStockColumns}
              data={analytics.lowStockAlerts}
              enableSearch={false}
              enableColumnFilter={false}
              enableRowSelector={false}
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