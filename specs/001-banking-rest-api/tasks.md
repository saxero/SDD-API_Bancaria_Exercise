# Tareas: Banking REST API Mínima

**Documentos de diseño**: `specs/001-banking-rest-api/` (spec.md, plan.md, data-model.md, contracts/api.md, quickstart.md)

**Organización**: 4 tareas secuenciales. Cada tarea incluye su implementación y pruebas — sin tareas separadas de test.

**Idioma**: Documentación en español, código fuente en inglés.

---

## T001 — Crear proyectos Web API y xUnit — ✅ COMPLETED

**Descripción**: Inicializar la solución con `dotnet new webapi` y `dotnet new xunit` según la estructura del plan.

**Archivos a crear**:
1. `src/BankingApi/BankingApi.csproj` — Proyecto Web API targeting `net9.0`, sin packages adicionales (Swashbuckle se agrega después).
2. `src/BankingApi.Tests/BankingApi.Tests.csproj` — Proyecto xUnit con referencia a proyecto `BankingApi`.
3. `src/BankingApi/Program.cs` — Mínimo necesario: `var builder = WebApplication.CreateBuilder(args); var app = builder.Build(); app.Run();`
4. `src/BankingApi/Properties/launchSettings.json` — Perfil de ejecución con `http://localhost:5000`.

**Criterio de éxito**: `dotnet build` compila sin errores. `dotnet test` descubre 0 pruebas (aún no hay).

---

## T002 — Modelos y BankingService con seed data y pruebas unitarias — ✅ COMPLETED

**Descripción**: Implementar el dominio (modelos Account/TransferRequest, BankingService con validación de negocio y seed data) y sus pruebas unitarias con xUnit.

**Archivos a crear**:
1. `src/BankingApi/Domain.cs` — Contiene:
   - `record Account(string AccountId, decimal Balance)`
   - `record TransferRequest(string Source, string Target, decimal Amount)`
   - `class BankingService` con:
     - `ConcurrentDictionary<string, Account>` interno
     - Constructor que precarga ACC-001 ($1000), ACC-002 ($500), ACC-003 ($0)
     - `Account? GetAccount(string accountId)` — retorna null si no existe
     - `(bool Success, string Message, Account? Source, Account? Target) Transfer(TransferRequest request)` — valida y ejecuta la transferencia atómicamente
     - Validaciones de negocio (en orden):
       1. Source == Target → error "La cuenta origen y destino deben ser diferentes"
       2. Amount <= 0 → error "El monto debe ser mayor a cero"
       3. Source no existe → error "La cuenta origen no existe"
       4. Target no existe → error "La cuenta destino no existe"
       5. Source.Balance < Amount → error "Saldo insuficiente"
       6. Debita Source, acredita Target, retorna éxito con saldos actualizados

2. `src/BankingApi.Tests/BankingServiceTests.cs` — Pruebas unitarias (sin mocks, instancia directa):
   - `GetBalance_ExistingAccount_ReturnsCorrectBalance` — ACC-001 → $1000
   - `GetBalance_NonExistingAccount_ReturnsNull` — ACC-999 → null
   - `Transfer_SufficientBalance_UpdatesBothAccounts` — $300 ACC-001→ACC-002, verifica $700 y $800
   - `Transfer_InsufficientBalance_ReturnsError` — $2000 ACC-001→ACC-002 → error
   - `Transfer_NegativeAmount_ReturnsError` — $-50 → error
   - `Transfer_ZeroAmount_ReturnsError` — $0 → error
   - `Transfer_SameAccount_ReturnsError` — ACC-001→ACC-001 → error
   - `Transfer_ExactBalance_LeavesSourceAtZero` — $1000 ACC-001→ACC-002, source queda $0
   - `Transfer_NonExistingSource_ReturnsError` — ACC-999→ACC-002 → error
   - `Transfer_NonExistingTarget_ReturnsError` — ACC-001→ACC-999 → error

**Criterio de éxito**: `dotnet test` — las 10 pruebas pasan.

---

## T003 — Endpoints Minimal API en Program.cs — ✅ COMPLETED

**Descripción**: Registrar `BankingService` como Singleton en DI y definir los 2 endpoints REST.

**Archivos a modificar**:
1. `src/BankingApi/Program.cs`:
   - `builder.Services.AddSingleton<BankingService>();`
   - `GET /api/accounts/{accountId}/balance`:
     - `accountId` vacío/nulo → `Results.BadRequest(new { error = "El ID de cuenta es requerido" })`
     - Cuenta no existe → `Results.NotFound(new { error = $"La cuenta {accountId} no existe" })`
     - Éxito → `Results.Ok(new { accountId, balance })`
   - `POST /api/transfers`:
     - Body nulo → `Results.BadRequest(new { error = "Solicitud inválida" })`
     - Campos faltantes → `Results.BadRequest(new { error = ... })`
     - Delegar en `BankingService.Transfer()` → si error, `Results.BadRequest(new { error })`; si éxito, `Results.Ok(new { message = "Transferencia exitosa", source, target })`

**Criterio de éxito**: `dotnet build` sin errores. API responde en los endpoints.

---

## T004 — Swagger y validación de arranque — ✅ COMPLETED

**Descripción**: Habilitar Swagger/Swashbuckle y verificar que la API arranca y responde correctamente.

**Archivos a modificar**:
1. `src/BankingApi/BankingApi.csproj` — Agregar package `Swashbuckle.AspNetCore`
2. `src/BankingApi/Program.cs`:
   - `builder.Services.AddEndpointsApiExplorer();`
   - `builder.Services.AddSwaggerGen();`
   - `app.UseSwagger();`
   - `app.UseSwaggerUI();`

**Archivos a crear**:
3. `src/BankingApi.Tests/ApiSmokeTests.cs` — Prueba de integración mínima (sin mock de servidor):
   - Verificar que `dotnet build` compila (ya cubierto)
   - La validación se hace con el smoke test manual del quickstart

**Criterio de éxito**: `dotnet build` compila. `dotnet run --project src/BankingApi/` inicia. Swagger UI accesible en `/swagger`. Endpoints responden correctamente (verificar con curl o quickstart.md).

---

## Dependencias

```
T001 (Proyectos) → T002 (Modelos + Service + Tests) → T003 (Endpoints) → T004 (Swagger + Validación)
```

Todas las tareas son secuenciales (sin paralelismo). Cada una depende de la anterior.

---

## Estrategia de implementación

**MVP**: La T001 + T002 + T003 constituyen el MVP funcional (API operativa con 2 endpoints). T004 es pulido (Swagger).

**Incrementos**:
1. T001 — Esqueleto compilable
2. T002 — Lógica de negocio completa con pruebas (núcleo del valor)
3. T003 — API expuesta y funcional (MVP listo)
4. T004 — Documentación interactiva y validación final

---

## Validación final

```bash
dotnet build                    # Sin errores ni warnings
dotnet test                     # Todas las pruebas pasan
dotnet run --project src/BankingApi/  # Servidor inicia en http://localhost:5000
# Probar en navegador: http://localhost:5000/swagger
# Probar con curl:
curl http://localhost:5000/api/accounts/ACC-001/balance
curl -X POST http://localhost:5000/api/transfers -H "Content-Type: application/json" -d "{\"source\":\"ACC-001\",\"target\":\"ACC-002\",\"amount\":300}"
```
