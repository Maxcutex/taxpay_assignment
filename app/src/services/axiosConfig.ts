import axios from "axios";
import { jwtDecode } from "jwt-decode";

const instance = axios.create({
  baseURL: process.env.VITE_API_BASE_URL,
  timeout: 50000, //TODO: get from env
  headers: {
    "Content-Type": "application/json",
  },
});

const isTokenExpired = (token: string): boolean => {
  try {
    const decodedToken: { exp: number } = jwtDecode(token);
    const currentTime = Date.now() / 1000;  

    if (!decodedToken.exp) {
      return true;
    }

    return decodedToken.exp < currentTime;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
  } catch (error: any) {
    console.log("error", error);
    return true;
  }
};

const handleTokenExpiration = () => {
  localStorage.removeItem("accessToken");
  window.location.href = "/logout";
};

instance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("accessToken");

    if (token) {
      if (isTokenExpired(token)) {
        handleTokenExpiration(); // Handle token expiration (logout and redirect)
        return Promise.reject(new Error("Token expired"));
      }

      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default instance;
