import { request } from "./client";

export const SalesAPI = {
  create: (data) =>
    request("/sales/create", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  get: (id) =>
    request("/sales/get", {
      method: "POST",
      body: JSON.stringify({ id }),
    }),

  find: (filters) =>
    request("/sales/find", {
      method: "POST",
      body: JSON.stringify(filters),
    }),

  delete: (id) =>
    request("/sales/delete", {
      method: "POST",
      body: JSON.stringify({ id }),
    }),
};