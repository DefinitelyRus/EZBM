import './Inventory.css';

import React, { useState } from "react";
import Dropdown from '../components/Dropdown';
import { dropdownOptions } from "../components/dropdownOptions";

import SearchIcon from "../assets/search.svg?react";
import DeleteIcon from "../assets/delete.svg";
import EditIcon from "../assets/edit.svg";

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
    unit: "Kilograms",
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

  const [search, setSearch] = useState("");

  const filteredItems = inventoryItems.filter((item) => {
    const query = search.toLowerCase();
    return (
      (item.name && item.name.toLowerCase().includes(query)) ||
      (item.tags && item.tags.toLowerCase().includes(query)) ||
      (item.description && item.description.toLowerCase().includes(query))
    );
  });

  return (
    <div id="inventory-contents" className="d-flex flex-row gap-4">
      <div id="inventory-content-left" className="d-flex col-9">
        <div id='top-text'>
          <h2>Inventory Management</h2>
          <h5>Keep track of your products and stock levels.</h5>
        </div>

        <div className="input-group flex-direction row">
          <div className="search-bar-container">
            <input
              type="text"
              className="form-control"
              placeholder="Type in to filter..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>

           <div className="table-container">
              <table className="inventory-table">
              <colgroup>
                <col style={{ width: "27%" }} /> {/* Name */}
                <col style={{ width: "6%" }} />  {/* For Sale */}
                <col style={{ width: "8%" }} /> {/* Cost Price */}
                <col style={{ width: "8%" }} /> {/* Sale Price */}
                <col style={{ width: "10%" }} />  {/* Quantity */}
                <col style={{ width: "10%" }} /> {/* Unit */}
                <col style={{ width: "12%" }} /> {/* Expiration */}
                <col style={{ width: "12%" }} /> {/* Tags */}
                <col style={{ width: "10%" }} />  {/* Actions */}
              </colgroup>

              <thead>
                <tr>
                  <th className="col-left">Name</th>
                  <th className="col-center">For<br />Sale?</th>
                  <th className="col-center">Cost<br />Price</th>
                  <th className="col-center">Sale<br />Price</th>
                  <th className="col-center">Quantity</th>
                  <th className="col-center">Unit</th>
                  <th className="col-center">Expiration</th>
                  <th className="col-center">Tags</th>
                  <th className="col-center"></th>
                </tr>
              </thead>

              <tbody>
                {filteredItems.length > 0 ? (
                  filteredItems.map((item) => (
                    <tr key={item.name}>
                      <td className="col-left">{item.name}</td>
                      <td className="col-center">
                        {item.forSale ? "Yes" : "No"}
                      </td>
                      <td className="col-center">₱{item.costPrice}</td>
                      <td className="col-center">
                        {item.salePrice ? `₱${item.salePrice}` : "-"}
                      </td>
                      <td className="col-center">{item.quantity}</td>
                      <td className="col-center">{item.unit}</td>
                      <td className="col-center">{item.expiration}</td>
                      <td className="col-center">{item.tags}</td>

                      <td className="col-center-btn">
                        <div className="d-flex flex-direction col gap-1 btn-group">
                          <button className="edit" title="Edit">
                            <img src={EditIcon} alt="Edit" />
                          </button>

                          <button className="delete" title="Delete">
                            <img src={DeleteIcon} alt="Delete" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="9" className="text-center py-4">
                      No items found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
            </div>
          </div>
        </div>

      <div
        id="add-items-container"
        className="d-flex card"
        style={{ backgroundColor: '#FFFFFF' }}
      >
        <div className="card-body">
          <h5 >Add New Item</h5>

          <div id="item-inputs">
            <div className="mb-3">
              <label className="form-label">Name</label>
              <input
                type="text"
                className="form-control usr-input"
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Description</label>
              <textarea
                className="form-control usr-input"
                style={{ minHeight: "80px" }}
              ></textarea>
            </div>

            <div className="mb-3">
              <label className="form-label">Cost Price</label>
              <div className="input-group">
                <span className="input-group-text currency-span">₱</span>
                <input
                  type="text"
                  className="form-control usr-input"
                />
              </div>
            </div>

            <div className="mb-3">
              <label className="form-label">Sale Price</label>
              <div className="input-group">
                <span className="input-group-text currency-span">₱</span>
                <input
                  type="text"
                  className="form-control usr-input"
                />
              </div>
            </div>

            <div className="mb-3">
              <label className="form-label">Stock Quantity</label>
              <input
                type="text"
                className="form-control usr-input"
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Unit of Measurement</label>
              <Dropdown
                  title="Select an option"
                  options={dropdownOptions.units}
                  className="usr-input"
              />
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

            <div className="mb-3">
              <label className="form-label">
                Expiry Date
              </label>

              <DatePicker
                selected={expiryDate}
                onChange={(date) => setExpiryDate(date)}
                className="form-control usr-input date-picker"
                dateFormat="MM/dd/yyyy"
              />
            </div>
            <div className="mb-2 d-flex flex-direction row">
            <label className="form-label">
              Tags:
            </label>
                <div className="d-flex flex-wrap gap-1">
                  <button className="tag-btn">Beverage</button>
                  <button className="tag-btn">Food</button>
                  <button className="tag-btn">Ingredient</button>
                  <button className="tag-btn">Dairy</button>
                  <button className="tag-btn">Supplies</button>
                  <button className="tag-btn">Hygiene</button>
                </div>
            </div>
          </div>
        </div>
        
        <div className="d-flex justify-content-center gap-3">
          <button
            id="create-item"
            className="btn btn-light align-self-center"
          >
            Create Item
          </button>
          <button
            id="clear-item"
            className="btn btn-light align-self-center"
          >
            Clear
          </button>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;