import { useState, useEffect } from "react";
import { StaffAPI } from "../../api/staff";
import { EnumsAPI } from "../../api/enums";
import './Staff.css';

import Dropdown from "../../components/Dropdown";
import DataTable from "../../components/DataTable/DataTable";
import { dropdownOptions } from "../../components/dropdownOptions";

import CloseMenu from "../../assets/arrow_menu.svg?react";
import SearchIcon from "../../assets/search.svg?react";
import AddIcon from "../../assets/add.svg";

const BREAKPOINT = 1600;

  const INITIAL_FORM = {
    firstName: "",
    lastName: "",
    username: "",
    password: "",
    email: "",
    phoneNumber: "",
    position: "",
    payFrequency: "",
  };
 
function Staff() {

  /* ---------- STATES ---------- */
  
  // Inventory
  const [search, setSearch] = useState("");

  // Enums
  const [enums, setEnums] = useState({
      payFrequencies: [],
  });

  // UI
  const [showAddItem, setShowAddItem] = useState(true);

  // Editing / Deleting
   const [formData, setFormData] = useState(INITIAL_FORM);

  /* ---------- DERIVED VALUES ---------- */

  useEffect(() => {
    async function loadEnums() {
        try {
            const data = await EnumsAPI.getAll();

            setEnums({
                payFrequencies: data.payFrequencies,
            });
        } catch (err) {
            console.error(err);
        }
    }

    loadEnums();
}, []);
  
  /* ---------- EFFECTS ---------- */
  
  // Responsive add-item panel
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
  
    /* ---------- HELPRES---------- */

    const handleClear = () => {
      setFormData(INITIAL_FORM);
    };
  
    const toggleTag = (tag) => {
      setFormData((prev) => ({
        ...prev,
        tags: prev.tags.includes(tag)
          ? prev.tags.filter((t) => t !== tag)
          : [...prev.tags, tag],
      }));
    };

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

    /* ---------- STAFF CRUD ---------- */

    // Create Employee
    const handleRegisterEmployee = async () => {
      const requiredFields = [
        "username",
        "password",
        "firstName",
        "lastName",
        "email",
        "phoneNumber",
        "position",
        "payFrequency",
      ];

      // Find the first missing field
      const missingField = requiredFields.find(
        (field) =>
          !formData[field] ||
          (typeof formData[field] === "string" && formData[field].trim() === "")
      );

      if (missingField) {
        alert(`Please fill in the ${missingField} field.`);
        return;
      }

      try {
        await StaffAPI.create({
          username: formData.username,
          password: formData.password,
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          phoneNumber: formData.phoneNumber,
          position: formData.position,
          payFrequency: formData.payFrequency,

          // Defaults
          payRate: 0,
          rfidCardId: "",
          permissions: [],
          permissionsAfterExpiry: [],
          expirationDate: null,
          roleIds: [],
        });

        alert("Employee registered successfully!");
        handleClear();

      } catch (err) {
        console.error(err);
        alert("Failed to register employee.");
      }
    };

  /* ---------- TABLE ---------- */
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
      width: "18%",
      className: "col-left",
    },
    {
      key: "username",
      label: "Username",
      width: "15%",
      className: "col-center",
    },
    {
      key: "email",
      label: "Email",
      width: "22%",
      className: "col-center",
    },
    {
      key: "phoneNumber",
      label: "Phone",
      width: "14%",
      className: "col-center",
    },
    {
      key: "position",
      label: "Position",
      width: "14%",
      className: "col-center",
    },
    {
      key: "payRate",
      label: "Pay Rate",
      width: "10%",
      className: "col-center",
      render: (row) =>
        row.payRate != null ? `₱${row.payRate}` : "-",
    },
    {
      key: "actions",
      label: "",
      width: "7%",
      className: "col-right-btn",
      hideable: false, 
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
            <div>
              <DataTable
                columns={StaffColumns}
                data={[]}
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

        <div className="staff-panel-content">
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
                      title="Select Pay Frequency"
                      options={enums.payFrequencies}
                      value={formData.payFrequency}
                      onSelect={(value) =>
                          setFormData(prev => ({
                              ...prev,
                              payFrequency: value,
                          }))
                      }
                  />
                </div>
              </div>

            </div>
          </form>
        </div>

        <div className="d-flex justify-content-center gap-3 item-btn-group">
            <button
              id="create-item"
              className="btn btn-light align-self-center"
              type="button"
              onClick={handleRegisterEmployee}
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