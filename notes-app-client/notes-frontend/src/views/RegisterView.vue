<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { AlertCircle, NotebookPen } from "lucide-vue-next";
import { useAuthStore } from "../stores/auth";
import BaseButton from "../components/BaseButton.vue";
import BaseInput from "../components/BaseInput.vue";

const auth = useAuthStore();
const router = useRouter();

const username = ref("");
const password = ref("");
const error = ref("");
const loading = ref(false);

async function submit() {
  error.value = "";
  loading.value = true;
  try {
    await auth.register(username.value, password.value);
    router.push({ name: "notes" });
  } catch (e: any) {
    error.value = e?.response?.data?.message ?? "Registration failed.";
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-50 p-4">
    <div class="w-full max-w-md">
      <div class="flex flex-col items-center mb-8">
        <div class="w-12 h-12 rounded-xl bg-brand-600 flex items-center justify-center shadow-lg shadow-brand-600/20">
          <NotebookPen class="w-6 h-6 text-white" :stroke-width="2" />
        </div>
        <h1 class="mt-4 text-2xl font-semibold tracking-tight text-gray-900">
          Create your account
        </h1>
        <p class="text-sm text-gray-500 mt-1">Start capturing ideas in seconds.</p>
      </div>

      <div class="bg-white rounded-2xl shadow-card border border-gray-200 p-6 sm:p-8">
        <form @submit.prevent="submit" class="space-y-5">
          <BaseInput
              v-model="username"
              label="Username"
              hint="At least 3 characters"
              autocomplete="username"
              required
              autofocus
          />
          <BaseInput
              v-model="password"
              label="Password"
              type="password"
              hint="At least 6 characters"
              autocomplete="new-password"
              required
          />

          <div
              v-if="error"
              class="flex items-start gap-2 rounded-lg bg-red-50 border border-red-100 px-3 py-2.5 text-sm text-red-700"
          >
            <AlertCircle class="w-4 h-4 mt-0.5 shrink-0" />
            <span>{{ error }}</span>
          </div>

          <BaseButton type="submit" :loading="loading" block size="lg">
            {{ loading ? "Creating..." : "Create account" }}
          </BaseButton>
        </form>
      </div>

      <p class="text-center text-sm text-gray-500 mt-6">
        Already have an account?
        <router-link
            to="/login"
            class="font-medium text-brand-600 hover:text-brand-700 transition-colors"
        >
          Sign in
        </router-link>
      </p>
    </div>
  </div>
</template>