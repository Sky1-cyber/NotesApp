import { defineStore } from "pinia";
import { computed, ref } from "vue";
import { authApi } from "../api/auth.api";

export const useAuthStore = defineStore("auth", () => {
    const token = ref<string | null>(localStorage.getItem("token"));
    const username = ref<string | null>(localStorage.getItem("username"));
    const userId = ref<number | null>(Number(localStorage.getItem("userId")) || null);

    const isAuthenticated = computed(() => Boolean(token.value));

    function persist(res: { token: string; username: string; userId: number }) {
        token.value = res.token;
        username.value = res.username;
        userId.value = res.userId;
        localStorage.setItem("token", res.token);
        localStorage.setItem("username", res.username);
        localStorage.setItem("userId", String(res.userId));
    }

    async function login(u: string, p: string) {
        const res = await authApi.login(u, p);
        persist(res);
    }

    async function register(u: string, p: string) {
        const res = await authApi.register(u, p);
        persist(res);
    }

    function logout() {
        token.value = null;
        username.value = null;
        userId.value = null;
        localStorage.removeItem("token");
        localStorage.removeItem("username");
        localStorage.removeItem("userId");
    }

    return { token, username, userId, isAuthenticated, login, register, logout };
});