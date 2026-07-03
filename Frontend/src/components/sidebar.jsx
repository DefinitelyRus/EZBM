import './sidebar.css';
import Button from './button';

import ArrowMenu from '../assets/arrow_menu.svg?react';

import DashboardIcon from '../assets/dashboard.svg?react';
import InventoryIcon from '../assets/inventory.svg?react';
import CheckoutIcon from '../assets/checkout.svg?react';
import StaffIcon from '../assets/staff.svg?react';
import LogsIcon from '../assets/logs.svg?react';
import LogOutIcon from '../assets/log_out.svg?react';
import SettingsIcon from '../assets/settings.svg?react';

function Sidebar() {
  return (
    <div id="side-bar" className="d-flex col-2">
      <div className="d-flex header-cont">
        <h4 id="user-name" className="align-self-center">Krishna Reformina</h4>
        <button>
          <ArrowMenu />
        </button>
      </div>

      <div id="nav-btn-group">
        
        <Button
          to="/dashboard"
          text="Analytics Dashboard"
          icon={<DashboardIcon />}
        />

        <Button
          to="/inventory"
          text="Inventory Management"
          icon={<InventoryIcon />}
        />

        <Button
          to="/pos"
          text="POS Checkout"
          icon={<CheckoutIcon />}
        />

        <Button
          to="/staff"
          text="Staff Management"
          icon={<StaffIcon />}
        />

        <Button
          to="/logs"
          text="Attendance & Payroll Logs"
          icon={<LogsIcon />}
        />

        <Button 
          to="/"
          text="Store Settings"
          icon={<SettingsIcon />}
        />
      </div>

      <div id="log-out">
        <Button
          to="/"
          text="Log out"
          icon={<LogOutIcon />}
        />
      </div>
    </div>
  );
}

export default Sidebar;