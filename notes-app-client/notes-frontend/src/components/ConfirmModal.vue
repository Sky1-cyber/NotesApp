<script setup lang="ts">
import { AlertTriangle } from "lucide-vue-next";
import BaseModal from "./BaseModal.vue";
import BaseButton from "./BaseButton.vue";

defineProps<{
  open: boolean;
  title?: string;
  message?: string;
  confirmText?: string;
  loading?: boolean;
}>();
const emit = defineEmits<{ (e: "confirm"): void; (e: "cancel"): void }>();
</script>

<template>
  <BaseModal :open="open" size="sm" @close="emit('cancel')">
    <div class="flex gap-4">
      <div class="w-10 h-10 rounded-full bg-red-50 flex items-center justify-center shrink-0">
        <AlertTriangle class="w-5 h-5 text-red-600" />
      </div>
      <div class="pt-0.5">
        <h3 class="text-base font-semibold text-gray-900">
          {{ title ?? "Delete note?" }}
        </h3>
        <p class="text-sm text-gray-600 mt-1">
          {{ message ?? "This action cannot be undone." }}
        </p>
      </div>
    </div>

    <template #footer>
      <div class="flex items-center justify-end gap-2">
        <BaseButton variant="secondary" @click="emit('cancel')">Cancel</BaseButton>
        <BaseButton variant="danger" :loading="loading" @click="emit('confirm')">
          {{ confirmText ?? "Delete" }}
        </BaseButton>
      </div>
    </template>
  </BaseModal>
</template>