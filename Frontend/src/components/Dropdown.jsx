import { useState } from "react";

function Dropdown({ title, options }) {
  const [selected, setSelected] = useState(title);

  return (
    <div className="dropdown w-100">
      <button
        className="btn btn-secondary dropdown-toggle w-100 d-flex justify-content-between align-items-center"
        type="button"
        data-bs-toggle="dropdown"
        aria-expanded="false"
        style={{ backgroundColor: '#FFFFFF', color: '#1C1E21', borderRadius: '25px' }}
      >
        {selected}
      </button>

      <ul className="dropdown-menu w-100">
        {options.map((option) => (
          <li key={option}>
            <button
              className="dropdown-item"
              type="button"
              onClick={() => setSelected(option)}
            >
              {option}
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default Dropdown;