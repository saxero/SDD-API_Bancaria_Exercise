# Research: Banking REST API Mínima

**Date**: 2026-06-07 | **Status**: Complete

## Overview

All technical context was fully specified upfront. No NEEDS CLARIFICATION found. This document captures key decisions and rationale.

---

## 1. API Framework: ASP.NET Core Minimal APIs

- **Decision**: Use Minimal APIs (not MVC Controllers)
- **Rationale**: Fewer files, less boilerplate, natural fit for 2-endpoint API. Aligns with KISS principle (Constitution I).
- **Alternatives considered**: MVC Controllers (more ceremony, requires Controller base class, action methods, routing attributes — overkill for 2 endpoints)

## 2. In-Memory Storage: ConcurrentDictionary

- **Decision**: `ConcurrentDictionary<string, Account>` registered as Singleton
- **Rationale**: Thread-safe by default, simple API, no external dependencies. Meets spec requirement for volatile in-memory data.
- **Alternatives considered**: `Dictionary` + `lock` (more error-prone), `ImmutableDictionary` (allocation overhead on every write)

## 3. Testing: xUnit + FluentAssertions

- **Decision**: xUnit v2 with FluentAssertions (optional per Constitution V)
- **Rationale**: xUnit is the .NET standard. FluentAssertions improves test readability for balance assertions.
- **Alternatives considered**: NUnit (less idiomatic in modern .NET), MSTest (less feature-rich assertions)

## 4. API Documentation: Swashbuckle (Swagger)

- **Decision**: Swashbuckle.AspNetCore v9.x
- **Rationale**: De facto standard for ASP.NET Core OpenAPI docs. Required by Constitution (VI).
- **Alternatives considered**: NSwag (more features but heavier), Scalar (newer, less ecosystem support)

## 5. Error Handling Pattern

- **Decision**: JSON error body `{ "error": "<message>" }` with appropriate HTTP status codes
- **Rationale**: Simple, predictable, matches FR-010. No custom middleware needed — return via `Results.BadRequest()` / `Results.NotFound()`.
- **HTTP codes used**: 200 OK (success), 400 Bad Request (validation errors), 404 Not Found (account missing)

## 6. Consistency Model

- **Decision**: Optimistic check-then-act with total balance invariant verified after transfers
- **Rationale**: Single-threaded Singleton service; no concurrent transfer conflicts in lab scope. ConcurrentDictionary guarantees atomic reads/writes per key.
- **Invariant**: Sum of all account balances remains constant after each transfer.

## 7. Seed Data

- **Decision**: Three accounts hardcoded on startup in Program.cs
- **Rationale**: Spec requires ACC-001 ($1000), ACC-002 ($500), ACC-003 ($0). No persistence means re-seeding on every restart is correct behavior.

---

## Dependency Versions

| Dependency | Version | Notes |
|---|---|---|
| .NET SDK | 9.0.x | Latest stable |
| xUnit | 2.9.x | Latest stable |
| FluentAssertions | 7.0.x | Latest stable |
| Swashbuckle.AspNetCore | 7.0.x | Latest compatible with .NET 9 |

## Risks

| Risk | Mitigation |
|---|---|
| ConcurrentDictionary only guarantees per-key atomicity, not cross-key atomicity | Lock both accounts during transfer (lock ordering by key hash to prevent deadlock) |
| Swashbuckle v7 API changes from v6 | Pin version, verify OpenAPI endpoint at `/swagger` |
