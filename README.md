# TaskFlow (Clean Architecture + CQRS + RabbitMQ + Azure Blob)

ASP.NET Core 8 Web API scaffold with:
- Clean Architecture (Domain, Application, Infrastructure, API)
- CQRS via MediatR
- EF Core (SQL Server)
- File uploads to Azure Blob
- RabbitMQ publisher + background consumer
- Swagger + Serilog

## Getting Started

1. Install prerequisites:
   - .NET 8 SDK
   - SQL Server (or localdb)
   - RabbitMQ (docker: `docker run -d --name rabbit -p 5672:5672 -p 15672:15672 rabbitmq:3-management`)
   - Azurite for local blob (docker: `docker run -p 10000:10000 mcr.microsoft.com/azure-storage/azurite`)

2. Restore & build:
   ```bash
   dotnet restore
   dotnet build
   ```

3. Run API:
   ```bash
   cd src/TaskFlow.Api
   dotnet run
   ```

4. Open Swagger: https://localhost:5001/swagger

## EF Core
The API auto-creates the DB (EnsureCreated). Switch to migrations when ready.

## Queues
- Publisher is used in command handlers (project/task created).
- Consumer runs as hosted service and logs messages.

## File Uploads
POST `/api/files/{taskId}/upload` with form-data file field `file`.


## What changed in v2
- MediatR ValidationBehavior (global)
- Validators for CreateProject & CreateTask
- MVC auto-validation enabled
- Safer file upload (size/type)
- Removed EnsureCreated(); use EF migrations


## 🔐 Authentication (JWT Bearer)
- Endpoint: `POST /api/auth/login`
- Body:
```json
{ "username": "admin", "password": "Pass@123" }
```
- Demo users:
  - admin / Pass@123  (role: Admin)
  - pm / Pass@123     (role: ProjectManager)
  - user / Pass@123   (role: User)

Use the returned `access_token` as `Authorization: Bearer <token>` in subsequent calls.
Swagger is configured to accept the token (Authorize button).

## 🐳 Docker Compose (API + SQL Server + RabbitMQ + Azurite)
Build & run:
```bash
docker compose up --build
```
API URL: `http://localhost:8080/swagger`

> Connection strings & JWT secrets περνάνε ως environment variables (βλ. docker-compose.yml).



## 👥 Roles & Permissions
- **Admin**: full access (can manage everything)
- **ProjectManager**: create/read/update projects, create/read tasks, upload files
- **User**: read projects, create/read tasks assigned to them (demo policy allows read/create)

Policies (examples configured):
- `Projects.Create` → ProjectManager, Admin
- `Projects.Read`   → User, ProjectManager, Admin
- `Tasks.Create`    → User, ProjectManager, Admin
- `Tasks.Read`      → User, ProjectManager, Admin
- `Files.Upload`    → User, ProjectManager, Admin

## 🔔 Notifications Microservice
- Νέο project **TaskFlow.NotificationsWorker** (Console/Worker)
- Καταναλώνει από RabbitMQ τις ουρές: `project-created`, `task-created`
- Τρέχει ως ξεχωριστό container (`notifier`) στο docker-compose

## 🚀 Run all
```bash
docker compose up --build
```
API: http://localhost:8080/swagger
RabbitMQ UI: http://localhost:15672 (guest/guest)
