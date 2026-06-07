# Research: Banking REST API Mínima

## Decisiones Técnicas

### Framework: .NET 9 Minimal APIs vs Controllers

- **Decisión**: Minimal APIs
- **Justificación**: Menos archivos, menos boilerplate, sin clases de Controller, sin routing por atributos. Todo en un `Program.cs` compacto. La API tiene solo 2 endpoints, no justifica la estructura de Controllers.
- **Alternativas consideradas**: Controllers (más boilerplate para el mismo resultado), ASP.NET Core + MVC (sobreingeniería).

### Documentación: Swashbuckle.AspNetCore vs OpenAPI nativo

- **Decisión**: Swashbuckle.AspNetCore v9.0.x
- **Justificación**: Microsoft eliminó Swashbuckle del template por defecto en .NET 9, pero el paquete NuGet sigue soportando `net9.0` oficialmente (v9.0.0+). La configuración es idéntica a versiones anteriores. El usuario lo solicitó explícitamente.
- **Alternativas consideradas**: `Microsoft.AspNetCore.OpenApi` (nativo, no requiere paquete externo, pero requiere `dotnet-getdocument` CLI para UI interactiva).

### Almacenamiento: ConcurrentDictionary vs List<Account>

- **Decisión**: `ConcurrentDictionary<string, Account>`
- **Justificación**: Thread-safe sin locks manuales. O(1) en búsqueda por ID. Semántica clara para actualización de saldos con `TryUpdate`.
- **Alternativas consideradas**: `List<Account>` (requiere locks manuales), `Dictionary<string, Account>` (no thread-safe), `ImmutableDictionary` (nueva instancia en cada modificación).

### Estructura de archivos: 3 archivos planos vs 1 mega-archivo

- **Decisión**: 3 archivos .cs (`Program.cs`, `Domain.cs`, `BankingApi.csproj`)
- **Justificación**: Separación clara entre modelos/dominio (Domain.cs), endpoints/configuración (Program.cs), y metadatos del proyecto (.csproj). Suficiente para mantener el código legible sin fragmentar.
- **Alternativas consideradas**: Todo en `Program.cs` (~200 líneas, difícil de leer), separación por carpetas (sobreingeniería para 2 endpoints).

### Pruebas: xUnit sin mocks

- **Decisión**: xUnit v2, pruebas contra instancia real de `BankingService`
- **Justificación**: `BankingService` no tiene dependencias externas (ni DB, ni HTTP, ni I/O). No hay nada que mockear. Las pruebas crean una instancia fresca, operan directamente y verifican saldos.
- **Alternativas consideradas**: Moq (innecesario — no hay interfaces), NUnit (equivalente, pero xUnit es estándar en .NET).

### Validaciones: En dominio vs middleware/filtros

- **Decisión**: Validación en `BankingService.Transfer()`
- **Justificación**: Las reglas de negocio (saldo insuficiente, monto inválido, misma cuenta) pertenecen al dominio. El endpoint solo traduce el resultado HTTP.
- **Alternativas consideradas**: FluentValidation (descartado por restricción), Data Annotations (solo validación de formato, no de negocio), middleware de validación (separa la regla del dominio).

## Resumen de Paquetes NuGet

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| `Swashbuckle.AspNetCore` | 9.0.x | Swagger UI + generación OpenAPI |
| `xunit` | 2.9.x | Framework de pruebas (test project) |
| `Microsoft.NET.Test.Sdk` | 17.x | Test runner (test project) |
| `coverlet.collector` | 6.x | Cobertura de código (test project) |

Sin otras dependencias externas.
