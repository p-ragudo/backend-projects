# Basic Auth & Student Management API

A custom authentication and session management exercise built from scratch using ASP.NET Core, PostgreSQL, and Entity Framework Core. Features email/password authentication, database-persisted session cookies, and a custom pipeline middleware for route-level access control alongside full CRUD operations for students.

> **Note:** This is a standalone study project outside of the roadmap track, built specifically to explore and understand the lower-level mechanics of cookie-based sessions, pipeline short-circuiting, and custom authentication middleware before moving on to higher-level abstractions.

---

## Tech Stack

- **Framework**: ASP.NET Core
- **Authentication**: Custom Session Cookies & Pipeline Middleware
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL (Npgsql)
- **Documentation**: OpenAPI / Scalar API Reference
- **Architecture**: Controller-Service pattern with strongly typed DTOs/records

---

## How It Works

1. **Authentication Flow**: When a user registers or logs in via `/auth/login`, the service verifies credentials, generates a persistent `session_token`, stores it in PostgreSQL, and attaches it as an `HttpOnly` cookie in the response.
2. **Custom Middleware (`BasicAuthMiddleware`)**: Intercepts all incoming requests. Public routes (`/`, `/auth/*`, `/scalar`, `/openapi`) are allowed downstream, while protected endpoints inspect the cookie jar. If `session_token` is missing or invalid in the database, the pipeline immediately short-circuits with a `401 Unauthorized` JSON payload.
3. **Protected Resources**: Standard CRUD endpoints under `/api/Students` serve as the authorization testbed to verify authenticated sessions and dynamic query filtering.

---

## Endpoints

### System & Documentation

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/` | Health check endpoint |
| `GET` | `/scalar/v1` | Interactive Scalar API Reference |
| `GET` | `/openapi/v1.json` | OpenAPI specification |

### Authentication (`/auth`) — *Public*

| Method | Endpoint | Body | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/auth/register` | `{ email, password }` | Register a new user |
| `POST` | `/auth/login` | `{ email, password }` | Authenticate and issue `session_token` cookie |
| `POST` | `/auth/logout` | — | Invalidate session and revoke cookie |

### Students (`/api/Students`) — *Requires Session Cookie*

| Method | Endpoint | Query / Body | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Students` | `?ids=...&firstNames=...` | List students with dynamic multi-field filtering |
| `GET` | `/api/Students/{id}` | — | Get single student details by ID |
| `POST` | `/api/Students` | `{ firstName, lastName }` | Create a new student record |
| `PUT` | `/api/Students/{id}` | `{ firstName, lastName, isEnrolled }` | Update an existing student |
| `DELETE` | `/api/Students/{id}` | — | Delete a student record |

---

## Quick Example

### 1. Register & Login
```http
POST /auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}

Response sets an HttpOnly cookie: session_token=<token>

### 2. Fetch Students (Authenticated via Cookie)
```
GET /api/Students?firstNames=John&isEnrolled=true
Cookie: session_token=<your_session_token>
```

### 3. Create a Student
```
POST /api/Students
Cookie: session_token=<your_session_token>
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe"
}
```

## Getting Started
```
git clone [https://github.com/p-ragudo/backend-projects.git](https://github.com/p-ragudo/backend-projects.git)
cd backend-projects/basic_auth

# Update ConnectionStrings:DefaultConnection in appsettings.json for PostgreSQL
dotnet ef database update
dotnet run
```

Access the Scalar documentation UI at http://localhost:5132/scalar/v1 (or your assigned codespace/dev container port).