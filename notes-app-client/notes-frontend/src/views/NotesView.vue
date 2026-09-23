<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { storeToRefs } from "pinia";
import {
  ArrowDownAZ,
  ArrowUpAZ,
  ChevronLeft,
  ChevronRight,
  FileText,
  Loader2,
  Plus,
  Search,
  SearchX,
  AlertCircle,
} from "lucide-vue-next";
import { useNotesStore } from "../stores/notes";
import type { Note } from "../types";
import AppHeader from "../layouts/AppHeader.vue";
import NoteCard from "../components/NoteCard.vue";
import NoteFormModal from "../components/NoteFormModal.vue";
import ConfirmModal from "../components/ConfirmModal.vue";
import BaseButton from "../components/BaseButton.vue";

const notesStore = useNotesStore();
const { notes, total, loading, error, query } = storeToRefs(notesStore);

const showForm = ref(false);
const editing = ref<Note | null>(null);

const showConfirm = ref(false);
const pendingDeleteId = ref<number | null>(null);
const deleting = ref(false);

let searchTimer: ReturnType<typeof setTimeout> | null = null;

watch(
    () => query.value.search,
    () => {
      if (searchTimer) clearTimeout(searchTimer);
      searchTimer = setTimeout(() => {
        query.value.page = 1;
        notesStore.fetchNotes();
      }, 300);
    }
);

watch(
    [() => query.value.sortBy, () => query.value.sortDir, () => query.value.page],
    () => notesStore.fetchNotes()
);

onMounted(() => notesStore.fetchNotes());

const totalPages = computed(() =>
    Math.max(1, Math.ceil(total.value / query.value.pageSize))
);

function openCreate() {
  editing.value = null;
  showForm.value = true;
}
function openEdit(note: Note) {
  editing.value = note;
  showForm.value = true;
}
function toggleSortDir() {
  query.value.sortDir = query.value.sortDir === "asc" ? "desc" : "asc";
}

async function handleSave(payload: { title: string; content?: string }) {
  try {
    if (editing.value) {
      await notesStore.updateNote(editing.value.id, payload.title, payload.content);
    } else {
      await notesStore.createNote(payload.title, payload.content);
    }
    showForm.value = false;
    editing.value = null;
  } catch {
    // optionally surface a toast
  }
}

function askDelete(id: number) {
  pendingDeleteId.value = id;
  showConfirm.value = true;
}

async function confirmDelete() {
  if (pendingDeleteId.value == null) return;
  deleting.value = true;
  try {
    await notesStore.deleteNote(pendingDeleteId.value);
    showConfirm.value = false;
    pendingDeleteId.value = null;
  } finally {
    deleting.value = false;
  }
}
</script>

