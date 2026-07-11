import { useState, useEffect } from "react";
import { Tooltip } from "bootstrap";

import "./sidebar.css";
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
const [isSmallScreen, setIsSmallScreen] = useState(false);

useEffect(() => {
  const mediaQuery = window.matchMedia(`(max-width: ${BREAKPOINT - 1}px)`);

  const handleChange = ({ matches }) => {
    setCollapsed(matches);
  };

  // Set initial state
  handleChange(mediaQuery);

  mediaQuery.addEventListener("change", handleChange);

  return () => {
    mediaQuery.removeEventListener("change", handleChange);
  };
}, []);

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
    {
      to: "/logs",
      text: "Attendance & Payroll Logs",
      icon: <LogsIcon />,
    },
    {
      to: "/settings",
      text: "Store Settings",
      icon: <SettingsIcon />,
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
          />
        ))}
      </div>

      <div id="log-out">
        <Button
          to="/"
          text="Log out"
          icon={<LogOutIcon />}
          collapsed={collapsed}
        />
      </div>
    </div>
  );
}

export default Sidebar;