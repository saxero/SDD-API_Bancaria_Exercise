# Data Model: Banking REST API Mínima

**Date**: 2026-06-07 | **Status**: Complete

## Entities

### Account

| Field | Type | Constraints | Description |
|---|---|---|---|
| `AccountId` | `string` | PK, Required, Non-empty, Alphanumeric | Unique account identifier (e.g., ACC-001) |
| `Balance` | `decimal` | Required, >= 0, 2 decimal places | Current account balance in USD |

**Seed Data**:

| AccountId | Balance |
|---|---|
| ACC-001 | 1000.00 |
| ACC-002 | 500.00 |
| ACC-003 | 0.00 |

**Invariant**: Balance must never be negative at rest. During transfer, source balance debited before credit (intermediate negative not possible since validation precedes mutation).

---

### TransferRequest (DTO — not persisted)

| Field | Type | Constraints | Description |
|---|---|---|---|
| `Source` | `string` | Required, Non-empty, != Target | Source account ID |
| `Target` | `string` | Required, Non-empty, != Source | Target account ID |
| `Amount` | `decimal` | Required, > 0, 2 decimal places | Amount to transfer in USD |

---

### ApiError (DTO — response only)

| Field | Type | Constraints | Description |
|---|---|---|---|
| `Error` | `string` | Required | Human-readable error message |

---

### BalanceResponse (DTO — response only)

| Field | Type | Constraints | Description |
|---|---|---|---|
| `AccountId` | `string` | Required | Account identifier |
| `Balance` | `decimal` | Required, >= 0 | Current balance |

---

### TransferResponse (DTO — response only)

| Field | Type | Constraints | Description |
|---|---|---|---|
| `Message` | `string` | Required | Success confirmation message |
| `Source` | `BalanceResponse` | Required | Source account post-transfer state |
| `Target` | `BalanceResponse` | Required | Target account post-transfer state |

---

## Validation Rules

### Business Rules (domain layer — BankingService)

| Rule | Condition | Error Message |
|---|---|---|
| Source exists | Source account not found in store | "La cuenta origen no existe" |
| Target exists | Target account not found in store | "La cuenta destino no existe" |
| Different accounts | Source == Target | "La cuenta origen y destino deben ser diferentes" |
| Amount positive | Amount <= 0 | "El monto debe ser mayor a cero" |
| Sufficient balance | Source.Balance < Amount | "Saldo insuficiente" |

### Input Validation (API layer — Program.cs)

| Rule | Condition | Error Message |
|---|---|---|
| AccountId non-empty | accountId is null or empty | "El ID de cuenta es requerido" |
| TransferRequest body valid | Body is null or malformed | "Solicitud inválida" |
| Source field required | Source is null or empty | "La cuenta origen es requerida" |
| Target field required | Target is null or empty | "La cuenta destino es requerida" |

---

## State Transitions

### Transfer Lifecycle

```
[Request Received]
       |
       v
[Validate Input] ──Failure──> [Return 400 Bad Request]
       |
      Success
       |
       v
[Validate Business Rules] ──Failure──> [Return 400 Bad Request]
       |
      Success
       |
       v
[Deduct Source.Balance -= Amount]
       |
       v
[Add Target.Balance += Amount]
       |
       v
[Return 200 OK with updated balances]
```

### Atomicity Guarantee

Both balance updates MUST complete or neither. Implementation: lock on a composite key derived from both AccountId values (ordered by hash to prevent deadlock), then perform both modifications within the lock.

---

## Balance Invariant

**Total Supply**: Sum of all account balances === constant ($1,500 seed total).

Verified after every successful transfer by assertion in test code. Not enforced at runtime in production (lab environment — no concurrent external mutations possible).
