import { Routes, Route, Navigate } from "react-router-dom";

import Sidebar from "./components/Sidebar/Sidebar";

import Login from "./pages/Login/Login";
import Dashboard from "./pages/Dashboard/Dashboard";
import Inventory from "./pages/Inventory/Inventory";
import Checkout from "./pages/Checkout/Checkout";
import Staff from "./pages/Staff/Staff";
import AttendanceLogs from "./pages/AttendanceLogs/AttendanceLogs";

function App() {
  const isLoggedIn = !!localStorage.getItem("staffId");

  return (
    <div id="main-pages" className="container-fluid">
      <div className="row">
        {/* Sidebar  */}
        <Sidebar />

        <div className="col">
          <Routes>
            {/* Login */}
            <Route
              path="/login"
              element={
                isLoggedIn ? (
                  <Navigate to="/dashboard" replace />
                ) : (
                  <Login />
                )
              }
            />

            {/* Root */}
            <Route
              path="/"
              element={
                isLoggedIn ? (
                  <Navigate to="/dashboard" replace />
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />

            {/* Protected Pages */}
            <Route
              path="/dashboard"
              element={
                isLoggedIn ? (
                  <Dashboard />
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />

            <Route
              path="/inventory"
              element={
                isLoggedIn ? (
                  <Inventory />
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />

            <Route
              path="/pos"
              element={
                isLoggedIn ? (
                  <Checkout />
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />

            <Route
              path="/staff"
              element={
                isLoggedIn ? (
                  <Staff />
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />

            <Route
              path="/logs"
              element={
                isLoggedIn ? (
                  <AttendanceLogs />
                ) : (
                  <Navigate to="/login" replace />
                )
              }
            />

            {/* Catch-all */}
            <Route
              path="*"
              element={
                <Navigate
                  to={isLoggedIn ? "/dashboard" : "/login"}
                  replace
                />
              }
            />
          </Routes>
        </div>
      </div>
    </div>
  );
}

export default App;