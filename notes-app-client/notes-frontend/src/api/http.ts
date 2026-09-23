import axios, { AxiosError } from "axios";

const http = axios.create({
    baseURL: import.meta.env.VITE_API_URL ?? "http://localhost:5000/api",
    headers: { "Content-Type": "application/json" },
    timeout: 15000,
});

http.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
});

http.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        if (error.response?.status === 401) {
            localStorage.removeItem("token");
            localStorage.removeItem("username");
            localStorage.removeItem("userId");
            if (window.location.pathname !== "/login") {
                window.location.href = "/login";
            }
        }
        return Promise.reject(error);
    }
);

export default http;