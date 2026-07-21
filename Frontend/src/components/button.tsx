import { NavLink } from "react-router-dom";
import { Tooltip } from "bootstrap";
import "./button.css";

function Button({
  to,
  text,
  icon,
  collapsed,
  onClick,
  disableActive = false,
}) {
  const handleClick = (e) => {
    const tooltip = Tooltip.getInstance(e.currentTarget);

    if (tooltip) {
      tooltip.hide();
    }

    e.currentTarget.blur();

    onClick?.(e);
  };

  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        disableActive
          ? "custom-btn"
          : `custom-btn${isActive ? " active" : ""}`
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