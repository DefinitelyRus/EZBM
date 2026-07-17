import { useEffect, useRef, useState } from "react";
import "./Dropdown.css";

function Dropdown({ title, options = [], value, onSelect }) {
  const [open, setOpen] = useState(false);
  const dropdownRef = useRef(null);

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
    value !== "" && value !== null && value !== undefined
      ? options[value]
      : title;

  return (
    <div className="md-dropdown" ref={dropdownRef}>
      <button
        type="button"
        className={`md-dropdown-btn ${open ? "open" : ""}`}
        onClick={() => setOpen(!open)}
      >
        <span>{displayValue}</span>

        <span className={`md-arrow ${open ? "rotate" : ""}`}>
          ▼
        </span>
      </button>

      <div className={`md-dropdown-menu ${open ? "show" : ""}`}>
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