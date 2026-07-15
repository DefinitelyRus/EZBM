import { request } from "./client";

export const EnumsAPI = {
  getAll() {
    return request("/enums");
  },

  getUnits() {
    return request("/enums/units");
  },

  getTags() {
    return request("/enums/tags");
  },

  getAccessCardTypes() {
    return request("/enums/access-card-types");
  },

  getPayFrequencies() {
    return request("/enums/pay-frequencies");
  },

  getStockTransactionTypes() {
    return request("/enums/stock-transaction-types");
  },

  getTransactionTypes() {
    return request("/enums/transaction-types");
  },

  getPaymentMethods() {
    return request("/enums/payment-methods");
  },
};
