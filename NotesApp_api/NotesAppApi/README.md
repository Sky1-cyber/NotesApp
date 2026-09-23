# Notes API — Backend

ASP.NET Core 8 Web API powering the Notes App. Uses **Dapper** for data access against **SQL Server**, **JWT** for auth, and **BCrypt** for password hashing.

---

## 📦 Tech Stack

| Concern | Choice |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Language | C# 12 |
| ORM | Dapper (micro-ORM) |
| Database | SQL Server 2019+ |
| Auth | JWT Bearer (HS256) |
| Password hashing | BCrypt.Net-Next |
| API docs | Swashbuckle (Swagger UI) |
| Logging | Microsoft.Extensions.Logging |

---

## 📁 Project Structure

```
backend/
├── Controllers/
│   ├── AuthController.cs        # /api/auth/register, /api/auth/login
│   └── NotesController.cs       # /api/notes — CRUD, search, sort, paginate
│
├── Services/
│   ├── AuthService.cs           # Registration, login, JWT generation
│   └── NoteService.cs           # Business rules, ownership checks
│
├── Repositories/
│   ├── UserRepository.cs        # Dapper queries for users
│   └── NoteRepository.cs        # Dapper queries for notes
│
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   └── Note.cs
│   └── Dtos/
│       ├── AuthDtos.cs          # RegisterDto, LoginDto, AuthResponseDto
│       └── NoteDtos.cs          # CreateNoteDto, UpdateNoteDto, NoteQueryDto
│
├── Data/
│   └── DbConnectionFactory.cs   # Creates SqlConnection from config
│
├── Middleware/
│   └── ExceptionMiddleware.cs   # Global unhandled-exception handler
│
├── Extensions/
│   └── ClaimsExtensions.cs      # GetUserId() from ClaimsPrincipal
│
├── Program.cs                   # DI, JWT, CORS, Swagger wiring
└── appsettings.json
```

---

## 🧩 Architecture

```
HTTP request
    ↓
Controller              (validates DTO shape, reads userId from JWT)
    ↓
Service                 (business rules, ownership enforcement)
    ↓
Repository              (parameterized SQL via Dapper)
    ↓
SQL Server
```

**Design rules**

- Controllers **never** touch the DB directly.
- Services orchestrate multiple repositories and enforce invariants.
- Repositories only execute SQL — no business logic.
- DTOs are never reused as entities.

---

## 🗄️ Database Schema

```sql
CREATE DATABASE notesdb;
GO

USE notesdb;
GO

CREATE TABLE users (
    id            INT IDENTITY(1,1) PRIMARY KEY,
    username      NVARCHAR(50)  NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    created_at    DATETIME2     NOT NULL DEFAULT SYSDATETIME()
);
GO

CREATE TABLE notes (
    id         INT IDENTITY(1,1) PRIMARY KEY,
    user_id    INT            NOT NULL,
    title      NVARCHAR(255)  NOT NULL,
    content    NVARCHAR(MAX)  NULL,
    created_at DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    updated_at DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT fk_notes_users FOREIGN KEY (user_id)
        REFERENCES users(id) ON DELETE CASCADE
);
GO

CREATE INDEX idx_notes_user_id ON notes(user_id);
GO
```

Save as `scripts/schema.sql` and apply with SSMS / Azure Data Studio / `sqlcmd`.

---

## ⚙️ Configuration

`appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=notesdb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  },
  "Jwt": {
    "Key": "CHANGE_ME_AT_LEAST_32_CHARACTERS_LONG_SECRET",
    "Issuer": "NotesApi",
    "Audience": "NotesApp",
    "ExpiresInMinutes": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> ⚠️ **Never commit production secrets.** Use [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) locally and environment variables in production.

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-real-secret"
dotnet user-secrets set "ConnectionStrings:Default" "your-real-conn-string"
```

---

## 🚀 Running

```bash
dotnet restore
dotnet run
```

- API base: `http://localhost:5000` (see console output for exact port)
- Swagger UI: `http://localhost:5000/swagger`

### Using Swagger with auth

