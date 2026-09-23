<script setup lang="ts">
import { computed, useId } from "vue";

const props = defineProps<{
  modelValue: string;
  label?: string;
  type?: string;
  placeholder?: string;
  error?: string;
  hint?: string;
  required?: boolean;
  minlength?: number;
  maxlength?: number;
  autocomplete?: string;
  autofocus?: boolean;
}>();

defineEmits<{ (e: "update:modelValue", value: string): void }>();

const id = useId();
const hasError = computed(() => Boolean(props.error));
</script>

<template>
  <div class="space-y-1.5">
    <label
        v-if="label"
        :for="id"
        class="block text-sm font-medium text-gray-700"
    >
      {{ label }}
      <span v-if="required" class="text-red-500" aria-hidden="true">*</span>
    </label>

    <input
        :id="id"
        :type="type ?? 'text'"
        :value="modelValue"
        :placeholder="placeholder"
        :required="required"
        :minlength="minlength"
        :maxlength="maxlength"
        :autocomplete="autocomplete"
        :autofocus="autofocus"
        :aria-invalid="hasError || undefined"
        :class="[
        'w-full h-10 rounded-lg border bg-white px-3 text-sm text-gray-900',
        'placeholder:text-gray-400',
        'transition-colors duration-150',
        'focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500',
        hasError
          ? 'border-red-400 focus:ring-red-500 focus:border-red-500'
          : 'border-gray-300 hover:border-gray-400',
      ]"
        @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    />

    <p v-if="error" class="text-xs text-red-600">{{ error }}</p>
    <p v-else-if="hint" class="text-xs text-gray-500">{{ hint }}</p>
  </div>
</template>