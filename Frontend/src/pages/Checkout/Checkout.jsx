import { useState, useEffect } from "react";
import { InventoryAPI } from "../../api/inventory";
import './Checkout.css';

import Dropdown from '../../components/Dropdown';
import DataTable from "../../components/DataTable";
import { dropdownOptions } from "../../components/dropdownOptions";

import CloseMenu from "../../assets/arrow_menu.svg?react";
import SearchIcon from "../../assets/search.svg?react";
import CheckoutIcon from "../../assets/checkout.svg";
import AddCartIcon from "../../assets/add_cart.svg";

const BREAKPOINT = 1600;

function Checkout() {

  const [inventoryItems, setInventoryItems] = useState([]);
  const [showAddItem, setShowAddItem] = useState(true);
  const [search, setSearch] = useState("");

  useEffect(() => {
    const mediaQuery = window.matchMedia(`(max-width: ${BREAKPOINT - 1}px)`);

    const handleChange = ({ matches }) => {
      // Hide the panel on small screens
      setShowAddItem(!matches);
    };

    // Set initial state
    handleChange(mediaQuery);

    mediaQuery.addEventListener("change", handleChange);

    return () =>
      mediaQuery.removeEventListener("change", handleChange);
  }, []);


  /* Search Bar */
  useEffect(() => {
    const timeout = setTimeout(async () => {
      try {
        const data =
          search.trim() === ""
            ? await InventoryAPI.getAll()
            : await InventoryAPI.find({
                name: search.trim(),
              });

        setInventoryItems(data);
      } catch (err) {
        console.error(err);
      }
    }, 300);

    return () => clearTimeout(timeout);
  }, [search]);

  /* Clear Button */
  const initialForm = {
  cartItems: [],
  promoCode: "",
  paymentMethod: "",
  transactionNotes: "",
  };

  const [formData, setFormData] = useState(initialForm);

  const handleClear = () => {
    setFormData(initialForm);
  };

/* Tables */
  const ForSaleColumns = [
    {
      key: "name",
      label: "Name",
      width: "28%",
      className: "col-left",
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
      label: "Stock",
      width: "9%",
      className: "col-center",
    },
    {
      key: "unitOfMeasurement",
      label: "Unit",
      width: "10%",
      className: "col-center",
    },
    {
      key: "expirationDate",
      label: "Expiration",
      width: "12%",
      className: "col-center",
      render: (row) =>
        row.expirationDate
          ? new Date(row.expirationDate).toLocaleDateString("en-US")
          : "-",
    },
    {
      key: "actions",
      label: "",
      width: "7%",
      className: "col-right-btn",
      render: () => (
        <div className="d-flex justify-content-end">
          <button className="addCart" title="Add To Cart">
            <img src={AddCartIcon} alt="Add" />
          </button>
        </div>
      ),
    },
  ];

  const CartColumns = [
    {
      key: "name",
      label: "Item",
      width: "28%",
      className: "col-left",
    },
    {
      key: "salePrice",
      label: "QTY",
      width: "12%",
      className: "col-center",
      render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
    },
    {
      key: "quantity",
      label: "Price",
      width: "9%",
      className: "col-center",
    },
    {
      key: "unitOfMeasurement",
      label: "Total",
      width: "10%",
      className: "col-center",
    },
    {
      key: "actions",
      label: "",
      width: "7%",
      className: "col-right-btn",
      render: () => (
        <div className="d-flex justify-content-end">
          <button className="addCart" title="Remove">
            ✕
          </button>
        </div>
      ),
    },
  ];

  return (
    <>
      <div id="checkout-contents">
        <div id="checkout-content-left" className="d-flex">
          <div id="top-text">
            <h2>Point-of-Sale Checkout</h2>
            <h5>Process customer transactions quickly.</h5>
          </div>
          
          <div className="input-group flex-direction row">
            <div className="search-bar-container">
              <div className="search-bar">
                <SearchIcon className="search-icon" />

                <input
                  type="text"
                  className="search-input"
                  placeholder="Type in to filter..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                />

                {search && (
                  <button
                    type="button"
                    className="clear-search-btn"
                    onClick={() => setSearch("")}
                  >
                    ✕
                  </button>
                )}
              </div>
            </div>

            <div>
              <DataTable
                columns={ForSaleColumns}
                data={inventoryItems.filter(item => item.isForSale)}
              />
            </div>
          </div>
        </div>
      </div>
      
      <div 
        id="checkout-items-container" 
        className={`card ${showAddItem ? "panel-open" : "panel-closed"}`}
        >
        <div className="card-body">
          <div className="d-flex flex-direction col gap-3 panel-header">
            <div className="flex-direction row">
              <h4>Add To Cart</h4>
              <p>Select an item to add to cart.</p>
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

        <div>
          <DataTable
                columns={CartColumns}
                data={inventoryItems.filter(item => item.isForSale)}
              />
        </div>

        <div className="d-flex flex-direction col total-amount">
          <h5>Total Amount:</h5>
          <h5>₱</h5>
        </div>

        <div id="item-inputs">
          <div className="mb-3">
            <label className="form-label">Promo Code:</label>
            <input
              type="text"
              className="form-control usr-input"
              placeholder="e.g. FREEWEEK"
              value={formData.promoCode}
              onChange={(e) =>
                setFormData({
                  ...formData,
                  promoCode: e.target.value,
                })
              }
            />
          </div>

          <div className="mb-3">
              <label className="form-label">Method of Payment</label>
              <Dropdown
                title="Select Method"
                options={dropdownOptions.paymentMethods}
                value={formData.paymentMethod}
                onSelect={(value) =>
                  setFormData({
                    ...formData,
                    paymentMethod: value,
                  })
                }
              />
            </div>

          <div className="mb-3">
            <label className="form-label">Transaction Notes</label>
           <textarea
              className="form-control usr-input"
              rows={2}
              value={formData.transactionNotes}
              placeholder="Optional comments..."
              style={{ resize: "none", overflow: "hidden" }}
              onChange={(e) => {
                setFormData({
                  ...formData,
                  transactionNotes: e.target.value,
                });

                e.target.style.height = "auto";
                e.target.style.height = `${e.target.scrollHeight}px`;
              }}
            />
          </div>

          <div className="d-flex justify-content-center gap-3 item-btn-group">
            <button
              id="create-item"
              className="btn btn-light align-self-center"
            >
              Complete Checkout
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
      </div>
        <button
          className={`floating-add-btn ${
            showAddItem ? "btn-hidden" : "btn-visible"
          }`}
          type="button"
          onClick={() => setShowAddItem(true)}
        >
          <img src={CheckoutIcon} alt="" />
          <span>Open Cart</span>
        </button>
    </>
  );
}

export default Checkout;