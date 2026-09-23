import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "../stores/auth";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: "/login",
            name: "login",
            component: () => import("../views/LoginView.vue"),
            meta: { public: true },
        },
        {
            path: "/register",
            name: "register",
            component: () => import("../views/RegisterView.vue"),
            meta: { public: true },
        },
        {
            path: "/",
            name: "notes",
            component: () => import("../views/NotesView.vue"),
        },
        { path: "/:pathMatch(.*)*", redirect: "/" },
    ],
});

router.beforeEach((to) => {
    const auth = useAuthStore();
    if (!to.meta.public && !auth.isAuthenticated) return { name: "login" };
    if (to.meta.public && auth.isAuthenticated) return { name: "notes" };
});

router.afterEach((to) => {
    const titles: Record<string, string> = {
        login: "Sign in · Notes",
        register: "Create account · Notes",
        notes: "All notes · Notes",
    };
    document.title = titles[to.name as string] ?? "Notes";
});

export default router;