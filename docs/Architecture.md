# HRVault - Arquitetura

## Objetivo

O HRVault é uma plataforma de gestão de colaboradores baseada em arquitetura modular.

O objetivo é centralizar toda a informação relacionada com um colaborador numa única aplicação.

---

# Tecnologias

Backend
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL

Infraestrutura
- Docker
- Redis
- MinIO
- Mailpit
- Seq

Frontend (futuro)
- React
- TypeScript
- Tailwind CSS

---

# Arquitetura

HRVault.Api

↓

HRVault.Application

↓

HRVault.Domain

↓

HRVault.Infrastructure

↓

PostgreSQL
MinIO
Redis

---

# Princípios

- Clean Architecture
- Domain Driven Design (DDD)
- SOLID
- CQRS
- Repository Pattern
- Dependency Injection