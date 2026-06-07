# Quickstart: Banking REST API

## Requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Git (opcional)

## Construir y ejecutar

```bash
# Ir al directorio del proyecto
cd src/BankingApi

# Restaurar dependencias
dotnet restore

# Ejecutar en modo Development (Swagger habilitado)
dotnet run

# El servidor inicia en http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

## Ejecutar pruebas

```bash
# Desde la raíz del repositorio
dotnet test src/BankingApi.Tests/BankingApi.Tests.csproj

# O desde cualquier directorio
dotnet test
```

## Ejemplos de uso (curl)

### Consultar saldo

```bash
curl http://localhost:5000/accounts/ACC-001/balance
```

### Transferencia exitosa

```bash
curl -X POST http://localhost:5000/transfers \
  -H "Content-Type: application/json" \
  -d '{"source":"ACC-001","target":"ACC-002","amount":300.00}'
```

### Transferencia rechazada (saldo insuficiente)

```bash
curl -X POST http://localhost:5000/transfers \
  -H "Content-Type: application/json" \
  -d '{"source":"ACC-001","target":"ACC-002","amount":99999.00}'
```

## Datos semilla

Al iniciar, el sistema precarga 3 cuentas:

| Cuenta | Saldo |
|--------|-------|
| ACC-001 | $1000.00 |
| ACC-002 | $500.00 |
| ACC-003 | $0.00 |

Al reiniciar el servidor, los saldos vuelven a estos valores.
