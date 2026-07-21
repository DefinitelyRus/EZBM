import { useState } from "react";
import { AuthAPI } from "../../api/auth";
import { StaffAPI } from "../../api/staff";

import "./Login.css";

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const handleLogin = async (e) => {
    e.preventDefault();

    if (!username.trim() || !password.trim()) {
      alert("Please enter your username and password.");
      return;
    }

    setLoading(true);

    try {
      // Login
      const staff = await AuthAPI.login(username, password);

      console.log("Login Response:", staff);

      // Fetch the complete staff record
      const fullStaff = await StaffAPI.get(staff.id);

      console.log("Full Staff Record:", fullStaff);

      // Save to localStorage
      localStorage.setItem("staffId", fullStaff.id);
      localStorage.setItem("username", fullStaff.username ?? "");
      localStorage.setItem("firstName", fullStaff.firstName ?? "");
      localStorage.setItem("lastName", fullStaff.lastName ?? "");
      localStorage.setItem("position", fullStaff.position ?? "");
      localStorage.setItem("payFrequency", fullStaff.payFrequency ?? "");
      localStorage.setItem("payRate", fullStaff.payRate ?? "");

      alert(
        `Welcome, ${fullStaff.firstName ?? fullStaff.username}!`
      );

      window.location.href = "/dashboard";
    } catch (err) {
      console.error(err);
      alert("Invalid username or password.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div id="login-contents">
      <div className="card login-card">
        <div className="card-body">
          <h2>Login</h2>
          <p>Please sign in to continue.</p>

          <form onSubmit={handleLogin}>
            <div className="mb-3">
              <label className="form-label">Username</label>

              <input
                type="text"
                className="form-control"
                placeholder="Enter username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>

            <div className="mb-4">
              <label className="form-label">Password</label>

              <input
                type="password"
                className="form-control"
                placeholder="Enter password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>

            <button
              type="submit"
              className="btn btn-primary w-100"
              disabled={loading}
            >
              {loading ? "Logging in..." : "Login"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Login;