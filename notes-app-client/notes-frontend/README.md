# Notes App — Frontend

Vue 3 + TypeScript + Tailwind + Pinia client for the Notes API. Professional, accessible, responsive.

---

## 📦 Tech Stack

| Concern | Choice |
|---|---|
| Framework | Vue 3 (Composition API, `<script setup>`) |
| Language | TypeScript 5 |
| Build tool | Vite 5 |
| State | Pinia |
| Routing | Vue Router 4 |
| HTTP | Axios |
| Styling | Tailwind CSS 3 |
| Icons | lucide-vue-next |
| Font | Inter |

---

## 📁 Project Structure

```
src/
├── api/
│   ├── http.ts             # Axios instance, JWT + 401 interceptors
│   ├── auth.api.ts         # register(), login()
│   └── notes.api.ts        # list(), get(), create(), update(), remove()
│
├── stores/
│   ├── auth.ts             # Session, token, login/logout/register
│   └── notes.ts            # Query state, notes list, CRUD actions
│
├── router/
│   └── index.ts            # Routes + auth guards
│
├── components/
│   ├── BaseButton.vue      # Variants: primary | secondary | danger | ghost
│   ├── BaseInput.vue       # Label, hint, error, accessibility
│   ├── BaseTextarea.vue
│   ├── BaseModal.vue       # Teleport, focus trap, Escape, scroll lock
│   ├── NoteCard.vue
│   ├── NoteFormModal.vue
│   └── ConfirmModal.vue
│
├── layouts/
│   └── AppHeader.vue       # Sticky top bar, user avatar, logout
│
├── views/
│   ├── LoginView.vue
│   ├── RegisterView.vue
│   └── NotesView.vue       # Main CRUD page
│
├── types/
│   └── index.ts            # Shared interfaces (Note, User, NotesQuery…)
│
├── App.vue
├── main.ts
└── style.css
```

---

## 🎨 Design System

### Colors (`tailwind.config.js`)

- `brand.500` → `#6366f1` (indigo) — primary actions
- `surface.*` — background, muted, border tokens
- Danger → Tailwind's `red-600`

### Typography

- **Inter** variable font, weights 400 / 500 / 600 / 700
- Headings: `tracking-tight`, `font-semibold`
- Body: `text-sm` / `text-base`

### Shadows

- `shadow-card` — resting state for cards
- `shadow-cardHover` — hover state
- `shadow-modal` — modal panels

### Spacing & radius

- Corner radius: `rounded-lg` (buttons/inputs), `rounded-xl` (cards), `rounded-2xl` (modals)
- Consistent 4/8/16/24 spacing scale

---

## ⚙️ Setup

### Prerequisites

- Node.js **20+**
- npm 10+ (or pnpm / yarn — commands below use npm)

### Install

```bash
cd frontend
npm install
```

### Environment

Create a `.env` file in the project root:

```
VITE_API_URL=http://localhost:5000/api
```

A `.env.example` is provided as a template — copy it to `.env` and adjust the API URL to match your backend port.

### Run

```bash
npm run dev
```

Open `http://localhost:5173`.

### Build

```bash
npm run build       # outputs to dist/
npm run preview     # serve the production build locally
```

### Type check

```bash
npm run type-check  # vue-tsc --noEmit
```

---

## 🔐 Authentication Flow

1. User submits the login/register form.
2. `auth.api.ts` calls the backend, receives `{ token, username, userId }`.
3. `auth` store persists them in `localStorage`.
4. The Axios request interceptor attaches `Authorization: Bearer <token>` to every request.
5. A router guard redirects unauthenticated users to `/login`.
6. If the backend returns **401**, the response interceptor clears storage and redirects to `/login`.

---

## 🧠 State Management (Pinia)

### `useAuthStore`

| State | Type | Purpose |
|---|---|---|
| `token` | `string \| null` | JWT from backend |
| `username` | `string \| null` | Display name |
| `userId` | `number \| null` | Owner id |
| `isAuthenticated` | `computed` | `Boolean(token)` |

| Actions | Description |
|---|---|
| `login(u, p)` | Authenticate + persist |
| `register(u, p)` | Create account + persist |
| `logout()` | Clear state + storage |

### `useNotesStore`

