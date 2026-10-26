# Zapper Payments Integration

A payment processing platform implementing Clean Architecture with PostgreSQL persistence, Acme payment provider integration, webhook processing, reporting, and automated tests.

---

# Architecture

The solution follows Clean Architecture principles with clear separation of concerns.

## Domain

Contains:

- Entities
- Enums
- Domain rules
- Payment lifecycle and state transitions

The Domain project has no infrastructure or framework dependencies.

## Application

Contains:

- Service interfaces
- Use-case implementations
- DTOs
- Repository abstractions
- Reporting contracts

Application services coordinate business workflows and enforce business rules.

## Infrastructure

Contains:

- Entity Framework Core
- PostgreSQL persistence
- Repository implementations
- Acme HTTP client
- HMAC signature validation
- Database configuration
- Seed data

## API

Contains:

- Minimal API endpoints
- Dependency injection
- Application composition root
- Swagger configuration

## Acme Stub

A supporting service used to simulate payment provider behaviour.

## Tests

Contains:

- Domain lifecycle tests
- Payment state transition tests

---

# Design Decisions

## Clean Architecture

The solution separates business rules from infrastructure concerns.

## No CQRS

CQRS, MediatR, Commands, Queries, and Handlers were intentionally not introduced.

Application services implement use cases directly to keep the codebase focused, lightweight, and easy to understand while maintaining separation of concerns.

## Idempotency

Duplicate payment requests are prevented through the use of the AppPaymentId.

## Notification Processing

The Acme webhook implementation supports:

- Duplicate notifications
- Out-of-order notifications
- Invalid state transition protection
- Signature validation

---

# Prerequisites

- .NET 10 SDK
- Docker Desktop
- Docker Compose

---

# Running the Solution

```bash
cp .env.example .env
docker compose up --build
```
# How to run the solution
Set the start up to multiple projects 
<img width="1220" height="702" alt="image" src="https://github.com/user-attachments/assets/12ced93f-fffa-42b4-b7a0-9421a86b17b1" />
Then run the solution and swagger for the API will show up 

<img width="1856" height="1019" alt="image" src="https://github.com/user-attachments/assets/b1ee0064-f8b5-400b-892d-a0babc919413" />


## Services
Payments API

http://localhost:8080/swagger

Acme Stub

http://localhost:8082/swagger

PostgreSQL

localhost:5432

---

# Seed Data

Customers

- customer-001
- customer-002

Merchants

- merchant-001
- merchant-002

---

# Useful Endpoints

Create Payment

POST /api/payments

Get Payment

GET /api/payments/{appPaymentId}

Daily Report

GET /api/reports/daily?date=YYYY-MM-DD

---

# Running Tests

```bash
dotnet test
```

---

# Resetting the Environment

```bash
docker compose down -v
```


