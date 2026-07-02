import './InventoryAlert.css';
import AlertIcon from '../assets/Alert.svg?react';

function Alert({ item, stock }) { 

  const background = stock < 10 ? "#FEE2E2" : "#FFF8E1";
  const iconColor = stock < 10 ? '#C62828' : '#B7791F';
  const textColor = stock < 10 ? '#7F1D1D' : '#8A5A00';
  const stockAlert = stock < 10 ? 'Critical' : 'Low';

  return (
    <button
      id="alert-button"
      type="button"
      className="btn d-flex flex-direction col" 
      style={{ backgroundColor: background }}
    >
      <AlertIcon
      id="icon"
      style={{ color: iconColor }}
      />
      
      <div className="d-flex flex-direction col justify-content-between">
        <h4 className="align-self-center" style={{ color: textColor }}>{item}</h4>

        <p className="align-self-end mb-0" style={{ color: textColor }}>
        {stockAlert}: {stock} units left
        </p>
      </div>
    </button>
  );
}

export default Alert;