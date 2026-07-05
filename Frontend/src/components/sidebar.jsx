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

function Sidebar() {
  const [collapsed, setCollapsed] = useState(false);

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
        <Button
          to="/dashboard"
          text="Analytics Dashboard"
          icon={<DashboardIcon />}
          collapsed={collapsed}
        />

        <Button
          to="/inventory"
          text="Inventory Management"
          icon={<InventoryIcon />}
          collapsed={collapsed}
        />

        <Button
          to="/pos"
          text="POS Checkout"
          icon={<CheckoutIcon />}
          collapsed={collapsed}
        />

        <Button
          to="/staff"
          text="Staff Management"
          icon={<StaffIcon />}
          collapsed={collapsed}
        />

        <Button
          to="/logs"
          text="Attendance & Payroll Logs"
          icon={<LogsIcon />}
          collapsed={collapsed}
        />

        <Button
          to="/settings"
          text="Store Settings"
          icon={<SettingsIcon />}
          collapsed={collapsed}
        />
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