# onlineCurses - Backend (.NET 9)

This is the backend project for the **online courses platform** created for the technical assessment.  
It implements a REST API using **Clean Architecture**, **manual JWT authentication** (without ASP.NET Identity), **MySQL** as the database (Pomelo provider), **soft delete**, mandatory business rules, and **unit tests**.

---

## Technologies Used

- .NET 9.0
- Entity Framework Core 9.0 with `Pomelo.EntityFrameworkCore.MySql 9.0.0`
- BCrypt.Net-Next for password hashing
- JWT for authentication
- Swagger for API documentation

---

## Project Structure (Clean Architecture)

```text
onlineCurses/
├── Domain/                 → Domain entities, enums, and interfaces
├── Application/            → Application services, DTOs, and business logic
├── Infrastructure/        → Repositories, DbContext, migrations, and seed data
└── API/                    → Controllers, configuration, and Program.cs
```
# Prerequisites

.NET 9.0 SDK installed (version 9.0.307 or higher)

MySQL Server running (local or Docker)

Node.js (optional, only for a separate frontend)

# Database Configuration

Create the database in MySQL:

Verify or update the connection string in /API/appsettings.json:

```text
"ConnectionStrings": {
"DefaultConnection": "Server=localhost;Database=onlinecursesdb;User=root;Password=;SslMode=none;"
}
```
Adjust the user, password, and port if necessary.

# Migrations and Seed Data
From the project root folder (onlineCurses-backend):

```text
# Add a new migration (first time or when entities change)
dotnet ef migrations add InitialCreate --project /Infrastructure --startup-project /API

# Apply migrations to the database
dotnet ef database update --project /Infrastructure --startup-project /API
```

When applying the migration for the first time, all tables will be created and a test user will be inserted:

Email: admin@example.com

Password: Password123!

Running the API
```text
cd src/API
dotnet run
```

The API will be available at:

HTTPS: https://localhost:7291  (or the port shown in the console)

Swagger UI: https://localhost:7291/swagger

# Main Endpoints

## Authentication (no authorization required)

POST /api/auth/register → Register a new user

POST /api/auth/login → Get JWT token (valid for 4 hours)

### Courses and Lessons

(Require JWT in header: Authorization: Bearer <token>)

GET /api/courses/search?q=&status=&page=&pageSize= → List courses with filters and pagination

GET /api/courses/{id}/summary → Course summary (total lessons, last modification)

POST /api/courses → Create course

PUT /api/courses/{id} → Update course

DELETE /api/courses/{id} → Soft delete course

PATCH /api/courses/{id}/publish → Publish (only if at least 1 active lesson exists)

PATCH /api/courses/{id}/unpublish → Unpublish

GET /api/courses/{courseId}/lessons → List lessons (ordered)

POST /api/courses/{courseId}/lessons → Create lesson (unique order per course)

PUT /api/courses/lessons/{id} → Update lesson

DELETE /api/courses/lessons/{id} → Soft delete lesson

### Implemented Business Rules

A course can only be published if it has at least one active lesson

The Order field of lessons must be unique within the same course

All deletions are logical (soft delete)

Global query filter for IsDeleted

Validation for duplicate lesson orders on create and update

## Unit Tests

Tests are located in the tests/Application.Tests project and cover the minimum required rules:

Publish course with lessons → success

Publish course without lessons → failure

Create lesson with unique order → success

Create lesson with duplicate order → failure

Delete course → soft delete

Run tests with:
```text
dotnet test
```