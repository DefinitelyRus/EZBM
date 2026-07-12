import { useState, useEffect } from "react";
import './Staff.css';

import Dropdown from "../../components/Dropdown";
import DataTable from "../../components/DataTable";
import { dropdownOptions } from "../../components/dropdownOptions";

import CloseMenu from "../../assets/arrow_menu.svg?react";
import SearchIcon from "../../assets/search.svg?react";
import AddIcon from "../../assets/add.svg";

const BREAKPOINT = 1600;

function Staff() {

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
    // useEffect(() => {
    //   const timeout = setTimeout(async () => {
    //     try {
    //       const data =
    //         search.trim() === ""
    //           ? await InventoryAPI.getAll()
    //           : await InventoryAPI.find({
    //               name: search.trim(),
    //             });
  
    //       setInventoryItems(data);
    //     } catch (err) {
    //       console.error(err);
    //     }
    //   }, 300);
  
    //   return () => clearTimeout(timeout);
    // }, [search]);

    /* Clear Button */
  const initialForm = {
    firstName: "",
    lastName: "",
    username: "",
    password: "",
    email: "",
    phoneNumber: "",
    position: "",
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

  /* Tables */
    const StaffColumns = [
      {
        key: "id",
        label: "ID",
        width: "10%",
        className: "col-left",
      },
      {
        key: "name",
        label: "Name",
        width: "8%",
        className: "col-center",
        render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
      },
      {
        key: "username",
        label: "Username",
        width: "8%",
        className: "col-center",
        render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
      },
      {
        key: "email",
        label: "Email",
        width: "8%",
        className: "col-center",
        render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
      },
      {
        key: "phoneNumber",
        label: "Phone",
        width: "8%",
        className: "col-center",
        render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
      },
      {
        key: "position",
        label: "Position",
        width: "8%",
        className: "col-center",
        render: (row) => (row.salePrice ? `₱${row.salePrice}` : "-"),
      },
      {
        key: "payRate",
        label: "Pay Rate",
        width: "9%",
        className: "col-center",
      },
      {
        key: "actions",
        label: "",
        width: "7%",
        className: "col-right-btn",
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
    <>
      <div id="staff-contents">
        <div id="staff-content-left" className="d-flex">
          <div id="top-text">
            <h2>Staff Management</h2>
            <h5>Keep your staff information accurate and up to date.</h5>
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
                columns={StaffColumns}
                data={[]}
              />
            </div>
          </div>
        </div>
      </div>

      <div 
        id="add-staff-container" 
        className={`card ${showAddItem ? "panel-open" : "panel-closed"}`}
        >
        <div className="card-body">
          <div className="d-flex flex-direction col gap-3 panel-header">
            <div className="flex-direction row">
              <h4>Add New Employee</h4>
              <p>Register a new employee.</p>
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

            {/* Account Information */}
            <div className="form-section">
              <h6 className="section-title">Account Information</h6>

              <div className="usr-auth">
                <div className="mb-3">
                  <label className="form-label">Username</label>
                  <input
                    type="text"
                    className="form-control usr-input"
                    value={formData.username}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        username: e.target.value,
                      })
                    }
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">Password</label>
                  <input
                    type="password"
                    className="form-control usr-input"
                    value={formData.password}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        password: e.target.value,
                      })
                    }
                  />
                </div>
              </div>
            </div>

            {/* Personal Information */}
            <div className="form-section">
              <h6 className="section-title">Personal Information</h6>

              <div className="name-row">
                <div className="mb-3">
                  <label className="form-label">First Name</label>
                  <input
                    type="text"
                    className="form-control usr-input"
                    value={formData.firstName}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        firstName: e.target.value,
                      })
                    }
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">Last Name</label>
                  <input
                    type="text"
                    className="form-control usr-input"
                    value={formData.lastName}
                    onChange={(e) =>
                      setFormData({
                        ...formData,
                        lastName: e.target.value,
                      })
                    }
                  />
                </div>
              </div>

              <div className="mb-3">
                <label className="form-label">Email</label>
                <input
                  type="email"
                  className="form-control usr-input"
                  value={formData.email}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      email: e.target.value,
                    })
                  }
                />
              </div>

              <div className="mb-3">
                <label className="form-label">Phone Number</label>
                <input
                  type="text"
                  className="form-control usr-input"
                  value={formData.phoneNumber}
                  inputMode="numeric"
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      phoneNumber: e.target.value.replace(/\D/g, ""),
                    })
                  }
                />
              </div>
            </div>

            {/* Employment Information */}
            <div className="form-section">
              <h6 className="section-title">Employment Information</h6>

              <div className="mb-3">
                <label className="form-label">Position / Role</label>
                <input
                  type="text"
                  className="form-control usr-input"
                  value={formData.position}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      position: e.target.value,
                    })
                  }
                />
              </div>

              <div className="mb-3">
                <label className="form-label">Pay Frequency</label>
                <Dropdown
                  title="Select Frequency"
                  options={dropdownOptions.payFrequency}
                  value={formData.payFrequency}
                  onSelect={(value) =>
                    setFormData({
                      ...formData,
                      payFrequency: value,
                    })
                  }
                />
              </div>
            </div>

          </div>
        </form>

        <div className="d-flex justify-content-center gap-3 item-btn-group">
            <button
              id="create-item"
              className="btn btn-light align-self-center"
            >
              Register Employee
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
          <span>Add New Employee</span>
        </button>
    </>
  );
}

export default Staff;