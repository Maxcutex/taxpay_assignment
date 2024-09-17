import axios from "axios";

export const api = axios.create({
  baseURL: "http://localhost:5001/",
  timeout: 50000, //TODO: get from env
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    const token = window.localStorage.getItem("accessToken");

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    } else {
      delete api.defaults.headers.common.Authorization;
    }

    return config;
  },

  (error) => Promise.reject(error)
);
