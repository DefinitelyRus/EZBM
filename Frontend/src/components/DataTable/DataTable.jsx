import { useState, useEffect, useMemo, useRef } from "react";
import "./DataTable.css";

import ColumnFilter from "./ColumnFilter";

import SearchIcon from "../../assets/search.svg?react";

function DataTable({
  columns,
  data,

  enableSearch = false,
  search = "",
  onSearchChange,

  enableColumnFilter = false,
  showFooter = true,   // NEW
}) {
  const [displayMode, setDisplayMode] = useState(50);
  const [visibleCount, setVisibleCount] = useState(50);

  const [visibleColumns, setVisibleColumns] = useState([]);

  const loadMoreRef = useRef(null);

  // Reset visible columns whenever a different table is loaded
  useEffect(() => {
    setVisibleColumns(columns.map((column) => column.key));
  }, [columns]);

  // Reset visible rows whenever the display mode changes
  useEffect(() => {
    setVisibleCount(displayMode === "all" ? 50 : displayMode);
  }, [displayMode, data]);

  // Infinite loading
  useEffect(() => {
    if (displayMode !== "all") return;

    const observer = new IntersectionObserver(([entry]) => {
      if (entry.isIntersecting && visibleCount < data.length) {
        setVisibleCount((prev) =>
          Math.min(prev + 50, data.length)
        );
      }
    });

    if (loadMoreRef.current) {
      observer.observe(loadMoreRef.current);
    }

    return () => observer.disconnect();
  }, [displayMode, visibleCount, data.length]);

  const displayedColumns = useMemo(
    () =>
      columns.filter((column) =>
        visibleColumns.includes(column.key)
      ),
    [columns, visibleColumns]
  );

  const displayedData = useMemo(
    () =>
      displayMode === "all"
        ? data.slice(0, visibleCount)
        : data.slice(0, displayMode),
    [data, displayMode, visibleCount]
  );

  return (
    <div className="table-wrapper">

      <div className="table-toolbar">

        {enableSearch && (
          <div className="search-bar">
            <SearchIcon className="search-icon" />

            <input
              type="text"
              className="search-input"
              placeholder="Type in to filter..."
              value={search}
              onChange={(e) =>
                onSearchChange?.(e.target.value)
              }
            />

            {search && (
              <button
                type="button"
                className="clear-search-btn"
                onClick={() => onSearchChange?.("")}
              >
                ✕
              </button>
            )}
          </div>
        )}

       {enableColumnFilter && (
          <ColumnFilter
            columns={columns}
            visibleColumns={visibleColumns}
            setVisibleColumns={setVisibleColumns}
          />
        )}

      </div>

      <table className="ez-table">
        <thead>
          <tr>
            {displayedColumns.map((column) => (
              <th
                key={column.key}
                className={column.className}
                style={{
                  width: column.width,
                  textAlign: column.align,
                }}
              >
                {column.label}
              </th>
            ))}
          </tr>
        </thead>

        <tbody>
          {displayedData.length > 0 ? (
            displayedData.map((row, index) => (
              <tr key={row.id ?? index}>
                {displayedColumns.map((column) => (
                  <td
                    key={column.key}
                    className={column.className}
                    style={{ textAlign: column.align }}
                  >
                    {column.render
                      ? column.render(row)
                      : row[column.key]}
                  </td>
                ))}
              </tr>
            ))
          ) : (
            <tr className="table-empty-row">
              <td
                colSpan={displayedColumns.length}
                className="table-empty"
              >
                Nothing to show here.
              </td>
            </tr>
          )}
        </tbody>
      </table>

      {showFooter && data.length > 0 && (
        <>
          <div ref={loadMoreRef} style={{ height: 1 }} />

          <div className="table-end">
            — End of the list —
          </div>

          <div className="table-footer">
            <div className="table-row-selector">
              <label htmlFor="rows-select">Show</label>

              <select
                id="rows-select"
                value={displayMode}
                onChange={(e) =>
                  setDisplayMode(
                    e.target.value === "all"
                      ? "all"
                      : Number(e.target.value)
                  )
                }
              >
                <option value={50}>50</option>
                <option value={100}>100</option>
                <option value="all">Load All</option>
              </select>

              <span>
                Showing <strong>{displayedData.length}</strong> of{" "}
                <strong>{data.length}</strong> results
              </span>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

export default DataTable;