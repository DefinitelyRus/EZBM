import { useState, useEffect, useMemo, useRef } from "react";
import "./DataTable.css";

function DataTable({
    columns,
    data,
    actionWidth = "12%",
    renderActions
}) {
    const [displayMode, setDisplayMode] = useState(50);
    const [visibleCount, setVisibleCount] = useState(50);

    const loadMoreRef = useRef(null);

    // Reset visible rows whenever the mode or data changes
    useEffect(() => {
        if (displayMode === "all") {
            setVisibleCount(50);
        } else {
            setVisibleCount(displayMode);
        }
    }, [displayMode, data]);

    // Infinite loading when "Load All" is selected
    useEffect(() => {
        if (displayMode !== "all") return;

        const observer = new IntersectionObserver(
            ([entry]) => {
                if (
                    entry.isIntersecting &&
                    visibleCount < data.length
                ) {
                    setVisibleCount((prev) =>
                        Math.min(prev + 50, data.length)
                    );
                }
            },
            {
                threshold: 1,
            }
        );

        if (loadMoreRef.current) {
            observer.observe(loadMoreRef.current);
        }

        return () => observer.disconnect();
    }, [displayMode, visibleCount, data.length]);

    const displayedData = useMemo(() => {
        if (displayMode === "all") {
            return data.slice(0, visibleCount);
        }

        return data.slice(0, displayMode);
    }, [data, displayMode, visibleCount]);

    return (
        <div className="table-wrapper">
            <table className="ez-table">
                <thead>
                    <tr>
                        {columns.map((column) => (
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
                            <tr key={row.id || index}>
                                {columns.map((column) => (
                                    <td
                                        key={column.key}
                                        className={column.className}
                                        style={{
                                            textAlign: column.align,
                                        }}
                                    >
                                        {column.render
                                            ? column.render(row)
                                            : row[column.key]}
                                    </td>
                                ))}
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td
                                colSpan={columns.length}
                                className="table-empty"
                            >
                                Nothing to show here.
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>

            {data.length > 0 && (
                <>
                    <div ref={loadMoreRef} style={{ height: "1px" }} />
                    
                    <div className="table-end">
                        — End of the list —
                    </div>

                    <div className="table-footer">
                        <div className="table-row-selector">
                            <label htmlFor="rows-select">
                                Show
                            </label>

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
                                <option value="all">
                                    Load All
                                </option>
                            </select>

                            <span>
                                Showing{" "}
                                <strong>{displayedData.length}</strong>{" "}
                                of <strong>{data.length}</strong>{" "}
                                results
                            </span>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
}

export default DataTable;