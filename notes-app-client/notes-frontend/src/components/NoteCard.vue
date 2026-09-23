<script setup lang="ts">
import { Calendar, Clock, Pencil, Trash2 } from "lucide-vue-next";
import type { Note } from "../types";

defineProps<{ note: Note }>();
const emit = defineEmits<{
  (e: "edit", note: Note): void;
  (e: "delete", id: number): void;
}>();

const fmt = (d: string) =>
    new Date(d).toLocaleString(undefined, {
      dateStyle: "medium",
      timeStyle: "short",
    });
</script>

<template>
  <article
      class="group relative flex flex-col bg-white rounded-xl border border-gray-200 shadow-card
           hover:shadow-cardHover hover:border-gray-300 transition-all duration-200"
  >
    <!-- Actions -->
    <div
        class="absolute top-3 right-3 flex items-center gap-1 opacity-0 group-hover:opacity-100
             focus-within:opacity-100 transition-opacity duration-150"
    >
      <button
          type="button"
          title="Edit note"
          aria-label="Edit note"
          class="p-1.5 rounded-lg text-gray-500 hover:text-brand-600 hover:bg-brand-50 transition-colors"
          @click="emit('edit', note)"
      >
        <Pencil class="w-4 h-4" />
      </button>
      <button
          type="button"
          title="Delete note"
          aria-label="Delete note"
          class="p-1.5 rounded-lg text-gray-500 hover:text-red-600 hover:bg-red-50 transition-colors"
          @click="emit('delete', note.id)"
      >
        <Trash2 class="w-4 h-4" />
      </button>
    </div>

    <!-- Body -->
    <div class="p-5 flex-1 flex flex-col">
      <h3 class="font-semibold text-gray-900 text-[15px] leading-snug break-words pr-16">
        {{ note.title }}
      </h3>

      <p
          class="mt-2 text-sm text-gray-600 leading-relaxed whitespace-pre-wrap break-words line-clamp-4 flex-1"
      >
        {{ note.content || "No content" }}
      </p>
    </div>

    <!-- Footer metadata -->
    <footer class="px-5 py-3 border-t border-gray-100 flex items-center gap-4 text-xs text-gray-500">
      <span class="inline-flex items-center gap-1.5">
        <Calendar class="w-3.5 h-3.5" />
        {{ fmt(note.createdAt) }}
      </span>
      <span
          v-if="note.updatedAt !== note.createdAt"
          class="inline-flex items-center gap-1.5"
      >
        <Clock class="w-3.5 h-3.5" />
        {{ fmt(note.updatedAt) }}
      </span>
    </footer>
  </article>
</template>