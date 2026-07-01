import './sidebar.css';
import Button from './Button';

import ArrowMenu from '../assets/arrow_menu.svg';
import DashboardIcon from '../assets/dashboard.svg?react';
import InventoryIcon from '../assets/inventory.svg?react';
import CheckoutIcon from '../assets/checkout.svg?react';
import StaffIcon from '../assets/staff.svg?react';
import LogsIcon from '../assets/logs.svg?react';

function Sidebar() {
  return (
    <div id="side-bar" className="d-flex col-2">
      <button className="menu-btn">
        <img src={arrowMenu} alt="Menu" />
      </button>
      <h4 id="user-name">Red Sinangote</h4>

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

      </div>

      <button id="log-out" className="btn btn-light align-self-center">
        Log out
      </button>
    </div>
  );
}

export default Sidebar;