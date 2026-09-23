<script setup lang="ts">
import { ref, watch } from "vue";
import { AlertCircle } from "lucide-vue-next";
import type { Note } from "../types";
import BaseModal from "./BaseModal.vue";
import BaseButton from "./BaseButton.vue";
import BaseInput from "./BaseInput.vue";
import BaseTextarea from "./BaseTextarea.vue";

const props = defineProps<{ open: boolean; editing?: Note | null }>();
const emit = defineEmits<{
  (e: "close"): void;
  (e: "save", payload: { title: string; content?: string }): void;
}>();

const title = ref("");
const content = ref("");
const error = ref("");
const saving = ref(false);

watch(
    () => props.open,
    (open) => {
      if (open) {
        title.value = props.editing?.title ?? "";
        content.value = props.editing?.content ?? "";
        error.value = "";
        saving.value = false;
      }
    }
);

function save() {
  error.value = "";
  if (!title.value.trim()) {
    error.value = "Title is required.";
    return;
  }
  saving.value = true;
  try {
    emit("save", { title: title.value.trim(), content: content.value });
  } finally {
    saving.value = false;
  }
}
</script>

<template>
  <BaseModal
      :open="open"
      :title="editing ? 'Edit note' : 'New note'"
      :description="editing ? 'Make your changes and save.' : 'Give it a title and start writing.'"
      size="lg"
      @close="emit('close')"
  >
    <div class="space-y-5">
      <BaseInput v-model="title" label="Title" placeholder="e.g., Meeting notes" required />
      <BaseTextarea v-model="content" label="Content" placeholder="Write something…" :rows="7" />

      <div
          v-if="error"
          class="flex items-start gap-2 rounded-lg bg-red-50 border border-red-100 px-3 py-2.5 text-sm text-red-700"
      >
        <AlertCircle class="w-4 h-4 mt-0.5 shrink-0" />
        <span>{{ error }}</span>
      </div>
    </div>

    <template #footer>
      <div class="flex items-center justify-end gap-2">
        <BaseButton variant="secondary" @click="emit('close')">Cancel</BaseButton>
        <BaseButton :loading="saving" @click="save">
          {{ editing ? "Save changes" : "Create note" }}
        </BaseButton>
      </div>
    </template>
  </BaseModal>
</template>