import './InventoryAlert.css';
import AlertIcon from '../assets/Alert.svg';

function Alert({ item, stock }) {

  const alertColor = stock < 10 ? '#DC6464' :'#F8BC56';
  const stockAlert = stock < 10 ? 'Critical' : 'Low';

  return (
    <button
      id="alert-button"
      type="button"
      className="btn d-flex flex-direction col" style={{ backgroundColor: alertColor}}
    >
      <img id="icon" src={AlertIcon} alt="Alert" />

      <h3 className="align-self-center">{item}</h3>

      <p className="align-self-end mb-0">
       {stockAlert}: {stock} units left
      </p>
    </button>
  );
}

export default Alert;