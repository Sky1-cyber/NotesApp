import http from "./http";
import type { AuthResponse } from "@/types";

export const authApi = {
    register: (username: string, password: string) =>
        http
            .post<AuthResponse>("/auth/register", { username, password })
            .then((r) => r.data),

    login: (username: string, password: string) =>
        http
            .post<AuthResponse>("/auth/login", { username, password })
            .then((r) => r.data),
};