# Taskflow Project

Taskflow is a .NET 8 backend service implementing project and task management with Clean Architecture, CQRS, authentication, file storage, and messaging.

This project serves both as a learning reference and a base for experimenting with architectural patterns, messaging, and storage integrations in .NET.

---

## Architecture

The solution follows Clean Architecture conventions:

- **API** → entrypoint, controllers, middleware
- **Application** → CQRS handlers, validators, abstractions
- **Domain** → core models and entities
- **Infrastructure** → EF Core persistence, Azure storage, messaging, external services

Features include:

- Entity Framework Core with SQL Server
- JWT authentication and role-based authorization
- Azure Blob Storage integration (using Azurite locally for emulation)
- RabbitMQ background processing
- MediatR pipeline implementing CQRS patterns
- FluentValidation request validation
- Serilog logging
- API documentation via Swagger

---

## Project Layout

src/
├─ TaskFlow.Api
├─ TaskFlow.Application
├─ TaskFlow.Domain
└─ TaskFlow.Infrastructure

tests/
docker-compose.yml
TaskFlow.sln


---

## Running Locally

---

### Requirements

- .NET 8 SDK
- SQL Server (local or container)
- Docker (optional)

---

#### Option A: Docker

- git clone https://github.com/Akrivos/Taskflow-Project.git
- cd Taskflow-Project
- docker compose up --build

Swagger UI will be available at:
http://localhost:8080/swagger

#### Option B: Without Docker

- git clone https://github.com/Akrivos/Taskflow-Project.git
- cd Taskflow-Project
- dotnet restore
- dotnet build
- dotnet run --project src/TaskFlow.Api

---

## Authentication
The application seeds a few users on startup (except in test environments):

| Username | Password | Role           |
| -------- | -------- | -------------- |
| admin    | Pass@123 | Admin          |
| pm       | Pass@123 | ProjectManager |
| user     | Pass@123 | User           |

---

## Docker Notes
The compose environment includes SQL Server, RabbitMQ, and Azurite (for local Blob Storage emulation), enabling a fully self-contained development setup without external cloud dependencies.
