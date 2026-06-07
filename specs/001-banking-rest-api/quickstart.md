# Quickstart: Banking REST API Mínima

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or later)

## Build

```bash
dotnet build
```

Builds both `src/BankingApi/` and `src/BankingApi.Tests/`.

## Run

```bash
dotnet run --project src/BankingApi/
```

Server starts on `http://localhost:5000` by default.

## Test

```bash
dotnet test
```

Runs all xUnit tests from `src/BankingApi.Tests/`.

## Explore API

Open Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

## Manual Smoke Test

```bash
# Get balance
curl http://localhost:5000/api/accounts/ACC-001/balance

# Transfer $300 from ACC-001 to ACC-002
curl -X POST http://localhost:5000/api/transfers \
  -H "Content-Type: application/json" \
  -d "{\"source\":\"ACC-001\",\"target\":\"ACC-002\",\"amount\":300.00}"

# Verify updated balances
curl http://localhost:5000/api/accounts/ACC-001/balance
curl http://localhost:5000/api/accounts/ACC-002/balance
```

## Endpoints

| Method | Path | Description |
|---|---|---|
| GET | `/api/accounts/{accountId}/balance` | Get account balance |
| POST | `/api/transfers` | Transfer between accounts |
| GET | `/swagger` | Swagger UI |
