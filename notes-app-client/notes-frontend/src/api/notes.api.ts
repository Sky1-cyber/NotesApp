import http from "./http";
import type { Note, NotesQuery, NotesResponse } from "@/types";

export const notesApi = {
    list: (query: NotesQuery) =>
        http.get<NotesResponse>("/notes", { params: query }).then((r) => r.data),

    get: (id: number) =>
        http.get<Note>(`/notes/${id}`).then((r) => r.data),

    create: (title: string, content?: string) =>
        http.post<Note>("/notes", { title, content }).then((r) => r.data),

    update: (id: number, title: string, content?: string) =>
        http.put<Note>(`/notes/${id}`, { title, content }).then((r) => r.data),

    remove: (id: number) =>
        http.delete(`/notes/${id}`).then((r) => r.data),
};