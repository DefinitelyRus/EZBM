import './Checkout.css';
import Dropdown from '../components/Dropdown';
import { dropdownOptions } from "../components/dropdownOptions";

function Checkout() {
  return (
    <div id="checkout-contents" className="d-flex flex-row gap-4">
      <div id="checkout-content-left" className="d-flex col-9">
      <div id="top-text">
        <h2>Point-of-Sale Checkout</h2>
      </div>
      <div id="card-container" className="d-flex flex-row gap-4" >
        
      </div>
      </div>
      <div id="recents-container" className="d-flex card" style={{ backgroundColor: '#EDE7D9' }}>
        <div className="card-body">
            <h5>Shopping Cart</h5>

           <div id="item-inputs">
              <h5>Total Amount:</h5>
             <div className="dropdown">
              <Dropdown
                  title="Payment Methods"
                  options={dropdownOptions.paymentMethods}
              />
            </div>

              <div className="input-group mb-3">
                  <textarea className="form-control" placeholder="Optional comments..." aria-label="With textarea"></textarea>
              </div>
            </div>
        </div>
         <button
          id="complete-checkout"
          className="btn btn-light align-self-center"
        >
          Complete Checkout
        </button>
      </div>
    </div>
  );
}

export default Checkout;