import './Inventory.css';

import React, { useState } from "react";
import UnitDropdown from '../components/Dropdown';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const inventoryItems = [
  {
    name: "Coca-Cola 1.5L",
    forSale: true,
    costPrice: 55,
    salePrice: 65,
    quantity: 24,
    unit: "Bottles",
    expiration: "2026/12/31",
    tags: "Beverage, Soft Drink"
  },
  {
    name: "Lucky Me Pancit Canton",
    forSale: true,
    costPrice: 12,
    salePrice: 15,
    quantity: 100,
    unit: "Packs",
    expiration: "2027/04/15",
    tags: "Food, Noodles"
  },
  {
    name: "White Sugar",
    forSale: false,
    costPrice: 50,
    salePrice: 0,
    quantity: 10,
    unit: "Kg",
    expiration: "N/A",
    tags: "Ingredient"
  },
  {
    name: "Fresh Milk",
    forSale: true,
    costPrice: 85,
    salePrice: 100,
    quantity: 15,
    unit: "Cartons",
    expiration: "2026/08/10",
    tags: "Dairy"
  },
  {
    name: "Plastic Cups 16oz",
    forSale: false,
    costPrice: 120,
    salePrice: 0,
    quantity: 200,
    unit: "Pieces",
    expiration: "N/A",
    tags: "Supplies"
  }
];

function Dashboard() {
  const [expiryDate, setExpiryDate] = useState(new Date());

  return (
    <div id="inventory-contents" className="d-flex flex-row gap-4">
      <div id="inventory-content-left" className="d-flex col-9">
        <div id='top-text'>
          <h2>Product & Service Catalogue</h2>
        </div>

        <div className="input-group flex-direction row">
          <div className="search-bar-container">
          <input
            type="text"
            className="form-control"
            placeholder="Type in to filter in real time..."
          />
          </div>

           <div className="table-responsive table-container">
            <table className="inventory-table">
              <thead>
                <tr>
                <th>Name</th>
                <th>For<br />Sale?</th>
                <th>Cost<br />Price</th>
                <th>Sale<br />Price</th>
                <th>Quantity</th>
                <th>Unit</th>
                <th>Expiration</th>
                <th>Tags</th>
                <th>Actions</th>
                </tr>
              </thead>

              <tbody>
                {inventoryItems.map((item) => (
                  <tr key={item.id}>
                    <td>{item.name}</td>
                    <td>{item.forSale ? "Yes" : "No"}</td>
                    <td>₱{item.costPrice}</td>
                    <td>{item.salePrice ? `₱${item.salePrice}` : "-"}</td>
                    <td>{item.quantity}</td>
                    <td>{item.unit}</td>
                    <td>{item.expiration}</td>
                    <td>{item.tags}</td>
                    <td>
                      <div className="d-flex flex-direction row">
                        <span className="action-link edit">Edit</span>
                        <span className="action-link delete">Delete</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div
        id="add-items-container"
        className="d-flex card"
        style={{ backgroundColor: '#EDE7D9' }}
      >
        <div className="card-body">
          <h5>Add New Item</h5>

          <div id="item-inputs">
            <div className="input-group mb-3">
                <input type="text" className="form-control" placeholder="Name" aria-describedby="basic-addon1" />
            </div>

            <div className="input-group mb-3">
                <textarea className="form-control" placeholder="Description" aria-label="With textarea"></textarea>
            </div>

            <div className="input-group mb-3">
                <span className="input-group-text currency-span">₱</span>
                <input type="text" className="form-control" placeholder="Cost Price" aria-label="Amount (to the nearest dollar)" />
            </div>

            <div className="input-group mb-3">
                <span className="input-group-text currency-span">₱</span>
                <input type="text" className="form-control" placeholder="Sale Price" aria-label="Amount (to the nearest dollar)" />
            </div>

            <div className="input-group mb-3">
                <input type="text" className="form-control" placeholder="Stock Quantity" aria-describedby="basic-addon1" />
            </div>

            <div>
              <UnitDropdown />
            </div>

            <div className="form-check">
              <input
                className="form-check-input"
                type="checkbox"
                id="flexCheckDefault"
              />
              <label
                className="form-check-label"
                htmlFor="flexCheckDefault"
              >
                Available for Sale
              </label>
            </div>

            <div className="mb-3 d-flex flex-direction row">
              <label className="form-label">
                Expiry Date:
              </label>

              <DatePicker
                selected={expiryDate}
                onChange={(date) => setExpiryDate(date)}
                className="form-control date-picker"
                dateFormat="MM/dd/yyyy"
              />
            </div>
            <div className="mb-2 d-flex flex-direction row">
            <label className="form-label">
              Tags:
            </label>
            </div>
          </div>
        </div>

        <button
          id="create-item"
          className="btn btn-light align-self-center"
        >
          Create Item
        </button>
      </div>
    </div>
  );
}

export default Dashboard;