| State | Type | Purpose |
|---|---|---|
| `notes` | `Note[]` | Current page items |
| `total` | `number` | Total across all pages |
| `loading` | `boolean` | Request in flight |
| `error` | `string \| null` | Last error message |
| `query` | `Required<NotesQuery>` | Reactive filter state (search, sort, page) |

| Actions | Description |
|---|---|
| `fetchNotes()` | GET `/notes` with current query |
| `createNote(title, content)` | POST then refresh |
| `updateNote(id, title, content)` | PUT then refresh |
| `deleteNote(id)` | DELETE with optimistic removal |
| `resetQuery()` | Reset filters on logout |

---

## 🖥️ Pages

### `/login`

- Brand mark + heading
- Username + password inputs with autocomplete hints
- Inline error banner on failure
- Link to `/register`

### `/register`

- Same layout as login
- Client-side length validation with hints
- Link back to `/login`

### `/` (Notes)

- Sticky header with user avatar + logout
- Toolbar: debounced search, sort field, sort direction toggle, **New note**
- Responsive grid of note cards (`1 → 2 → 3` columns)
- Skeleton loaders while fetching
- Empty-state distinction: *no notes yet* vs *no search results*
- Pagination footer
- Modals: create/edit form + delete confirmation

---

## 🎯 Behavior Details

### Search

Debounced **300 ms** before hitting the API. Changing the query resets to page 1.

### Sorting

Dropdown for field (`updatedAt` / `createdAt` / `title`) plus a toggle button for direction. Changing either triggers a refetch.

### Pagination

Server-side. Default `pageSize = 9` (matches the 3-column grid). Current page and total count displayed; prev/next disabled at bounds.

### Delete

Optimistic — the note is removed from the list immediately and the total is decremented. If the API call fails, the note reappears on the next fetch.

### Auth expiry

Any `401` response clears local storage and redirects to `/login`.

---

## ♿ Accessibility

- All interactive elements have focus rings (`focus-visible`)
- Modals trap Escape to close, lock body scroll, and support backdrop click
- Buttons use semantic `<button>` with `aria-label` where needed
- Inputs are labelled via `<label for>`, and errors set `aria-invalid`
- Icons that convey meaning have an accompanying text label
- `prefers-reduced-motion` disables animations

---

## 📱 Responsive Breakpoints

| Breakpoint | Layout |
|---|---|
| `< sm` (640px) | 1-column grid, modals slide from bottom (`rounded-t-2xl`), username hidden in header |
| `sm – lg` | 2-column grid |
| `≥ lg` (1024px) | 3-column grid, full toolbar inline |

---

## 🧪 Testing *(planned)*

```bash
npm run test          # Vitest + @vue/test-utils
```

Suggested coverage:
- `useNotesStore` — fetch / create / update / delete happy + error paths
- `BaseButton` — variant class mapping, loading state
- `NoteFormModal` — validation, save emit
- `NotesView` — search debounce, sort change triggers fetch

---

## 🎨 Icon Reference

All icons come from `lucide-vue-next`:

| Purpose | Icon |
|---|---|
| Brand / logo | `NotebookPen` |
| Create | `Plus` |
| Edit | `Pencil` |
| Delete | `Trash2` |
| Search | `Search` |
| Empty (no results) | `SearchX` |
| Empty (no notes) | `FileText` |
| Error | `AlertCircle` |
| Delete warning | `AlertTriangle` |
| Sort A→Z / Z→A | `ArrowUpAZ` / `ArrowDownAZ` |
| Prev / Next | `ChevronLeft` / `ChevronRight` |
| Loading | `Loader2` (with `animate-spin`) |
| Logout | `LogOut` |
| Calendar | `Calendar` |
| Clock | `Clock` |
| Close | `X` |

---

## 🐳 Production Build (Nginx)

**Dockerfile**
```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

**nginx.conf** (serves SPA + proxies API)
```nginx
server {
    listen 80;
    root /usr/share/nginx/html;
    index index.html;

    location /api/ {
        proxy_pass http://api:8080/api/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

---

## 🧭 Roadmap

- [ ] Dark mode toggle
- [ ] Toast notifications
- [ ] Keyboard shortcuts (`Ctrl+N` new, `Ctrl+K` search)
- [ ] Offline cache for notes
- [ ] Rich-text note editor
- [ ] Tags and folders
- [ ] i18n (English / Khmer)

---