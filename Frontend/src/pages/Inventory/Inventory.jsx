import React, { useState, useEffect } from "react";
import './Inventory.css';

import Dropdown from "../../components/Dropdown";
import DataTable from "../../components/DataTable";
import { dropdownOptions } from "../../components/dropdownOptions";

import CloseMenu from "../../assets/arrow_menu.svg?react";
import SearchIcon from "../../assets/search.svg?react";
import DeleteIcon from "../../assets/delete.svg";
import EditIcon from "../../assets/edit.svg";
import AddIcon from "../../assets/add.svg";

import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

function Dashboard() {
  
  const [inventoryItems, setInventoryItems] = useState([]);
  const [showAddItem, setShowAddItem] = useState(true);
  const [search, setSearch] = useState("");

  // Fetch items from the backend!
  useEffect(() => {
    fetch("http://localhost:5056/api/items")
      .then((response) => {
        if (!response.ok) {
          throw new Error("Failed to fetch inventory items.");
        }
        return response.json();
      })
      .then((data) => {
        console.log("Items from server:", data);

        const mappedItems = data.map((item) => ({
          name: item.name ?? item.Name,
          description: item.description ?? item.Description,
          forSale: item.isForSale ?? item.IsForSale,
          costPrice: item.cost ?? item.Cost,
          salePrice: item.salePrice ?? item.SalePrice,
          quantity: item.quantity ?? item.Quantity,

          // Handles either a string or an object
          unit:
            typeof (item.unitOfMeasurement ?? item.UnitOfMeasurement) === "object"
              ? (item.unitOfMeasurement ?? item.UnitOfMeasurement)?.name ??
                (item.unitOfMeasurement ?? item.UnitOfMeasurement)?.Name ??
                ""
              : (item.unitOfMeasurement ?? item.UnitOfMeasurement),

          expiration: item.expirationDate ?? item.ExpirationDate
            ? new Date(item.expirationDate ?? item.ExpirationDate).toLocaleDateString(
                "en-US",
                {
                  month: "2-digit",
                  day: "2-digit",
                  year: "numeric",
                }
              )
            : "N/A",

          // Converts tags array into a comma-separated string
          tags: Array.isArray(item.tags ?? item.Tags)
            ? (item.tags ?? item.Tags)
                .map((tag) =>
                  typeof tag === "object"
                    ? tag.name ?? tag.Name
                    : tag
                )
                .join(", ")
            : "",
        }));

        setInventoryItems(mappedItems);
      })
      .catch((error) => {
        console.error("Error fetching inventory:", error);
      });
  }, []);

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
    <div id="inventory-contents" className="d-flex flex-row gap-4 align-items-start">
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
          className={`card ${showAddItem ? "panel-open" : "panel-closed"}`}
          >
          <div className="card-body">
            <div className="d-flex flex-direction col gap-3 panel-header">
              <div className="flex-direction row">
                <h4>Add New Item</h4>
                <p>Create a new inventory item.</p>
              </div>

              <div>
                <button 
                  className="close-btn"
                  type="button"
                  onClick={() => setShowAddItem(false)}
                >
                  <CloseMenu />
                </button>
              </div>
                
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
                  rows={2}
                  style={{ resize: "none", overflow: "hidden" }}
                  value={formData.description}
                  onChange={(e) => {
                    e.target.style.height = "auto";
                    e.target.style.height = `${e.target.scrollHeight}px`;

                    setFormData({
                      ...formData,
                      description: e.target.value,
                    });
                  }}
                />
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
            <div className="d-flex justify-content-center gap-3 item-btn-group">
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
        <button
          className={`floating-add-btn ${
            showAddItem ? "btn-hidden" : "btn-visible"
          }`}
          type="button"
          onClick={() => setShowAddItem(true)}
        >
          <img src={AddIcon} alt="" />
          <span>Add New Item</span>
        </button>
    </div>
    
  );
}

export default Dashboard;