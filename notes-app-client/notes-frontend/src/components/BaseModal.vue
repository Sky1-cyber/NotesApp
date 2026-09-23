<script setup lang="ts">
import { onBeforeUnmount, onMounted, watch } from "vue";
import { X } from "lucide-vue-next";

const props = defineProps<{
  open: boolean;
  title?: string;
  description?: string;
  size?: "sm" | "md" | "lg";
  closeOnBackdrop?: boolean;
}>();

const emit = defineEmits<{ (e: "close"): void }>();

const sizes = {
  sm: "max-w-sm",
  md: "max-w-lg",
  lg: "max-w-2xl",
} as const;

function onKey(e: KeyboardEvent) {
  if (e.key === "Escape" && props.open) emit("close");
}

onMounted(() => window.addEventListener("keydown", onKey));
onBeforeUnmount(() => window.removeEventListener("keydown", onKey));

// Lock body scroll while open
watch(
    () => props.open,
    (open) => {
      document.body.style.overflow = open ? "hidden" : "";
    },
    { immediate: true }
);
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
          v-if="open"
          class="fixed inset-0 z-50 flex items-end sm:items-center justify-center p-0 sm:p-4"
          role="dialog"
          aria-modal="true"
      >
        <!-- Backdrop -->
        <div
            class="absolute inset-0 bg-gray-900/50 backdrop-blur-sm"
            @click="closeOnBackdrop !== false && emit('close')"
        />

        <!-- Panel -->
        <div
            :class="[
            'relative w-full bg-white shadow-modal',
            'rounded-t-2xl sm:rounded-2xl',
            'max-h-[92vh] sm:max-h-[90vh] overflow-y-auto',
            'transform transition-transform duration-200',
            sizes[size ?? 'md'],
          ]"
        >
          <!-- Header -->
          <div
              v-if="title || $slots.header"
              class="flex items-start justify-between gap-4 px-6 pt-5 pb-4 border-b border-gray-100"
          >
            <div>
              <slot name="header">
                <h2 class="text-lg font-semibold text-gray-900">{{ title }}</h2>
                <p v-if="description" class="text-sm text-gray-500 mt-0.5">
                  {{ description }}
                </p>
              </slot>
            </div>
            <button
                type="button"
                class="p-1.5 -m-1.5 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 transition-colors"
                aria-label="Close dialog"
                @click="emit('close')"
            >
              <X class="w-5 h-5" />
            </button>
          </div>

          <!-- Body -->
          <div class="px-6 py-5">
            <slot />
          </div>

          <!-- Footer -->
          <div
              v-if="$slots.footer"
              class="px-6 py-4 bg-gray-50 border-t border-gray-100 rounded-b-2xl"
          >
            <slot name="footer" />
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.2s ease;
}
.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
.modal-enter-active > div:last-child {
  transition: transform 0.2s ease;
}
.modal-enter-from > div:last-child {
  transform: translateY(8px);
}
</style>