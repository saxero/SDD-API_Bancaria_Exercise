# Implementation Plan: Banking REST API Mínima

**Branch**: `main` | **Date**: 2026-06-07 | **Spec**: `specs/001-banking-rest-api/spec.md`

**Input**: Banking REST API minimalista con consulta de saldo y transferencias entre cuentas.

## Summary

API REST minimalista para operaciones bancarias básicas (consulta de saldo y transferencias) implementada con .NET 9 Minimal APIs, almacenamiento en memoria con `ConcurrentDictionary`, sin base de datos, sin autenticación. Arquitectura plana de máximo 3 archivos .cs en el proyecto principal.

## Technical Context

**Language/Version**: C# 13 / .NET 9.0 SDK

**Primary Dependencies**: ASP.NET Core (built-in), Swashbuckle.AspNetCore v9.0.x

**Storage**: `ConcurrentDictionary<string, Account>` en memoria (Singleton). Tres cuentas semilla: ACC-001 ($1000), ACC-002 ($500), ACC-003 ($0).

**Testing**: xUnit v2 (proyecto separado `src/BankingApi.Tests/`). Sin mocks — pruebas contra instancia directa de `BankingService`.

**Target Platform**: Cualquier SO con .NET 9 Runtime (Windows, Linux, macOS)

**Project Type**: web-service (REST API)

**Performance Goals**: < 500ms por request (datos en memoria, sin I/O de red externa)

**Constraints**: Sin base de datos, sin autenticación/autorización, sin HTTPS, sin librerías de terceros (excepto Swashbuckle). Máximo 5 archivos en `src/`. Moneda única (USD). Sin historial de transacciones.

**Scale/Scope**: Laboratorio — datos en memoria volátil. 3 cuentas semilla. Sin sesiones de usuario.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate | Result | Justification |
|------|--------|---------------|
| I. KISS — Sin interfaces/abstracciones innecesarias | ✅ PASS | BankingService es clase concreta. Sin repositorios, sin Unit of Work, sin capas. Máximo 3 archivos .cs. |
| II. Estructura — Código bajo `src/BankingApi/` | ✅ PASS | Proyecto principal en `src/BankingApi/`, tests en `src/BankingApi.Tests/`. |
| III. Seguridad mínima — Sin auth ni HTTPS | ✅ PASS | Sin middleware de seguridad. Sin login, tokens, roles, ni certificados. |
| IV. Validaciones de dominio — Reglas en dominio, no en presentación | ✅ PASS | BankingService valida saldo insuficiente, montos <= 0, misma cuenta. Los endpoints solo delegan. |
| V. Pruebas unitarias — xUnit sin mocks | ✅ PASS | BankingServiceTests.cs prueba 3 reglas de transferencia contra instancia real. |
| VI. Código limpio — Nombres en inglés, PascalCase | ✅ PASS | Account, BankingService, TransferRequest, GetBalance, Transfer. Todo en inglés. |

**Veredicto**: Sin violaciones. Todas las puertas pasan. No se requiere Complexity Tracking.

## Project Structure

### Documentation (this feature)

```text
specs/001-banking-rest-api/
├── spec.md              # Feature specification
├── plan.md              # This file — implementation plan
├── research.md          # Phase 0 — technical decisions and rationale
├── data-model.md        # Phase 1 — entity definitions and validation rules
├── quickstart.md        # Phase 1 — how to build, run, and test
└── contracts/           # Phase 1 — API endpoint contracts
    └── api.md
```

### Source Code (repository root)

```text
src/
├── BankingApi/                          # ASP.NET Core Minimal API project
│   ├── Program.cs                       # Endpoints, DI, middleware, Swagger config
│   ├── Domain.cs                        # Account record, TransferRequest record, BankingService
│   └── BankingApi.csproj               # Project file targeting net9.0
│
└── BankingApi.Tests/                    # xUnit test project
    ├── BankingServiceTests.cs           # Unit tests for 3 transfer business rules
    └── BankingApi.Tests.csproj          # Test project file
```

**Total: 5 archivos en `src/`**

**Structure Decision**: Arquitectura plana de un solo proyecto más proyecto de tests. Sin capas (Domain, Application, Infrastructure). Sin interfaces. Sin carpetas — solo archivos en la raíz del proyecto.

## Complexity Tracking

No aplica — todas las puertas constitucionales pasan sin violaciones.
