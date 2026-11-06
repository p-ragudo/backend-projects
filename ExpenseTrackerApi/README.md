# Expense Tracker API

A REST API built with ASP.NET Core and .NET 10 for account registration, JWT-based authentication, and expense CRUD. It uses PostgreSQL through Entity Framework Core and stores access and refresh tokens in secure HTTP-only cookies.

This is **Project 4** of the [20 Backend Project Ideas Roadmap](https://roadmap.sh/backend/project-ideas).

---

## Tech Stack

- **Framework**: ASP.NET Core (.NET 10)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core with Npgsql
- **Authentication**: JWT bearer validation using access tokens from the `accessToken` cookie
- **Password hashing**: Argon2
- **API reference**: OpenAPI and Scalar
- **Architecture**: Controller-service pattern

---

## Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Body / Cookies | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/auth/register` | `email`, `username`, `password` | Creates an account and sets access and refresh token cookies. |
| `POST` | `/api/auth/login` | `email`, `password` | Validates credentials and sets new token cookies. |
| `POST` | `/api/auth/refresh-token` | `refreshToken` cookie | Rotates the refresh token and sets new token cookies. |
| `POST` | `/api/auth/logout` | None | Revokes the refresh token and clears the token cookies. Requires authentication. |

Token cookies are `HttpOnly`, `Secure`, and use `SameSite=Lax`. The configured defaults are 1 minute for access tokens and 2 minutes for refresh tokens.

### Expenses (`/api/expenses`)

All expense endpoints require a valid access token in the `accessToken` cookie.

| Method | Endpoint | Body / Query | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/expenses` | `name`, `amount`, `categoryName` | Creates an expense, creating the category if it does not already exist. |
| `GET` | `/api/expenses` | None | Returns all expenses. |
| `PUT` | `/api/expenses` | `guid`, optional `name`, `amount`, `category` | Updates an expense. |
| `DELETE` | `/api/expenses/{id}?id={id}` | Required `id` query parameter | Deletes the expense with the given ID. The ID is currently read from the query string as well as included in the route. |

Categories are created from request values and are not restricted to a predefined list. The list endpoint does not currently support date filtering. Expenses are not associated with individual users, so any authenticated user can access the same expense collection.

---

## Request Examples

### Register

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "jane@example.com",
  "username": "jane",
  "password": "replace-with-a-strong-password"
}
```

On success, the API responds with `200 OK` and sets the `accessToken` and `refreshToken` cookies.

### Refresh Tokens

Send the refresh token cookie without a JSON body. On success, the API rotates the refresh token and returns updated `accessToken` and `refreshToken` cookies.

```http
POST /api/auth/refresh-token
Cookie: refreshToken=YOUR_REFRESH_TOKEN
```

### Create an Expense

```http
POST /api/expenses
Content-Type: application/json
Cookie: accessToken=YOUR_ACCESS_TOKEN

{
  "name": "Groceries",
  "amount": 42.50,
  "categoryName": "Food"
}
```

### List Expenses

```http
GET /api/expenses
Cookie: accessToken=YOUR_ACCESS_TOKEN
```

The response contains an `expenses` array with each expense's ID, name, amount, category, and creation time.

---

## Getting Started

### 1. Configure PostgreSQL and JWT settings

The application requires a PostgreSQL connection string named `DefaultConnection`. Configure it and replace the development JWT secret with a strong secret using environment variables:

```bash
export ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=expense_tracker;Username=postgres;Password=YOUR_PASSWORD'
export ExpenseTrackerApi__Jwt__SecretKey='replace-with-a-long-random-secret'
export ExpenseTrackerApi__Jwt__Issuer='expense-tracker-api'
export ExpenseTrackerApi__Jwt__Audience='expense-tracker-api'
```

Token lifetimes can also be configured with `ExpenseTrackerApi__Jwt__AccessTokenExpirationMinutes` and `ExpenseTrackerApi__Jwt__RefreshTokenExpirationMinutes`. Do not use the example secret from `appsettings.json` outside local development.

### 2. Apply database migrations

From the `ExpenseTrackerApi` directory, apply the included EF Core migrations:

```bash
dotnet ef database update
```

The application does not apply migrations automatically at startup.

### 3. Run the API

```bash
dotnet restore
dotnet run --launch-profile https
```

The HTTPS launch profile uses `https://localhost:7183`. In Development, the Scalar API reference is available at `https://localhost:7183/scalar/v1`, and the OpenAPI document is at `https://localhost:7183/openapi/v1.json`.

### 4. Run tests

```bash
dotnet test Tests/ExpenseTrackerApi.Tests.csproj
```

---
