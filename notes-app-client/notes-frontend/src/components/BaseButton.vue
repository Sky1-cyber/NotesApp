<script setup lang="ts">
import { computed } from "vue";
import { Loader2 } from "lucide-vue-next";

type Variant = "primary" | "secondary" | "danger" | "ghost";
type Size = "sm" | "md" | "lg" | "icon";

const props = withDefaults(
    defineProps<{
      variant?: Variant;
      size?: Size;
      loading?: boolean;
      disabled?: boolean;
      type?: "button" | "submit" | "reset";
      block?: boolean;
    }>(),
    {
      variant: "primary",
      size: "md",
      loading: false,
      disabled: false,
      type: "button",
      block: false,
    }
);

const base =
    "inline-flex items-center justify-center gap-2 font-medium rounded-lg " +
    "transition-colors duration-150 select-none " +
    "disabled:opacity-50 disabled:cursor-not-allowed disabled:pointer-events-none";

const variants: Record<Variant, string> = {
  primary:
      "bg-brand-600 text-white shadow-sm hover:bg-brand-700 active:bg-brand-800",
  secondary:
      "bg-white text-gray-700 border border-gray-300 hover:bg-gray-50 active:bg-gray-100",
  danger:
      "bg-red-600 text-white shadow-sm hover:bg-red-700 active:bg-red-800",
  ghost:
      "text-gray-600 hover:bg-gray-100 hover:text-gray-900",
};

const sizes: Record<Size, string> = {
  sm:   "h-8  px-3 text-sm",
  md:   "h-10 px-4 text-sm",
  lg:   "h-11 px-5 text-base",
  icon: "h-8 w-8 p-0",
};

const classes = computed(() => [
  base,
  variants[props.variant],
  sizes[props.size],
  props.block && "w-full",
]);
</script>

<template>
  <button
      :type="type"
      :disabled="disabled || loading"
      :class="classes"
  >
    <Loader2
        v-if="loading"
        class="w-4 h-4 animate-spin shrink-0"
        aria-hidden="true"
    />
    <slot />
  </button>
</template>