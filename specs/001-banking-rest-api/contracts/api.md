# API Contract: Banking REST API

**Base URL**: `http://localhost:5000`

**Content-Type**: `application/json`

**Error format**: `{ "error": "mensaje descriptivo" }`

---

## GET /accounts/{id}/balance

Consulta el saldo de una cuenta.

### Request

| Parámetro | Tipo | Ubicación | Requerido | Descripción |
|-----------|------|-----------|-----------|-------------|
| `id` | `string` | Path | Sí | ID de cuenta (ej: ACC-001) |

### Response 200 OK

```json
{
  "accountId": "ACC-001",
  "balance": 1000.00
}
```

### Response 400 Bad Request

```json
{
  "error": "El ID de cuenta es inválido"
}
```

### Response 404 Not Found

```json
{
  "error": "Cuenta no encontrada: ACC-999"
}
```

---

## POST /transfers

Transfiere dinero entre dos cuentas.

### Request Body

```json
{
  "source": "ACC-001",
  "target": "ACC-002",
  "amount": 300.00
}
```

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `source` | `string` | Sí | ID de cuenta origen |
| `target` | `string` | Sí | ID de cuenta destino |
| `amount` | `decimal` | Sí | Monto a transferir (> 0) |

### Response 200 OK

```json
{
  "message": "Transferencia exitosa",
  "source": "ACC-001",
  "target": "ACC-002",
  "amount": 300.00
}
```

### Response 400 Bad Request — Monto inválido

```json
{
  "error": "El monto debe ser mayor a cero"
}
```

### Response 400 Bad Request — Misma cuenta

```json
{
  "error": "No se puede transferir a la misma cuenta"
}
```

### Response 400 Bad Request — Saldo insuficiente

```json
{
  "error": "Saldo insuficiente"
}
```

### Response 404 Not Found — Cuenta no existe

```json
{
  "error": "Cuenta origen no encontrada: ACC-999"
}
```

---

## Swagger UI

Disponible en `http://localhost:5000/swagger` en entorno Development.
