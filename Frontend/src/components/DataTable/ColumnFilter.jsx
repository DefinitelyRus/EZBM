import { useState, useEffect, useRef } from "react";
import "./ColumnFilter.css";

import MenuIcon from "../../assets/menu.svg";

function ColumnFilter({
  columns,
  visibleColumns,
  setVisibleColumns,
}) {
  const [open, setOpen] = useState(false);

  const filterRef = useRef(null);

  const toggleColumn = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key)
        ? prev.filter((c) => c !== key)
        : [...prev, key]
    );
  };

  // Close when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (
        filterRef.current &&
        !filterRef.current.contains(event.target)
      ) {
        setOpen(false);
      }
    };

    document.addEventListener("mousedown", handleClickOutside);

    return () => {
      document.removeEventListener(
        "mousedown",
        handleClickOutside
      );
    };
  }, []);

  return (
    <div className="column-filter" ref={filterRef}>
      <button
        type="button"
        className="d-flex column-filter-btn gap-2"
        onClick={() => setOpen((prev) => !prev)}
      >
        <img src={MenuIcon} alt="Menu" />
        Columns
      </button>

      {open && (
        <div className="column-filter-menu">
          {columns
            .filter((column) => column.hideable !== false)
            .map((column) => (
              <label
                key={column.key}
                className="column-filter-item"
              >
                <input
                  type="checkbox"
                  checked={visibleColumns.includes(column.key)}
                  onChange={() => toggleColumn(column.key)}
                />

                <span>
                  {column.filterLabel ?? column.label}
                </span>
              </label>
            ))}
        </div>
      )}
    </div>
  );
}

export default ColumnFilter;