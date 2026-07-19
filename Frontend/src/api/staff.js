import { request } from "./client";

export const StaffAPI = {

    get(id) {
        return request("/staff/get", {
            method: "POST",
            body: JSON.stringify({ id })
        });
    },

    find(filters = {}) {
        return request("/staff/find", {
            method: "POST",
            body: JSON.stringify(filters)
        });
    },

    create(staff) {
        return request("/staff/create", {
            method: "POST",
            body: JSON.stringify(staff)
        });
    },

    update(staff) {
        return request("/staff/update", {
            method: "POST",
            body: JSON.stringify(staff)
        });
    },

    delete(id) {
        return request("/staff/delete", {
            method: "POST",
            body: JSON.stringify({ id })
        });
    }

};