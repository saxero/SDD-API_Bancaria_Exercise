using System.Collections.Concurrent;

namespace BankingApi;

public record Account(string AccountId, decimal Balance);

public record TransferRequest(string Source, string Target, decimal Amount);

public class BankingService
{
    private readonly ConcurrentDictionary<string, Account> _accounts = new();

    public BankingService()
    {
        _accounts["ACC-001"] = new Account("ACC-001", 1000m);
        _accounts["ACC-002"] = new Account("ACC-002", 500m);
        _accounts["ACC-003"] = new Account("ACC-003", 0m);
    }

    public Account? GetAccount(string accountId)
    {
        _accounts.TryGetValue(accountId, out var account);
        return account;
    }

    public (bool Success, string Message, Account? Source, Account? Target) Transfer(TransferRequest request)
    {
        if (request.Source == request.Target)
            return (false, "La cuenta origen y destino deben ser diferentes", null, null);

        if (request.Amount <= 0)
            return (false, "El monto debe ser mayor a cero", null, null);

        if (!_accounts.TryGetValue(request.Source, out var source))
            return (false, "La cuenta origen no existe", null, null);

        if (!_accounts.TryGetValue(request.Target, out var target))
            return (false, "La cuenta destino no existe", null, null);

        if (source.Balance < request.Amount)
            return (false, "Saldo insuficiente", null, null);

        var (low, high) = string.Compare(request.Source, request.Target, StringComparison.Ordinal) < 0
            ? (request.Source, request.Target)
            : (request.Target, request.Source);

        lock (_accounts[low])
        {
            lock (_accounts[high])
            {
                var src = _accounts[request.Source];
                var tgt = _accounts[request.Target];

                var updatedSource = src with { Balance = src.Balance - request.Amount };
                var updatedTarget = tgt with { Balance = tgt.Balance + request.Amount };

                _accounts[request.Source] = updatedSource;
                _accounts[request.Target] = updatedTarget;

                return (true, "Transferencia exitosa", updatedSource, updatedTarget);
            }
        }
    }
}
