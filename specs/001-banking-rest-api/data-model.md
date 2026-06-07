# Data Model: Banking REST API

## Entidades

### Account

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| `Id` | `string` | Identificador único de cuenta | Requerido, no vacío. Ej: `ACC-001` |
| `Balance` | `decimal` | Saldo actual en USD | >= 0 |

**Datos semilla**:

| Id | Balance |
|----|---------|
| ACC-001 | 1000.00 |
| ACC-002 | 500.00 |
| ACC-003 | 0.00 |

### TransferRequest (DTO de entrada)

| Campo | Tipo | Descripción | Restricciones |
|-------|------|-------------|---------------|
| `Source` | `string` | ID de cuenta origen | Requerido, debe existir |
| `Target` | `string` | ID de cuenta destino | Requerido, debe existir, distinto de `Source` |
| `Amount` | `decimal` | Monto a transferir | > 0 |

## Reglas de Validación (Reglas de Negocio)

Todas las validaciones se ejecutan en `BankingService.Transfer()` antes de modificar cualquier saldo.

### R1 — Saldo insuficiente (FR-008)
- **Condición**: `sourceAccount.Balance < request.Amount`
- **Resultado**: Error `{ "error": "Saldo insuficiente" }`
- **Código HTTP**: 400 Bad Request

### R2 — Monto inválido (FR-006)
- **Condición**: `request.Amount <= 0`
- **Resultado**: Error `{ "error": "El monto debe ser mayor a cero" }`
- **Código HTTP**: 400 Bad Request

### R3 — Misma cuenta (FR-007)
- **Condición**: `request.Source == request.Target`
- **Resultado**: Error `{ "error": "No se puede transferir a la misma cuenta" }`
- **Código HTTP**: 400 Bad Request

### Validaciones adicionales

| Validación | Condición | Resultado | HTTP |
|------------|-----------|-----------|------|
| Cuenta origen no existe | `!accounts.ContainsKey(source)` | Error: cuenta origen no encontrada | 404 |
| Cuenta destino no existe | `!accounts.ContainsKey(target)` | Error: cuenta destino no encontrada | 404 |
| Formato ID inválido | `string.IsNullOrEmpty(id)` | Error: ID inválido | 400 |

## Atomicidad de Transferencia

La modificación de saldos (origen: debitar, destino: acreditar) usa `ConcurrentDictionary.TryUpdate` con control cíclico:

1. Leer cuenta origen → validar saldo suficiente
2. Restar monto a cuenta origen (`TryUpdate`)
3. Sumar monto a cuenta destino (`TryUpdate`)
4. Si falla algún paso, ninguna cuenta se modifica

Esto garantiza consistencia: el saldo total del sistema se conserva.
