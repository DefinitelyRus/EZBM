import './Inventory.css';

import React, { useState } from "react";
import Dropdown from '../components/Dropdown';
import DataTable from "../components/DataTable";
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

    const handlePriceChange = (field, value) => {
      if (/^\d*\.?\d{0,2}$/.test(value) || value === "") {
        setFormData((prev) => ({
          ...prev,
          [field]: value,
        }));
      }
    };

  const handleQuantityChange = (value) => {
  if (/^\d*\.?\d*$/.test(value) || value === "") {
    setFormData((prev) => ({
      ...prev,
      quantity: value,
    }));
  }
};

  const [search, setSearch] = useState("");

  /* Search Bar */
  const filteredItems = inventoryItems.filter((item) => {
    const query = search.toLowerCase();
    return (
      (item.name && item.name.toLowerCase().includes(query)) ||
      (item.tags && item.tags.toLowerCase().includes(query)) ||
      (item.description && item.description.toLowerCase().includes(query))
    );
  });

  /* Clear Button */
  const initialForm = {
      name: "",
      description: "",
      forSale: false,
      costPrice: "",
      salePrice: "",
      quantity: "",
      unit: "",
      expiryDate: null,
      tags: [],
    };

  const [formData, setFormData] = useState(initialForm);

  const handleClear = () => {
    setFormData(initialForm);
  };

  const toggleTag = (tag) => {
    setFormData((prev) => ({
      ...prev,
      tags: prev.tags.includes(tag)
        ? prev.tags.filter((t) => t !== tag)
        : [...prev.tags, tag],
    }));
  };

  /* Table */
  const columns = [
    {
      key: "name",
      label: "Name",
      width: "28%",
      className: "col-left",
    },
    {
      key: "forSale",
      label: <>For<br />Sale?</>,
      width: "6%",
      className: "col-center",
      render: (row) => (row.forSale ? "Yes" : "No"),
    },
    {
      key: "costPrice",
      label: <>Cost<br />Price</>,
      width: "8%",
      className: "col-center",
      render: (row) => `₱${row.costPrice}`,
    },
    {
      key: "salePrice",
      label: <>Sale<br />Price</>,
      width: "8%",
      className: "col-center",
      render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
    },
    {
      key: "quantity",
      label: "Quantity",
      width: "9%",
      className: "col-center",
    },
    {
      key: "unit",
      label: "Unit",
      width: "10%",
      className: "col-center",
    },
    {
      key: "expiration",
      label: "Expiration",
      width: "12%",
      className: "col-center",
    },
    {
      key: "tags",
      label: "Tags",
      width: "12%",
      className: "col-center",
    },
    {
      key: "actions",
      label: "",
      width: "12%",
      className: "col-center-btn",
      render: () => (
        <div className="d-flex flex-direction col btn-group">
          <button className="edit" title="Edit">
            <img src={EditIcon} alt="Edit" />
          </button>

          <button className="delete" title="Delete">
            <img src={DeleteIcon} alt="Delete" />
          </button>
        </div>
      ),
    },
  ];

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
              <DataTable
                columns={columns}
                data={filteredItems}
              />
            </div>
          </div>
        </div>

      <div
          id="add-items-container"
          className="card"
      >
        <div className="card-body">
          <div className="panel-header">
            <h4>Add New Item</h4>
            <p>Create a new inventory item.</p>
        </div>

        <form id="item-form">
          <div id="item-inputs">
            <div className="mb-3">
              <label className="form-label">Name</label>
              <input
                type="text"
                className="form-control usr-input"
                 value={formData.name}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      name: e.target.value,
                    })
                  }
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Description</label>
              <textarea
                className="form-control usr-input"
                style={{ minHeight: "60px" }}
                 value={formData.description}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    description: e.target.value,
                  })
                }
              ></textarea>
            </div>

            <div className="form-check">
              <input
                className="form-check-input"
                type="checkbox"
                id="flexCheckDefault"
                checked={formData.forSale}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    forSale: e.target.checked,
                  })
                }
              />
              <label
                className="form-check-label"
                htmlFor="flexCheckDefault"
              >
                Available for Sale
              </label>
            </div>

            <div className="mb-3">
              <label className="form-label">Cost Price</label>
              <div className="input-group">
                <span className="input-group-text currency-span">₱</span>
                <input
                  type="text"
                  className="form-control usr-input"
                  value={formData.costPrice}
                  onChange={(e) => handlePriceChange("costPrice", e.target.value)}
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
                  value={formData.salePrice}
                  onChange={(e) => handlePriceChange("salePrice", e.target.value)}
                />
              </div>
            </div>

            <div className="mb-3">
              <label className="form-label">Quantity</label>
              <input
                type="text"
                className="form-control usr-input w-70"
                value={formData.quantity}
                onChange={(e) => handleQuantityChange(e.target.value)}
                inputMode="decimal"
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Unit of Measurement</label>
              <Dropdown
                title="Select Unit"
                options={dropdownOptions.units}
                value={formData.unit}
                onSelect={(value) =>
                    setFormData({
                        ...formData,
                        unit: value,
                    })
                }
            />
            </div>

            <div className="mb-3">
              <label className="form-label">
                Expiry Date
              </label>

              <DatePicker
                className="form-control usr-input date-picker"
                calendarClassName="md-calendar"
                dateFormat="MM/dd/yyyy"
                placeholderText="mm/dd/yyyy"
                isClearable

                selected={formData.expiryDate}
                onChange={(date) =>
                  setFormData({
                    ...formData,
                    expiryDate: date,
                  })
                }
              />
            </div>

            <div className="mb-2 d-flex flex-direction row">
                <label className="form-label">Tags</label>
                <div className="d-flex flex-wrap gap-2">
                  {[
                    "Beverage",
                    "Food",
                    "Ingredient",
                    "Dairy",
                    "Supplies",
                    "Hygiene",
                  ].map((tag) => (
                    <button
                      key={tag}
                      type="button"
                      className={`tag-btn ${
                        formData.tags.includes(tag) ? "selected" : ""
                      }`}
                      onClick={() => toggleTag(tag)}
                    >
                      {tag}
                    </button>
                  ))}
                </div>
            </div>
          </div>
        </form>
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
                type="button"
                onClick={handleClear}
              >
                Clear
            </button>
            </div>
      </div>
    </div>
  );
}

export default Dashboard;