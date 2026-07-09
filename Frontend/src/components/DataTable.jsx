import "./DataTable.css";

function DataTable({
    columns,
    data,
    actionWidth = "12%",
    renderActions
}) {
    return (
        <div className="table-responsive">
            <table className="ez-table">
                <thead>
                    <tr>
                        {columns.map((column) => (
                            <th
                            key={column.key}
                            className={column.className}
                            style={{
                                width: column.width,
                                textAlign: column.align
                            }}
                        >
                            {column.label}
                        </th>
                        ))}
                    </tr>
                </thead>

                <tbody>
                    {data.map((row, index) => (
                        <tr key={row.id || index}>
                            {columns.map((column) => (
                               <td
                                key={column.key}
                                className={column.className}
                                style={{
                                    textAlign: column.align
                                }}
                            >
                                {column.render
                                    ? column.render(row)
                                    : row[column.key]}
                            </td>
                            ))}
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default DataTable;