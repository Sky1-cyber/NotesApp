import { defineStore } from "pinia";
import { reactive, ref } from "vue";
import { notesApi } from "../api/notes.api";
import type { Note, NotesQuery } from "@/types";

export const useNotesStore = defineStore("notes", () => {
    const notes = ref<Note[]>([]);
    const total = ref(0);
    const loading = ref(false);
    const error = ref<string | null>(null);

    const query = reactive<Required<NotesQuery>>({
        search: "",
        sortBy: "updatedAt",
        sortDir: "desc",
        page: 1,
        pageSize: 9,
    });

    async function fetchNotes() {
        loading.value = true;
        error.value = null;
        try {
            const res = await notesApi.list(query);
            notes.value = res.items;
            total.value = res.total;
        } catch (e: any) {
            error.value = e?.response?.data?.message ?? "Failed to load notes";
        } finally {
            loading.value = false;
        }
    }

    async function createNote(title: string, content?: string) {
        const note = await notesApi.create(title, content);
        await fetchNotes();
        return note;
    }

    async function updateNote(id: number, title: string, content?: string) {
        const note = await notesApi.update(id, title, content);
        await fetchNotes();
        return note;
    }

    async function deleteNote(id: number) {
        await notesApi.remove(id);
        // optimistic update
        notes.value = notes.value.filter((n) => n.id !== id);
        total.value = Math.max(0, total.value - 1);
    }

    function resetQuery() {
        query.search = "";
        query.sortBy = "updatedAt";
        query.sortDir = "desc";
        query.page = 1;
        query.pageSize = 9;
    }

    return {
        notes, total, loading, error, query,
        fetchNotes, createNote, updateNote, deleteNote, resetQuery,
    };
});