  import { useState, useEffect } from "react";
  import { StaffAPI } from "../../api/staff";
  import { EnumsAPI } from "../../api/enums";
  import './Staff.css';

  import Dropdown from "../../components/Dropdown/Dropdown";
  import DataTable from "../../components/DataTable/DataTable";

  import CloseMenu from "../../assets/arrow_menu.svg?react";
  import SearchIcon from "../../assets/search.svg?react";
  import DeleteIcon from "../../assets/delete.svg";
  import EditIcon from "../../assets/edit.svg";
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
      payRate: "",
      payFrequency: "",
    };
  
  function Staff() {

    /* ---------- STATES ---------- */
    
    // Inventory
    const [staff, setStaff] = useState([]);
    const [search, setSearch] = useState("");

    // Form
    const [formData, setFormData] = useState(INITIAL_FORM);

    // Enums
    const [enums, setEnums] = useState({
        payFrequencies: [],
    });
    const enumName = (list, value) => list?.[value] ?? "-";

    // UI
    const [showAddItem, setShowAddItem] = useState(true);

    // Editing / Deleting
    const [editingStaff, setEditingStaff] = useState(null);
    const [deletingId, setDeletingId] = useState(null);

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

      useEffect(() => {
        loadStaff();
      }, []);
    
    /* ---------- HELPRES---------- */

    const handleRateChange = (field, value) => {
      if (/^\d*\.?\d{0,2}$/.test(value) || value === "") {
        setFormData(prev => ({
          ...prev,
          [field]: value,
        }));
      }
    };

    const handleClear = () => {
      setEditingStaff(null);
      setFormData(INITIAL_FORM);
    };

      /* Search Bar */
      useEffect(() => {
        loadStaff();
      }, []);

      // Search debounce
      useEffect(() => {
        const timeout = setTimeout(() => {
          loadStaff(search);
        }, 300);

        return () => clearTimeout(timeout);
      }, [search]);

    /* ---------- STAFF CRUD ---------- */

      // Load Staff
      const loadStaff = async (searchTerm = search) => {
        try {
          const data =
            searchTerm.trim() === ""
              ? await StaffAPI.find({})
              : await StaffAPI.find({
                  username: searchTerm.trim(),
                });

          setStaff(data);
        } catch (err) {
          console.error(err);
        }
      };

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
          "payRate",
          "payFrequency",
        ];

        // Find the first missing field
        const missingField = requiredFields.find((field) => {
          const value = formData[field];

          return (
            value === null ||
            value === undefined ||
            (typeof value === "string" && value.trim() === "")
          );
        });

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
            payRate: Number(formData.payRate),
            payFrequency: formData.payFrequency,

            rfidCardId: "",
            permissions: [],
            permissionsAfterExpiry: [],
            expirationDate: null,
            roleIds: [],
          });

          await loadStaff(search);

          alert("Employee registered successfully!");
          handleClear();

        } catch (err) {
          console.error(err);
          alert("Failed to register employee.");
        }
      };

      // Edit Employee
      const handleEdit = (employee) => {
        setEditingStaff(employee);

        setFormData({
          username: employee.username ?? "",
          password: "", // don't populate passwords
          firstName: employee.firstName ?? "",
          lastName: employee.lastName ?? "",
          email: employee.email ?? "",
          phoneNumber: employee.phoneNumber ?? "",
          position: employee.position ?? "",
          payRate: employee.payRate ?? "",
          payFrequency: employee.payFrequency ?? "",
        });

        setShowAddItem(true);
      };

      // Handle Update
      const handleUpdate = async () => {
        try {
          await StaffAPI.update({
            id: editingStaff.id,
            username: formData.username,
            firstName: formData.firstName,
            lastName: formData.lastName,
            email: formData.email,
            phoneNumber: formData.phoneNumber,
            position: formData.position,
            payRate: Number(formData.payRate),
            payFrequency: formData.payFrequency,

            // keep defaults for now
            rfidCardId: editingStaff.rfidCardId ?? "",
            permissions: editingStaff.permissions ?? [],
            permissionsAfterExpiry: editingStaff.permissionsAfterExpiry ?? [],
            expirationDate: editingStaff.expirationDate ?? null,
            roleIds: editingStaff.roleIds ?? [],
          });

          await loadStaff(search);

          alert("Employee updated successfully!");

          setEditingStaff(null);
          handleClear();
        } catch (err) {
          console.error(err);
          alert("Failed to update employee.");
        }
      };

      // Delete Employee
      const handleDelete = async (id) => {
        if (!window.confirm("Delete this employee?")) return;

        try {
          setDeletingId(id);

          await StaffAPI.delete(id);

          await loadStaff(search);

          alert("Employee deleted successfully.");
        } catch (err) {
          console.error(err);
          alert("Failed to delete employee.");
        } finally {
          setDeletingId(null);
        }
      };

    /* ---------- TABLE ---------- */
    const StaffColumns = [
      // {
      //   key: "id",
      //   label: "ID",
      //   width: "20%",
      //   className: "col-center",
      // },
      {
      key: "name",
      label: "Name",
      filterLabel: "Name",
      width: "15%",
      className: "col-left",
      render: (row) =>
        [row.firstName, row.lastName]
          .filter(Boolean)
          .join(" "),
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
        label: <>Pay<br />Rate</>,
        width: "10%",
        className: "col-center",
        render: (row) =>
          row.payRate != null ? `₱${row.payRate}` : "-",
      },
      {
        key: "payFrequency",
        label: <>Pay<br />Frequency</>,
        width: "10%",
        className: "col-center",
        render: (row) => 
          enumName(enums.payFrequencies, row.payFrequency),
      },
      {
        key: "actions",
        label: "",
        width: "5%",
        className: "col-right-btn",
        hideable: false,
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
              title="Delete"
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
                  data={staff}
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
                <h4>{editingStaff ? "Edit Employee Details" : "Add New Employee"}</h4>
                <p>
                  {editingStaff
                    ? "Update Employee Details"
                    : "Register a New Employee."}
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
                    <label className="form-label">Pay Rate</label>
                    <div className="input-group">
                      <span className="input-group-text currency-span">₱</span>
                      <input
                        type="text"
                        className="form-control usr-input"
                        value={formData.payRate}
                        onChange={(e) => handleRateChange("payRate", e.target.value)}
                      />
                    </div>
                  </div>

                  <div className="mb-3">
                    <label className="form-label">Pay Frequency</label>
                    <Dropdown
                        direction="down"
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

              </div>
            </form>
          </div>

          <div className="d-flex justify-content-center gap-3 item-btn-group">
              <button
                id="create-item"
                type="button"
                onClick={editingStaff ? handleUpdate : handleRegisterEmployee}
              >
                {editingStaff ? "Update Employee" : "Register Employee"}
              </button>

              <button
                id="clear-item"
                className="btn btn-light align-self-center"
                type="button"
                onClick={handleClear}
              >
                {editingStaff ? "Cancel" : "Clear"}
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