export interface User {
    id: number;
    username: string;
}

export interface AuthResponse {
    token: string;
    username: string;
    userId: number;
}

export interface Note {
    id: number;
    userId: number;
    title: string;
    content?: string | null;
    createdAt: string;
    updatedAt: string;
}

export interface NotesResponse {
    items: Note[];
    total: number;
    page: number;
    pageSize: number;
}

export type SortBy = "createdAt" | "updatedAt" | "title";
export type SortDir = "asc" | "desc";

export interface NotesQuery {
    search?: string;
    sortBy?: SortBy;
    sortDir?: SortDir;
    page?: number;
    pageSize?: number;
}