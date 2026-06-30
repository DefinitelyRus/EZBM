import './Checkout.css';
import Dropdown from '../components/Dropdown';
import { dropdownOptions } from "../components/dropdownOptions";

const forSaleItems = [
  {
    name: "Coca-Cola 1.5L",
    forSale: true,
    salePrice: 65,
    quantity: 24,
    unit: "Bottles"
  },
  {
    name: "Lucky Me Pancit Canton",
    forSale: true,
    salePrice: 15,
    quantity: 100,
    unit: "Packs"
  },
  {
    name: "White Sugar",
    forSale: false,
    salePrice: 0,
    quantity: 10,
    unit: "Kg"
  },
  {
    name: "Fresh Milk",
    forSale: true,
    salePrice: 100,
    quantity: 15,
    unit: "Cartons"
  },
  {
    name: "Plastic Cups 16oz",
    forSale: false,
    salePrice: 0,
    quantity: 200,
    unit: "Pieces"
  }
];

const availableItems = forSaleItems.filter(item => item.forSale);

function Checkout() {
  return (
    <div id="checkout-contents" className="d-flex flex-row gap-4">
      <div id="checkout-content-left" className="d-flex col-8">
      <div id="top-text">
        <h2>Point-of-Sale Checkout</h2>
      </div>
      
      <div className="input-group flex-direction row">
          <div className="search-bar-container">
          <input
            type="text"
            className="form-control"
            placeholder="Search item name..."
          />
          </div>

           <div className="table-responsive checkout-table-container">
            <table className="checkout-table">
              <colgroup>
                <col style={{ width: "45%" }} />
                <col style={{ width: "15%" }} />
                <col style={{ width: "20%" }} />
                <col style={{ width: "15%" }} />
              </colgroup>

              <thead>
                <tr>
                  <th>Name</th>
                  <th className="col-center">Price</th>
                  <th className="col-center">In Stock</th>
                  <th className="col-center">Action</th>
                </tr>
              </thead>

              <tbody>
                {availableItems.map((item) => (
                  <tr key={item.id}>
                    <td className="col-left">{item.name}</td>
                    <td className="col-center">₱{item.salePrice}</td>
                    <td className="col-center">{item.quantity} {item.unit}</td>
                    <td className="col-center">
                      <button className="add-btn">Add to Cart</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

      </div>
      <div id="cart-container" className="d-flex card" style={{ backgroundColor: '#EDE7D9' }}>
        <div className="card-body">
            <h5>Shopping Cart</h5>

            <div className="checkout-cart-container">
              <table className="shopping-cart">
                <colgroup>
                  <col style={{ width: "50%" }} />
                  <col style={{ width: "15%" }} />
                  <col style={{ width: "15%" }} />
                  <col style={{ width: "15%" }} />
                </colgroup>

                <thead>
                  <tr>
                    <th>Item</th>
                    <th className="col-center">QTY</th>
                    <th className="col-center">Price</th>
                    <th className="col-center">Total</th>
                  </tr>
                </thead>
              </table>

            <div id="item-inputs">
                <h5>Total Amount:</h5>
              <div className="dropdown">
                <Dropdown
                    title="Payment Methods"
                    options={dropdownOptions.paymentMethods}
                />
              </div>

                <div className="input-group mb-3">
                    <textarea 
                      className="form-control" 
                      placeholder="Optional comments..." 
                      aria-label="With textarea">
                    </textarea>
                </div>
              </div>
            </div>
        </div>
         <button
          id="complete-checkout"
          className="btn btn-light align-self-center">
          Complete Checkout
        </button>
      </div>
    </div>
  );
}

export default Checkout;