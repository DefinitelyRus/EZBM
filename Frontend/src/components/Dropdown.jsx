import { useEffect, useRef, useState } from "react";
import "./Dropdown.css";

function Dropdown({ title, options, value, onSelect }) {
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

  const handleSelect = (option) => {
    setOpen(false);

    if (onSelect) {
      onSelect(option);
    }
  };

  return (
    <div className="md-dropdown" ref={dropdownRef}>
      <button
        type="button"
        className={`md-dropdown-btn ${open ? "open" : ""}`}
        onClick={() => setOpen(!open)}
      >
        <span>{value || title}</span>

        <span className={`md-arrow ${open ? "rotate" : ""}`}>
          ▼
        </span>
      </button>

      <div className={`md-dropdown-menu ${open ? "show" : ""}`}>
        {options.map((option) => (
          <button
            key={option}
            type="button"
            className="md-dropdown-item"
            onClick={() => handleSelect(option)}
          >
            {option}
          </button>
        ))}
      </div>
    </div>
  );
}

export default Dropdown;