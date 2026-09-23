<script setup lang="ts">
import { useRouter } from "vue-router";
import { NotebookPen, LogOut } from "lucide-vue-next";
import { useAuthStore } from "../stores/auth";
import { useNotesStore } from "../stores/notes";
import BaseButton from "../components/BaseButton.vue";

const auth = useAuthStore();
const notesStore = useNotesStore();
const router = useRouter();

function logout() {
  auth.logout();
  notesStore.resetQuery();
  router.push({ name: "login" });
}
</script>

<template>
  <header class="sticky top-0 z-30 bg-white/90 backdrop-blur border-b border-gray-200">
    <div class="max-w-6xl mx-auto px-4 sm:px-6 h-16 flex items-center justify-between gap-4">
      <div class="flex items-center gap-2.5">
        <div class="w-9 h-9 rounded-lg bg-brand-600 flex items-center justify-center shadow-sm">
          <NotebookPen class="w-5 h-5 text-white" :stroke-width="2" />
        </div>
        <div class="leading-tight">
          <p class="font-semibold text-gray-900 text-[15px]">Notes</p>
          <p class="text-xs text-gray-500 hidden sm:block">Your personal workspace</p>
        </div>
      </div>

      <div class="flex items-center gap-3">
        <div class="hidden sm:flex items-center gap-2 pl-3 pr-1 py-1">
          <div class="w-8 h-8 rounded-full bg-brand-100 text-brand-700 flex items-center justify-center text-sm font-semibold">
            {{ auth.username?.charAt(0).toUpperCase() }}
          </div>
          <span class="text-sm font-medium text-gray-700">{{ auth.username }}</span>
        </div>
        <BaseButton variant="secondary" size="sm" @click="logout">
          <LogOut class="w-4 h-4" />
          <span class="hidden sm:inline">Logout</span>
        </BaseButton>
      </div>
    </div>
  </header>
</template>