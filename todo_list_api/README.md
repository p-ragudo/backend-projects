# To-Do List API

A secure RESTful backend API built with ASP.NET Core, ASP.NET Core Identity, and Entity Framework Core. Features token-based authentication, user account management, and full CRUD operations for tasks with status and keyword filtering.

This is **Project 2** of the [20 Backend Project Ideas Roadmap](https://roadmap.sh/backend/project-ideas).

---

## Tech Stack

- **Framework**: ASP.NET Core
- **Authentication**: ASP.NET Core Identity (`MapIdentityApi`)
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Documentation**: OpenAPI / Scalar API Reference
- **Architecture**: Controller-Service pattern using strongly typed DTOs

---

## Endpoints

### Authentication (`/auth`)

| Method | Endpoint | Body | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/auth/register` | `{ email, password }` | Register a new user |
| `POST` | `/auth/login` | `{ email, password }` | Log in and obtain a Bearer token |
| `POST` | `/auth/refresh` | `{ refreshToken }` | Refresh an expired access token |

### Tasks (`/api/v1/todo`) — *Requires Authorization: Bearer &lt;token&gt;*

| Method | Endpoint | Query / Body | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/todo` | `?title=...&isCompleted=...` | List and filter tasks |
| `GET` | `/api/v1/todo/{id}` | — | Get task details by ID |
| `POST` | `/api/v1/todo` | `{ title }` | Create a new task |
| `PUT` | `/api/v1/todo/{id}` | `{ title, isCompleted }` | Update task details or status |
| `DELETE`| `/api/v1/todo/{id}` | — | Delete a task |

---

## Quick Example

### 1. Authenticate
```http
POST /auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123!"
}
```

### 2. Create a Task
```http
POST /api/v1/todo
Authorization: Bearer <your_access_token>
Content-Type: application/json

{
  "title": "Finish Project 2 documentation"
}
```

### 3. Fetch a Task with Filters
```http
GET /api/v1/todo?title=keyword&title=keyword&isCompleted=true
Authorization: Bearer <your_access_token>
Content-Type: application/json
```

## Getting Started
```bash
git clone https://github.com/p-ragudo/backend-projects.git
cd backend-projects/todo_list_api

# Update ConnectionStrings:DefaultConnection in appsettings.json
dotnet ef database update
dotnet run
```

Access the OpenAPI document at `http://localhost:5046/scalar/v1` or `http://localhost:{PORT}/scalar/v1`.