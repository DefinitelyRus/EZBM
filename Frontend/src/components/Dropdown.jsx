import { useState } from 'react';

function UnitDropdown() {
  const [unit, setUnit] = useState('Unit of Measurement');

  const units = [
    'Kilograms (Kg)',
    'Packs',
    'Bottles',
    'Cartons',
    'Pieces'
  ];

  return (
    <div className="dropdown w-100">
      <button
        className="btn btn-secondary dropdown-toggle w-100 d-flex justify-content-between align-items-center"
        type="button"
        data-bs-toggle="dropdown"
        aria-expanded="false"
        style={{ backgroundColor: '#121212' }}
      >
        {unit}
      </button>

      <ul className="dropdown-menu w-100">
        {units.map((u) => (
          <li key={u}>
            <button
              className="dropdown-item"
              type="button"
              onClick={() => setUnit(u)}
            >
              {u}
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default UnitDropdown;