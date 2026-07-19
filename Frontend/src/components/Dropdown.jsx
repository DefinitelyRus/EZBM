import { useEffect, useRef, useState } from "react";
import "./Dropdown.css";

function Dropdown({
  title,
  options = [],
  value,
  onSelect,
  direction = "auto", // "auto" | "up" | "down"
}) {
  const [open, setOpen] = useState(false);
  const [openUp, setOpenUp] = useState(false);

  const dropdownRef = useRef(null);
  const menuRef = useRef(null);

  useEffect(() => {
    function handleOutsideClick(e) {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(e.target)
      ) {
        setOpen(false);
      }
    }

    document.addEventListener("mousedown", handleOutsideClick);

    return () =>
      document.removeEventListener("mousedown", handleOutsideClick);
  }, []);

  const handleSelect = (index) => {
    setOpen(false);
    onSelect?.(index);
  };

  const displayValue =
    value !== "" &&
    value !== null &&
    value !== undefined
      ? options[value]
      : title;

  const toggleDropdown = () => {
    if (!open) {
      const GAP = 20;
      const DEFAULT_MAX_HEIGHT = 220;

      const rect = dropdownRef.current.getBoundingClientRect();

      const spaceBelow =
        window.innerHeight - rect.bottom - GAP;
      const spaceAbove =
        rect.top - GAP;

      let shouldOpenUp = false;

      switch (direction) {
        case "up":
          shouldOpenUp = true;
          break;

        case "down":
          shouldOpenUp = false;
          break;

        default: // auto
          shouldOpenUp =
            spaceBelow < DEFAULT_MAX_HEIGHT &&
            spaceAbove > spaceBelow;
          break;
      }

      setOpenUp(shouldOpenUp);

      requestAnimationFrame(() => {
        if (!menuRef.current) return;

        const availableSpace = shouldOpenUp
          ? spaceAbove
          : spaceBelow;

        if (availableSpace < DEFAULT_MAX_HEIGHT) {
          menuRef.current.style.maxHeight = `${Math.max(
            availableSpace,
            100
          )}px`;
        } else {
          menuRef.current.style.removeProperty("max-height");
        }
      });
    }

    setOpen((prev) => !prev);
  };

  return (
    <div
      className="md-dropdown"
      ref={dropdownRef}
    >
      <button
        type="button"
        className={`md-dropdown-btn ${
          open ? "open" : ""
        }`}
        onClick={toggleDropdown}
      >
        <span>{displayValue}</span>

        <span
          className={`md-arrow ${
            open ? "rotate" : ""
          }`}
        >
          ▼
        </span>
      </button>

      <div
        ref={menuRef}
        className={`md-dropdown-menu ${
          open ? "show" : ""
        } ${openUp ? "open-up" : ""}`}
      >
        {options.map((option, index) => (
          <button
            key={option}
            type="button"
            className="md-dropdown-item"
            onClick={() => handleSelect(index)}
          >
            {option}
          </button>
        ))}
      </div>
    </div>
  );
}

export default Dropdown;