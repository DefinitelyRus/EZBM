import { request } from "./client";

export const InventoryAPI = {

    getAll() {
        return request("/items");
    },

    get(id) {
        return request("/items/get", {
            method: "POST",
            body: JSON.stringify({ id })
        });
    },

    create(item) {
        return request("/items/create", {
            method: "POST",
            body: JSON.stringify(item)
        });
    },

    update(item) {
        return request("/items/update", {
            method: "POST",
            body: JSON.stringify(item)
        });
    },

    delete(id) {
        return request("/items/delete", {
            method: "POST",
            body: JSON.stringify({ id })
        });
    },

    find(filters) {
        return request("/items/find", {
            method: "POST",
            body: JSON.stringify(filters)
        });
    }

};