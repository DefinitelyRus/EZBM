import { useState, useEffect } from "react";
import { InventoryAPI } from "../../api/inventory";
import { SalesAPI } from "../../api/sales";
import { EnumsAPI } from "../../api/enums";
import { useMemo } from "react";
import './Checkout.css';

import Dropdown from '../../components/Dropdown/Dropdown';
import DataTable from "../../components/DataTable/DataTable";

import CloseMenu from "../../assets/arrow_menu.svg?react";
import SearchIcon from "../../assets/search.svg?react";
import CheckoutIcon from "../../assets/checkout.svg";
import AddCartIcon from "../../assets/add_cart.svg";

  const BREAKPOINT = 1600;

  const INITIAL_FORM = {
  cartItems: [],
  promoCode: "",
  paymentMethod: "",
  transactionNotes: "",
  };

function Checkout() {

  /* ---------- STATES ---------- */

  // Inventory
  const [inventoryItems, setInventoryItems] = useState([]);
  const [search, setSearch] = useState("");
  
  // Cart
  const [cart, setCart] = useState([]);

  // Enum
  const [enums, setEnums] = useState({
    paymentMethods: [],
  });

  // Form
  const [formData, setFormData] = useState(INITIAL_FORM);

  // UI
  const [showAddItem, setShowAddItem] = useState(true);
  const [error, setError] = useState("");
  
  // Editing / Deleting

  /* ---------- EFFECTS ---------- */

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

  // Load inv
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

  // Load payment methods
  useEffect(() => {
    const loadEnums = async () => {
      try {
        const data = await EnumsAPI.getAll();

        setEnums({
          paymentMethods: data.paymentMethods ?? [],
        });
      } catch (err) {
        console.error(err);
      }
    };

    loadEnums();
  }, []);

  /* ---------- HELPRES---------- */

  // Add to Cart
  const addToCart = (item) => {
    setCart((prev) => {
      const existing = prev.find((i) => i.id === item.id);

      if (existing) {
        return prev.map((i) =>
          i.id === item.id
            ? {
                ...i,
                quantity: i.quantity + 1,
                subtotal: (i.quantity + 1) * i.unitPrice,
              }
            : i
        );
      }

      return [
        ...prev,
        {
          id: item.id,
          name: item.name,

          quantity: 1,

          unitPrice: Number(item.salePrice),   
          subtotal: Number(item.salePrice),   
        },
      ];
    });
  };

  // Add Quantity
  const increaseQuantity = (id) => {
    setCart((prev) =>
      prev.map((item) =>
        item.id === id
          ? {
              ...item,
              quantity: item.quantity + 1,
              subtotal: (item.quantity + 1) * item.unitPrice,
            }
          : item
      )
    );
  };

  // Subtract Quantity
  const decreaseQuantity = (id) => {
  setCart((prev) =>
    prev
      .map((item) => {
        if (item.id !== id) return item;

        if (item.quantity === 1) {
          return null;
        }

        return {
          ...item,
          quantity: item.quantity - 1,
          subtotal: (item.quantity - 1) * item.unitPrice,
        };
      })
      .filter(Boolean)
  );
};

  // Remove item
  const removeFromCart = (id) => {
    setCart((prev) => prev.filter((item) => item.id !== id));
  };

  const handleClear = () => {
    setFormData(INITIAL_FORM);
    setCart([]);
  };

  // Field validation
  const validateCheckout = () => {
    if (cart.length === 0) {
      setError(`Please add at least one item to the cart`);
      return false;
    }

    if (
      formData.paymentMethod === "" ||
      formData.paymentMethod === null ||
      formData.paymentMethod === undefined
    ) {
      setError(`Please select a payment method`);
      return false;
    }

    for (const cartItem of cart) {
      if (cartItem.quantity <= 0) {
        setError(`${cartItem.name} has an invalid quantity.`);
        return false;
      }

      const inventoryItem = inventoryItems.find(
        (item) => item.id === cartItem.id
      );

      if (!inventoryItem) {
        setError(`${cartItem.name} no longer exists in inventory.`);
        return false;
      }

      if (cartItem.quantity > Number(inventoryItem.quantity)) {
        alert(
          `Not enough stock for ${cartItem.name}. Only ${inventoryItem.quantity} remaining.`
        );
        setError(`Not enough stock for ${cartItem.name}. Only ${inventoryItem.quantity} remaining.`);
        return false;
      }

      if (cartItem.unitPrice <= 0) {
        alert(`${cartItem.name} has an invalid selling price.`);
        return false;
      }
    }

    return true;
  };

  const totalAmount = cart.reduce(
    (total, item) => total + item.unitPrice * item.quantity,
    0
  );

  // Checkout 
 const handleCheckout = async () => {
    if (!validateCheckout()) return;

    try {
      const payload = {
        staffId: localStorage.getItem("staffId"),

        paymentMethod: Number(formData.paymentMethod),

        totalAmount: Number(totalAmount),

        notes: formData.transactionNotes,

        promoCode:
          formData.promoCode.trim() === ""
            ? null
            : formData.promoCode.trim(),

        customerId: null,

        items: cart.map(item => ({
          itemId: item.id,
          quantity: Number(item.quantity),
          unitPrice: Number(item.unitPrice),
        })),

        splitPayments: [],
      };

      console.log("Checkout payload:", payload);

      const result = await SalesAPI.create(payload);

      const inventory = await InventoryAPI.getAll();
      setInventoryItems(inventory);

      setCart([]);
      setFormData(INITIAL_FORM);

      alert(`Checkout successful! Sale #${result.saleId}`);

    } catch (err) {
      console.error(err);
      alert("Checkout failed.");
    }
  };

  /* ---------- TABLES ---------- */

  const ForSaleColumns = [
    {
      key: "name",
      label: "Name",
      filterLabel: "Name",
      width: "28%",
      className: "col-left",
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
      label: "Stock",
      filterLabel: "Stock",
      width: "9%",
      className: "col-center",
    },
    {
      key: "unitOfMeasurement",
      label: "Unit",
      filterLabel: "Unit",
      width: "10%",
      className: "col-center",
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
      key: "actions",
      label: "",
      filterLabel: "Actions",
      hideable: false,
      width: "7%",
      className: "col-right-btn",
      render: (row) => (
        <div className="d-flex justify-content-end">
          <button
            className="addCart"
            title="Add To Cart"
            onClick={() => addToCart(row)}
          >
            <img src={AddCartIcon} alt="Add" />
          </button>
        </div>
      ),
    }
  ];

  const CartColumns = [
    {
      key: "name",
      label: "Item",
      width: "40%",
    },
    {
      key: "quantity",
      label: "Qty",
      width: "25%",
      className: "col-center",
      render: (row) => (
        <div className="qty-controls">
          <button
            className="qty-btn"
            onClick={() => decreaseQuantity(row.id)}
          >
            −
          </button>

          <span className="qty-value">
            {row.quantity}
          </span>

          <button
            className="qty-btn"
            onClick={() => increaseQuantity(row.id)}
          >
            +
          </button>
        </div>
      ),
    },
    {
      key: "salePrice",
      label: <>Unit<br></br>Price</>,
      width: "20%",
      className: "col-center",
       render: (row) => `₱${row.unitPrice.toFixed(2)}`,
    },
    {
      key: "total",
      label: "Total",
      width: "20%",
      className: "col-center",
      render: (row) =>
        `₱${(row.unitPrice * row.quantity).toFixed(2)}`,
    },
    {
      key: "actions",
      label: "",
      width: "5%",
      className: "col-right",
      render: (row) => (
        <button
          className="removeItem"
          onClick={() => removeFromCart(row.id)}
        >
          ✕
        </button>
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
            <div>
              <DataTable
                columns={ForSaleColumns}
                data={inventoryItems.filter(item => item.isForSale)}
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
        
        <div className="cart-panel-content">
          <div>
            <DataTable
              columns={CartColumns}
              data={cart}
              showFooter={false}
            />
          </div>

         <div className="d-flex flex-direction col total-amount">
            <h5>Total Amount:</h5>

            <h4 className="total-price">
              ₱{totalAmount.toFixed(2)}
            </h4>
          </div>

          <div id="item-inputs">
            <div className="mb-3">
                <label className="form-label">
                  Method of Payment <span className="required">*</span>
                </label>
                <Dropdown
                  title="Select Method"
                  options={enums.paymentMethods}
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
               {error && (
                <div className="form-error">
                  {error}
                </div>
              )}
              <div className="d-flex justify-content-center gap-3 item-btn-group">
                <button
                  id="create-item"
                  className="btn btn-light align-self-center"
                  onClick={handleCheckout}
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