1. `POST /api/auth/register` → copy the returned `token`
2. Click **Authorize** → enter `Bearer <token>`
3. All locked endpoints are now accessible

---

## 📚 API Reference

### Auth

#### `POST /api/auth/register`

```json
{ "username": "alice", "password": "secret123" }
```

**200 OK**
```json
{ "token": "eyJ...", "username": "alice", "userId": 1 }
```

**409 Conflict** — username taken
**400 Bad Request** — validation failure

---

#### `POST /api/auth/login`

```json
{ "username": "alice", "password": "secret123" }
```

**200 OK** — same response as register
**401 Unauthorized** — invalid credentials

---

### Notes *(all require `Authorization: Bearer <token>`)*

#### `GET /api/notes`

Query parameters:

| Param | Type | Default | Values |
|---|---|---|---|
| `search` | string | — | Free text |
| `sortBy` | string | `updatedAt` | `createdAt` \| `updatedAt` \| `title` |
| `sortDir` | string | `desc` | `asc` \| `desc` |
| `page` | int | 1 | ≥ 1 |
| `pageSize` | int | 10 | 1–100 |

**200 OK**
```json
{
  "items": [
    {
      "id": 12,
      "userId": 1,
      "title": "Meeting notes",
      "content": "Discussed Q4 roadmap…",
      "createdAt": "2025-01-10T09:15:00Z",
      "updatedAt": "2025-01-11T14:22:00Z"
    }
  ],
  "total": 42,
  "page": 1,
  "pageSize": 10
}
```

---

#### `GET /api/notes/{id}`

**200 OK** — single `Note` object
**404 Not Found** — not owned by caller or doesn't exist

---

#### `POST /api/notes`

```json
{ "title": "New idea", "content": "Optional body" }
```

**201 Created** with `Location` header → the new note

---

#### `PUT /api/notes/{id}`

```json
{ "title": "Updated title", "content": "Updated body" }
```

**200 OK** — updated note
**404 Not Found** — not owned by caller

---

#### `DELETE /api/notes/{id}`

**204 No Content**
**404 Not Found** — not owned by caller

---

## 🔒 Security

| Concern | Mitigation |
|---|---|
| Password storage | BCrypt (work factor 11) |
| Token signing | HS256 with config-supplied key |
| Token validation | Issuer, audience, lifetime, signing key all checked |
| SQL injection | 100% parameterized Dapper queries |
| Sort injection | Column whitelist switch in `NoteRepository.GetPagedAsync` |
| Cross-user access | Every note query includes `AND user_id = @UserId` |
| CORS | Only the frontend origin is whitelisted |
| Secrets | `user-secrets` locally, env vars in prod |

---

## 🧪 Testing *(planned)*

```bash
dotnet test
```

Recommended coverage:
- `NoteService` unit tests (in-memory fake repository)
- `AuthService` unit tests (password rules, duplicate users)
- Integration tests with `WebApplicationFactory<Program>` + Testcontainers SQL Server

---

## 🐳 Docker

**Dockerfile**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "NotesApi.dll"]
```

**docker run**
```bash
docker build -t notes-api .
docker run -p 5000:8080 \
  -e "ConnectionStrings__Default=Server=host.docker.internal;Database=notesdb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True" \
  -e "Jwt__Key=CHANGE_ME_AT_LEAST_32_CHARS_LONG_SECRET" \
  notes-api
```

---

## 📝 Notes on Dapper Choices

- **No AutoMapper** — mapping happens inline in the repository with `AS` aliases.
- **No EF Core** — Dapper keeps the SQL explicit and the runtime lean.
- **`OUTPUT INSERTED.*`** returns the created/updated row in one round-trip.
- **`QuerySingleOrDefaultAsync`** used everywhere an entity may be `null`.
- **`ExecuteScalarAsync<int>`** for count queries.

---

## 🧭 Roadmap

- [ ] Refresh tokens
- [ ] FluentValidation for DTOs
- [ ] Serilog structured logging
- [ ] Health checks (`/health`)
- [ ] Rate limiting on auth endpoints
- [ ] Soft delete (`deleted_at` column)
- [ ] Notes sharing between users

---