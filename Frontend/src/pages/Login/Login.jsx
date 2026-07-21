import { useState } from "react";
import { AuthAPI } from "../../api/auth";

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
      const staff = await AuthAPI.login(username, password);

      console.log("API ID:", staff.id);
      console.log("TYPE:", typeof staff.id);

      localStorage.setItem("staffId", staff.id);

      console.log(
        "Stored ID:",
        localStorage.getItem("staffId"),
        typeof localStorage.getItem("staffId")
      );

      localStorage.setItem("username", staff.username);
      localStorage.setItem("position", staff.position);
      localStorage.setItem("payFrequency", staff.payFrequency);
      localStorage.setItem("payRate", staff.payRate);

      alert(`Welcome, ${staff.username}!`);

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