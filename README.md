# High-Concurrency Limited Stock Ordering System

A portfolio-grade flash-sale ordering system focused on backend correctness under concurrent purchase requests.

## Stack

- React 19, TypeScript, and Vite
- ASP.NET Core 10 Web API
- Dapper and PostgreSQL
- SignalR for committed stock updates
- ASP.NET Core partitioned rate limiting
- xUnit for API tests
- k6 for load testing
- Docker and Render for deployment

.NET Aspire is intentionally not part of the project.

## Repository layout

```text
src/
  LimitedStockOrdering.Api/       ASP.NET Core API and SignalR hub
  LimitedStockOrdering.Application/ Application services and abstractions
  LimitedStockOrdering.Infra/     Dapper, Npgsql, and persistence entities
  LimitedStockOrdering.Web/       React/Vite web application
tests/
  LimitedStockOrdering.Api.Tests/ API and integration tests
  load/                            k6 scenarios and assertions
database/
  migrations/                      Versioned PostgreSQL schema scripts
  seeds/                           Development and load-test data
documents/                         Architecture and database documentation
```

## Local setup

Requirements: .NET SDK 10, Node.js, npm, and Docker.

```powershell
docker compose up -d postgres
dotnet restore LimitedStockOrdering.sln
dotnet run --project src/LimitedStockOrdering.Api
```

In a second terminal:

```powershell
cd src/LimitedStockOrdering.Web
npm install
npm run dev
```

The API starts at `http://localhost:5171`, the web app at `http://localhost:5173`, and PostgreSQL at `localhost:5432`.

## Documentation

- [ER diagram and database design](documents/er-diagram.md)
- [Purchase flow and concurrency rules](documents/purchase-flow.md)
