import { request } from "./client";

export const AuthAPI = {
  login: (username, password) =>
    request("/auth/login", {
      method: "POST",
      body: JSON.stringify({
        username,
        password,
      }),
    }),
};