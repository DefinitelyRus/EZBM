import { useState, useEffect } from "react";
import { NavLink, useNavigate, useLocation } from "react-router-dom";
import { Tooltip } from "bootstrap";

import "./Sidebar.css";
import Button from "./button";

import ArrowMenu from "../assets/arrow_menu.svg?react";

import DashboardIcon from "../assets/dashboard.svg?react";
import InventoryIcon from "../assets/inventory.svg?react";
import CheckoutIcon from "../assets/checkout.svg?react";
import StaffIcon from "../assets/staff.svg?react";
import LogsIcon from "../assets/logs.svg?react";
import LogOutIcon from "../assets/log_out.svg?react";
import SettingsIcon from "../assets/settings.svg?react";

const BREAKPOINT = 1600;

function Sidebar() {

  const [collapsed, setCollapsed] = useState(false);
  const [logsOpen, setLogsOpen] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();

  const isLoggedIn = !!localStorage.getItem("staffId");

  const closeLogsMenu = () => {
    setLogsOpen(false);
  }

  useEffect(() => {
    const mediaQuery = window.matchMedia(`(max-width: ${BREAKPOINT - 1}px)`);

    const handleChange = ({ matches }) => {
      setCollapsed(matches);
    };

    handleChange(mediaQuery);

    mediaQuery.addEventListener("change", handleChange);

    return () => {
      mediaQuery.removeEventListener("change", handleChange);
    };
  }, []);

  /* Helper */
 const handleLogout = () => {
  localStorage.removeItem("staffId");
  localStorage.removeItem("username");
  localStorage.removeItem("position");
  localStorage.removeItem("payFrequency");
  localStorage.removeItem("payRate");

  window.location.href = "/login";
};

  /* Navigation */

  const navItems = [
    {
      to: "/dashboard",
      text: "Analytics Dashboard",
      icon: <DashboardIcon />,
    },
    {
      to: "/inventory",
      text: "Inventory Management",
      icon: <InventoryIcon />,
    },
    {
      to: "/pos",
      text: "POS Checkout",
      icon: <CheckoutIcon />,
    },
    {
      to: "/staff",
      text: "Staff Management",
      icon: <StaffIcon />,
    },
  ];

  useEffect(() => {
    const tooltipTriggerList = document.querySelectorAll(
      '[data-bs-toggle="tooltip"]'
    );

    const tooltipList = [...tooltipTriggerList].map(
      (el) =>
        new Tooltip(el, {
          animation: true,
          trigger: "hover focus",
          customClass: "sidebar-tooltip",
        })
    );

    return () => {
      tooltipList.forEach((tooltip) => tooltip.dispose());
    };
  }, [collapsed]);

  return (
    <div
      id="side-bar"
      className={`d-flex ${collapsed ? "collapsed" : ""}`}
    >
      <div className="d-flex header-cont">
        {!collapsed && (
          <h4 id="user-name" className="align-self-center">
            Krishna Reformina
          </h4>
        )}

        <button onClick={() => setCollapsed(!collapsed)}>
          <ArrowMenu />
        </button>
      </div>

      <div id="nav-btn-group">
        {navItems.map((item) => (
          <Button
            key={item.to}
            to={item.to}
            text={item.text}
            icon={item.icon}
            collapsed={collapsed}
            onClick={closeLogsMenu}
          />
        ))}

        {/* Attendance & Payroll Logs */}
        <button
          className="custom-btn logs-parent"
          onClick={() => setLogsOpen(!logsOpen)}
          type="button"
          data-bs-toggle={collapsed ? "tooltip" : undefined}
          data-bs-placement="right"
          data-bs-title={collapsed ? "Attendance & Payroll Logs" : undefined}
        >
          <LogsIcon />

          {!collapsed && (
            <>
              <span>Attendance & Payroll Logs</span>
            </>
          )}
        </button>

        {!collapsed && logsOpen && (
          <div className="logs-submenu">
            <NavLink
              to="/attendance"
              className={({ isActive }) =>
                `submenu-link${isActive ? " active" : ""}`
              }
            >
              Attendance Ledger
            </NavLink>

            <NavLink
              to="/payroll"
              className={({ isActive }) =>
                `submenu-link${isActive ? " active" : ""}`
              }
            >
              Payroll Records
            </NavLink>

            <NavLink
              to="/leave"
              className={({ isActive }) =>
                `submenu-link${isActive ? " active" : ""}`
              }
            >
              Action Audit Logs
            </NavLink>
          </div>
        )}

        {/* Store Settings */}
        <Button
          to="/settings"
          text="Store Settings"
          icon={<SettingsIcon />}
          collapsed={collapsed}
          onClick={closeLogsMenu}
        />
      </div>

      <div id="log-out">
        {/* <button className="clock-out-btn">
          Clock out
        </button> */}
         {isLoggedIn ? (
          <Button
            to="/login"
            text="Log out"
            icon={<LogOutIcon />}
            collapsed={collapsed}
            onClick={handleLogout}
            disableActive
          />
        ) : (
          <Button
            to="/login"
            text="Login"
            icon={<LogOutIcon />}
            collapsed={collapsed}
            disableActive
          />
        )}
      </div>
    </div>
  );
}

export default Sidebar;