import './Checkout.css';

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
        </div>
      </div>
    </div>
  );
}

export default Checkout;