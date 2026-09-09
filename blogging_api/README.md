# Personal Blogging Platform API

A RESTful backend API built with ASP.NET Core and Entity Framework Core. Provides full CRUD functionality for managing blog posts, supports many-to-many tag relationships, and includes filtering across titles, contents, and tags.

This is **Project 1** of the [20 Backend Project Ideas Roadmap](https://roadmap.sh/backend/project-ideas)

---

## Features

- **Full CRUD Operations**: Create, read, update, and delete blog posts.
- **Tag Management**: Many-to-many associations between posts and tags.
- **Dynamic Search & Filtering**:
  - Filter posts by multiple keywords in title or content.
  - Filter posts matching one or more tags.
- **Query Optimizations**: Read-only queries using `AsNoTracking()` and database-level projections via `.Select()`.

---

## Tech Stack

- **Framework**: ASP.NET Core (.NET 10+)
- **ORM**: Entity Framework Core
- **Database**: Azure SQL Server 
- **Architecture**: Controller-Service pattern using typed DTO records

---

## API Endpoints

### Blog Posts

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/blogs` | Retrieve all blog posts (supports search, tags, pagination) |
| `GET` | `/api/blogs/{id}` | Retrieve a single blog post by its unique ID |
| `POST` | `/api/blogs` | Create and publish a new blog post |
| `PUT` | `/api/blogs/{id}` | Update an existing blog post by ID |
| `DELETE` | `/api/blogs/{id}` | Delete a blog post by ID |

---

## Query Parameters (`GET /api/blogs`)

| Parameter | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `term` | `string` (array) | Search keywords in title or content | `?term=programming&term=solo` |
| `tag` | `string` (array) | Filter posts containing specific tags | `?tag=csharp&tag=backend` |
| `page` | `int` | Current page number (default: `1`) | `?page=2` |
| `pageSize` | `int` | Number of posts per page (default: `10`) | `?pageSize=5` |

---

## Request & Response Schemas

### Create / Update Request (`POST`, `PUT`)

```json
{
  "title": "Getting Started with ASP.NET Core",
  "content": "Step-by-step guide to setting up a REST API using EF Core.",
  "tags": [
    "csharp",
    "dotnet",
    "backend"
  ]
}
```

### Blog Response (`GET`)

```json
{
  "id": 1,
  "title": "Getting Started with ASP.NET Core",
  "content": "Step-by-step guide to setting up a REST API using EF Core.",
  "tags": [
    "csharp",
    "dotnet",
    "backend"
  ],
  "createdAt": "2026-09-09T07:25:01.925Z",
  "editedAt": null
}
```

---

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (8.0 or later)
- Configured database instance

### Installation & Run

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/your-username/blogging-api.git](https://github.com/your-username/blogging-api.git)
   cd blogging-api
   ```

2. **Configure connection string:**
   Update your database credentials in `appsettings.json` or `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=BlogDb;User Id=postgres;Password=yourpassword;"
     }
   }
   ```

3. **Apply EF Core migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Launch the application:**
   ```bash
   dotnet run
   ```

The API will start locally at `https://localhost:5001` or `http://localhost:5000` with Swagger documentation available at `/openapi/v1.json`.