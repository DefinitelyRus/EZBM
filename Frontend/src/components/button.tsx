import { NavLink } from "react-router-dom";

import './button.css';

function Button({ to, text, icon }) {
  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        `custom-btn ${isActive ? "active" : ""}`
      }
    >
      {icon}
      <span>{text}</span>
    </NavLink>
  );
}

export default Button;