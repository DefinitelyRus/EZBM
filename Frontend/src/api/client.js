const BASE_URL = "http://localhost:5056/api";

export async function request(endpoint, options = {}) {

    const username = localStorage.getItem("username");

    const response = await fetch(`${BASE_URL}${endpoint}`, {
        headers: {
            "Content-Type": "application/json",
            "X-Operator-Username": username,
            ...options.headers
        },
        ...options
    });

    if (!response.ok) {
        throw new Error(`Request failed (${response.status})`);
    }

    const text = await response.text();
    return text ? JSON.parse(text) : null;
}