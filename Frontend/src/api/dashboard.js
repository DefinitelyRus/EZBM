import { request } from "./client";

export const DashboardAPI = {
  getAnalytics() {
    return request("/dashboard/analytics");
  },

  getRecentSales() {
    return request("/sales/find", {
      method: "POST",
      body: JSON.stringify({}),
    });
  },

  getSale(id) {
    return request("/sales/get", {
      method: "POST",
      body: JSON.stringify({ id }),
    });
  },
};