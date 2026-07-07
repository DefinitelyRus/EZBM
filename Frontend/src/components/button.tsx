import { NavLink } from "react-router-dom";
import { Tooltip } from "bootstrap";
import "./button.css";

function Button({ to, text, icon, collapsed }) {
  const handleClick = (e) => {
    const tooltip = Tooltip.getInstance(e.currentTarget);

    if (tooltip) {
      tooltip.hide();
    }

    e.currentTarget.blur();
  };

  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        `custom-btn${isActive ? " active" : ""}`
      }
      data-bs-toggle={collapsed ? "tooltip" : undefined}
      data-bs-placement="right"
      data-bs-title={collapsed ? text : undefined}
      onClick={handleClick}
    >
      {icon}
      {!collapsed && <span>{text}</span>}
    </NavLink>
  );
}

export default Button;