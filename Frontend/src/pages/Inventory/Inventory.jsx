import React, { useState, useEffect } from "react";
import { InventoryAPI } from "../../api/inventory";
import { EnumsAPI } from "../../api/enums";
import './Inventory.css';

import Dropdown from "../../components/Dropdown";
import DataTable from "../../components/DataTable/DataTable";

import CloseMenu from "../../assets/arrow_menu.svg?react";
import SearchIcon from "../../assets/search.svg?react";
import DeleteIcon from "../../assets/delete.svg";
import EditIcon from "../../assets/edit.svg";
import AddIcon from "../../assets/add.svg";

import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const BREAKPOINT = 1600;

function Dashboard() {
 
  const [inventoryItems, setInventoryItems] = useState([]);
  const [showAddItem, setShowAddItem] = useState(true);
  const [search, setSearch] = useState("");

    /* Refreshing the Inventory */
  const loadInventory = async (searchTerm = search) => {
    try {
      const data =
        searchTerm.trim() === ""
          ? await InventoryAPI.getAll()
          : await InventoryAPI.find({
              name: searchTerm.trim(),
            });

      setInventoryItems(data);
    } catch (err) {
      console.error(err);
    }
  };


  /* Fetching enums */
  const [enums, setEnums] = useState({
    units: [],
    tags: [],
  });

  const enumName = (list, value) =>
  list?.[value] ?? "-";

  useEffect(() => {
    async function loadEnums() {
      try {
        const data = await EnumsAPI.getAll();

        setEnums({
          units: data.units,
          tags: data.tags,
        });
      } catch (err) {
        console.error(err);
      }
    }

    loadEnums();
  }, []);

  useEffect(() => {
    const mediaQuery = window.matchMedia(`(max-width: ${BREAKPOINT - 1}px)`);

    const handleChange = ({ matches }) => {
      setShowAddItem(!matches);
    };

    handleChange(mediaQuery);

    mediaQuery.addEventListener("change", handleChange);

    return () =>
      mediaQuery.removeEventListener("change", handleChange);
  }, []);

    /* Ensure only numbers are accepted */
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
  useEffect(() => {
    const timeout = setTimeout(() => loadInventory(search), 300);

    return () => clearTimeout(timeout);
  }, [search]);

  /* Clear Button */
  const initialForm = {
      name: "",
      description: "",
      isForSale: false,
      cost: "",
      salePrice: "",
      quantity: "",
      unitOfMeasurement: "",
      expirationDate: null,
      tags: [],
    };

  const [formData, setFormData] = useState(initialForm);

  const handleClear = () => {
    setEditingItem(null);
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

  const toBackendItem = (formData, enums, extra = {}) => ({
    ...extra,
    ...formData,

    cost: Number(formData.cost),
    salePrice: Number(formData.salePrice),
    quantity: Number(formData.quantity),

    // Convert dropdown index -> enum string
    unitOfMeasurement: formData.unitOfMeasurement,

    // Convert Date object -> ISO string
    expirationDate: formData.expirationDate
      ? formData.expirationDate.toISOString()
      : null,
  });
  
  /* Adding an Item */
 const handleCreate = async () => {
    const item = toBackendItem(formData, enums, {
      imageUrl: null,
      itemType: "Product",
      barcode: null,
      targetStock: 0,
      lowStockThresholdPercentage: 0.2,
      brand: null,
    });

    console.log(item);

    try {
      await InventoryAPI.create(item);
      await loadInventory(search);
      handleClear();
    } catch (err) {
      console.error(err);
    }
  };

  /* Editing an Item */
  const [editingItem, setEditingItem] = useState(null);

    const handleEdit = (item) => {
    setEditingItem(item);

   console.log("Backend expiration:", item.expirationDate);
    console.log("Type:", typeof item.expirationDate);

    setFormData({
      name: item.name ?? "",
      description: item.description ?? "",
      isForSale: item.isForSale,
      cost: item.cost ?? "",
      salePrice: item.salePrice ?? "",
      quantity: item.quantity ?? "",

      unitOfMeasurement: item.unitOfMeasurement,

      // enter code for date here!

      tags: item.tags ?? [],
    });

    setShowAddItem(true);
  };

  const handleUpdate = async () => {
    const item = toBackendItem(formData, enums, {
      id: editingItem.id,
      imageUrl: editingItem.imageUrl,
      barcode: editingItem.barcode,
      brand: editingItem.brand,
      targetStock: editingItem.targetStock ?? 0,
      lowStockThresholdPercentage:
        editingItem.lowStockThresholdPercentage ?? 0.2,
    });

    console.log(item);

    try {
      await InventoryAPI.update(item);

      await loadInventory(search);

      handleClear();

      alert("Item updated successfully!");
    } catch (err) {
      console.error(err);
      alert("Failed to update item.");
    }
  };

  /* Deleting an Item */
  const [deletingId, setDeletingId] = useState(null);

  const handleDelete = async (id) => {
    if (!window.confirm("Delete this item?")) return;

    try {
      setDeletingId(id);

      await InventoryAPI.delete(id);

      // Reload using the current search term
      await loadInventory(search);
    } catch (err) {
      console.error(err);
    } finally {
      setDeletingId(null);
    }
  };

  /* Table */
  const InventoryColumns = [
    {
      key: "name",
      label: "Name",
      filterLabel: "Name",
      width: "28%",
      className: "col-left",
    },
    {
      key: "isForSale",
      label: <>For<br />Sale</>,
      filterLabel: "For Sale",
      width: "6%",
      className: "col-center",
      render: (row) => (row.isForSale ? "Yes" : "No"),
    },
    {
      key: "cost",
      label: <>Cost<br />Price</>,
      filterLabel: "Cost Price",
      width: "8%",
      className: "col-center",
      render: (row) => `₱${row.cost}`,
    },
    {
      key: "salePrice",
      label: <>Sale<br />Price</>,
      filterLabel: "Sale Price",
      width: "8%",
      className: "col-center",
      render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
    },
    {
      key: "quantity",
      label: "Quantity",
      filterLabel: "Quantity",
      width: "9%",
      className: "col-center",
    },
    {
      key: "unitOfMeasurement",
      label: "Unit",
      filterLabel: "Unit",
      width: "10%",
      className: "col-center",
      render: (row) => enumName(enums.units, row.unitOfMeasurement),
    },
    {
      key: "expirationDate",
      label: "Expiration",
      filterLabel: "Expiration",
      width: "12%",
      className: "col-center",
      render: (row) =>
        row.expirationDate
          ? new Date(row.expirationDate).toLocaleDateString("en-US")
          : "-",
    },
    {
      key: "tags",
      label: "Tags",
      filterLabel: "Tags",
      width: "12%",
      className: "col-center",
    },
    {
      key: "actions",
      label: "",
      filterLabel: "Actions",
      hideable: false,
      width: "12%",
      className: "col-center-btn",
      render: (row) => (
        <div className="d-flex flex-direction col btn-group">
          <button
            className="edit"
            title="Edit"
            onClick={() => handleEdit(row)}
          >
            <img src={EditIcon} alt="Edit" />
          </button>

          <button
            className="delete"
            disabled={deletingId === row.id}
            onClick={() => handleDelete(row.id)}
          >
            <img src={DeleteIcon} alt="Delete" />
          </button>
        </div>
      ),
    },
  ];

  return (
    <>
      <div id="inventory-contents">
        <div id="inventory-content-left" className="d-flex">
          <div id='top-text'>
            <h2>Inventory Management</h2>
            <h5>Keep track of your products and stock levels.</h5>
          </div>
          
          <div className="input-group flex-direction row">
            <div>
              <DataTable
                columns={InventoryColumns}
                data={inventoryItems}
                enableSearch
                search={search}
                onSearchChange={setSearch}
                enableColumnFilter
              />
              </div>
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
              <h4>{editingItem ? "Edit Item" : "Add New Item"}</h4>
              <p>
                {editingItem
                  ? "Update the selected inventory item."
                  : "Create a new inventory item."}
              </p>
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
                checked={formData.isForSale}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    isForSale: e.target.checked,
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
                  value={formData.cost}
                  onChange={(e) => handlePriceChange("cost", e.target.value)}
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
                options={enums.units}
                value={formData.unitOfMeasurement}
                onSelect={(value) =>
                  setFormData({
                    ...formData,
                    unitOfMeasurement: value,
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

                selected={formData.expirationDate}
                onChange={(date) =>
                  setFormData({
                    ...formData,
                    expirationDate: date,
                  })
                }
              />
            </div>

            <div className="mb-2 d-flex flex-direction row">
                <label className="form-label">Tags</label>
                <div className="d-flex flex-wrap gap-2">
                  {enums.tags.map((tag) => (
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
                type="button"
                onClick={editingItem ? handleUpdate : handleCreate}
              >
                {editingItem ? "Update Item" : "Create Item"}
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
    </>
  );
}

export default Dashboard;