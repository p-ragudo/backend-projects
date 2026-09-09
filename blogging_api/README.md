# Personal Blogging Platform API

A RESTful backend API built with ASP.NET Core and Entity Framework Core. Provides full CRUD functionality for managing blog posts, supports many-to-many tag relationships, and includes filtering across titles, contents, and tags.

This is **Project 1** of the [20 Backend Project Ideas Roadmap](https://roadmap.sh/backend/project-ideas)

---

## Tech Stack

- **Framework**: ASP.NET Core (.NET 10+)
- **ORM**: Entity Framework Core
- **Database**: Azure SQL Server 
- **Architecture**: Controller-Service pattern using typed DTO records

---
## Endpoints

| Method | Endpoint | Query / Body | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/blogs` | `?term=...&tag=...` | List filtered posts |
| `GET` | `/api/blogs/{id}` | — | Get post by ID |
| `POST` | `/api/blogs` | `{ title, content, tags: [] }` | Create a post |
| `PUT` | `/api/blogs/{id}` | `{ title, content, tags: [] }` | Update a post |
| `DELETE`| `/api/blogs/{id}` | — | Delete a post |

---

## Quick Example

```json
// POST /api/blogs
{
  "title": "Clean Architecture in .NET",
  "content": "Exploring repository patterns and EF Core optimizations.",
  "tags": ["dotnet", "csharp", "backend"]
}
```
---

## Getting Started

```bash
git clone https://github.com/p-ragudo/backend-projects.git
cd backend-projects/blogging_api

# Update ConnectionStrings:DefaultConnection in appsettings.json
dotnet ef database update
dotnet run
```

Access the OpenAPI document at `http://localhost:8080/openapi/v1.json`.