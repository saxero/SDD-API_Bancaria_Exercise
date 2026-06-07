# API Contracts: Banking REST API Mínima

**Base URL**: `http://localhost:5000` (configurable via `ASPNETCORE_URLS`)

**Content-Type**: `application/json`

**OpenAPI**: Available at `/swagger` via Swagger UI

---

## GET /api/accounts/{accountId}/balance

Retrieve the current balance of a specific account.

### Request

| Parameter | In | Type | Required | Description |
|---|---|---|---|---|
| `accountId` | Path | `string` | Yes | Account identifier (e.g., ACC-001) |

### Response: 200 OK

```json
{
  "accountId": "ACC-001",
  "balance": 1000.00
}
```

### Response: 404 Not Found

```json
{
  "error": "La cuenta ACC-999 no existe"
}
```

### Response: 400 Bad Request

```json
{
  "error": "El ID de cuenta es requerido"
}
```

---

## POST /api/transfers

Transfer money between two accounts.

### Request

```json
{
  "source": "ACC-001",
  "target": "ACC-002",
  "amount": 300.00
}
```

### Response: 200 OK

Source and target account balances after successful transfer.

```json
{
  "message": "Transferencia exitosa",
  "source": {
    "accountId": "ACC-001",
    "balance": 700.00
  },
  "target": {
    "accountId": "ACC-002",
    "balance": 800.00
  }
}
```

### Response: 400 Bad Request — Validation Errors

```json
{ "error": "El monto debe ser mayor a cero" }
```

| Scenario | Error Message |
|---|---|
| Source == Target | "La cuenta origen y destino deben ser diferentes" |
| Amount <= 0 | "El monto debe ser mayor a cero" |
| Amount null/missing | "El monto es requerido" |
| Source null/missing | "La cuenta origen es requerida" |
| Target null/missing | "La cuenta destino es requerida" |
| Insufficient balance | "Saldo insuficiente" |
| Source account not found | "La cuenta origen no existe" |
| Target account not found | "La cuenta destino no existe" |
| Malformed JSON body | "Solicitud inválida" |

### Response: 404 Not Found

Not applicable for this endpoint — business rule violations return 400.

---

## Error Response Schema

All errors share a uniform shape:

```json
{
  "error": "Descripción del error en español"
}
```

## HTTP Status Code Usage

| Code | When |
|---|---|
| `200 OK` | Successful balance query or transfer |
| `400 Bad Request` | Any validation failure (input or business rule) |
| `404 Not Found` | Account ID does not exist in store |