<template>
  <div class="min-h-screen flex flex-col">
    <AppHeader />

    <main class="flex-1 max-w-6xl w-full mx-auto px-4 sm:px-6 py-6">
      <!-- Page title -->
      <div class="mb-6">
        <h1 class="text-2xl font-semibold tracking-tight text-gray-900">All notes</h1>
        <p class="text-sm text-gray-500 mt-1">
          {{ total }} {{ total === 1 ? "note" : "notes" }} in your workspace
        </p>
      </div>

      <!-- Toolbar -->
      <div class="flex flex-col lg:flex-row gap-3 mb-6">
        <!-- Search -->
        <div class="relative flex-1">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" />
          <input
              v-model="query.search"
              type="search"
              placeholder="Search notes by title or content…"
              class="w-full h-10 pl-9 pr-3 rounded-lg border border-gray-300 bg-white text-sm
                   placeholder:text-gray-400 transition-colors
                   hover:border-gray-400
                   focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500"
          />
        </div>

        <!-- Sort by -->
        <div class="relative">
          <select
              v-model="query.sortBy"
              class="h-10 pl-3 pr-8 rounded-lg border border-gray-300 bg-white text-sm text-gray-700
                   appearance-none hover:border-gray-400
                   focus:outline-none focus:ring-2 focus:ring-brand-500 focus:border-brand-500"
          >
            <option value="updatedAt">Last updated</option>
            <option value="createdAt">Date created</option>
            <option value="title">Title</option>
          </select>
          <ChevronRight class="absolute right-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 rotate-90 pointer-events-none" />
        </div>

        <!-- Sort direction -->
        <button
            type="button"
            class="h-10 px-3 rounded-lg border border-gray-300 bg-white text-sm text-gray-700
                 inline-flex items-center gap-2 hover:border-gray-400 transition-colors
                 focus:outline-none focus:ring-2 focus:ring-brand-500"
            :title="query.sortDir === 'asc' ? 'Ascending' : 'Descending'"
            @click="toggleSortDir"
        >
          <ArrowUpAZ v-if="query.sortDir === 'asc'" class="w-4 h-4 text-gray-500" />
          <ArrowDownAZ v-else class="w-4 h-4 text-gray-500" />
          <span class="hidden sm:inline">
            {{ query.sortDir === "asc" ? "Asc" : "Desc" }}
          </span>
        </button>

        <!-- New note -->
        <BaseButton @click="openCreate">
          <Plus class="w-4 h-4" />
          <span>New note</span>
        </BaseButton>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <div
            v-for="i in 6"
            :key="i"
            class="bg-white rounded-xl border border-gray-200 p-5 animate-pulse"
        >
          <div class="h-4 bg-gray-200 rounded w-3/4"></div>
          <div class="mt-3 h-3 bg-gray-100 rounded"></div>
          <div class="mt-2 h-3 bg-gray-100 rounded w-5/6"></div>
          <div class="mt-4 h-3 bg-gray-100 rounded w-1/3"></div>
        </div>
      </div>

      <!-- Error -->
      <div
          v-else-if="error"
          class="rounded-xl border border-red-100 bg-red-50 px-4 py-6 text-center"
      >
        <AlertCircle class="w-6 h-6 text-red-600 mx-auto" />
        <p class="mt-2 text-sm text-red-700">{{ error }}</p>
        <BaseButton variant="secondary" size="sm" class="mt-3" @click="notesStore.fetchNotes()">
          Try again
        </BaseButton>
      </div>

      <!-- Empty state -->
      <div
          v-else-if="notes.length === 0"
          class="rounded-2xl border border-dashed border-gray-300 bg-white py-16 px-6 text-center"
      >
        <div class="mx-auto w-12 h-12 rounded-full bg-gray-100 flex items-center justify-center">
          <SearchX v-if="query.search" class="w-6 h-6 text-gray-500" />
          <FileText v-else class="w-6 h-6 text-gray-500" />
        </div>
        <h3 class="mt-4 text-base font-semibold text-gray-900">
          {{ query.search ? "No matching notes" : "No notes yet" }}
        </h3>
        <p class="mt-1 text-sm text-gray-500">
          {{
            query.search
                ? "Try a different keyword or clear the search."
                : "Create your first note to get started."
          }}
        </p>
        <BaseButton v-if="!query.search" class="mt-5" @click="openCreate">
          <Plus class="w-4 h-4" />
          <span>New note</span>
        </BaseButton>
        <BaseButton
            v-else
            variant="secondary"
            class="mt-5"
            @click="query.search = ''"
        >
          Clear search
        </BaseButton>
      </div>

      <!-- Grid -->
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <NoteCard
            v-for="n in notes"
            :key="n.id"
            :note="n"
            @edit="openEdit"
            @delete="askDelete"
        />
      </div>

      <!-- Pagination -->
      <div
          v-if="!loading && total > 0"
          class="flex items-center justify-between gap-4 mt-8 pt-6 border-t border-gray-200"
      >
        <p class="text-sm text-gray-500">
          Page <span class="font-medium text-gray-700">{{ query.page }}</span>
          of <span class="font-medium text-gray-700">{{ totalPages }}</span>
        </p>
        <div class="flex items-center gap-2">
          <BaseButton
              variant="secondary"
              size="sm"
              :disabled="query.page <= 1"
              @click="query.page = query.page - 1"
          >
            <ChevronLeft class="w-4 h-4" />
            <span>Previous</span>
          </BaseButton>
          <BaseButton
              variant="secondary"
              size="sm"
              :disabled="query.page >= totalPages"
              @click="query.page = query.page + 1"
          >
            <span>Next</span>
            <ChevronRight class="w-4 h-4" />
          </BaseButton>
        </div>
      </div>
    </main>

    <NoteFormModal
        :open="showForm"
        :editing="editing"
        @close="showForm = false"
        @save="handleSave"
    />

    <ConfirmModal
        :open="showConfirm"
        title="Delete this note?"
        message="The note will be permanently removed. This can't be undone."
        :loading="deleting"
        @confirm="confirmDelete"
        @cancel="showConfirm = false"
    />
  </div>
</template>