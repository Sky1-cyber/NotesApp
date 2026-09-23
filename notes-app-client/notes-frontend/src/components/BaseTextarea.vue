<script setup lang="ts">
import { useId } from "vue";

defineProps<{
  modelValue: string;
  label?: string;
  placeholder?: string;
  rows?: number;
  maxlength?: number;
}>();

defineEmits<{ (e: "update:modelValue", value: string): void }>();

const id = useId();
</script>

<template>
  <div class="space-y-1.5">
    <label v-if="label" :for="id" class="block text-sm font-medium text-gray-700">
      {{ label }}
    </label>
    <textarea
        :id="id"
        :value="modelValue"
        :placeholder="placeholder"
        :rows="rows ?? 6"
        :maxlength="maxlength"
        class="w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm text-gray-900 placeholder:text-gray-400 transition-colors duration-150 hover:border-gray-400 focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500 resize-y min-h-[120px]"
        @input="$emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
    />
  </div>
</template>