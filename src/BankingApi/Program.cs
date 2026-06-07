using BankingApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BankingService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/accounts/{accountId}/balance", (string? accountId, BankingService service) =>
{
    if (string.IsNullOrWhiteSpace(accountId))
        return Results.BadRequest(new { error = "El ID de cuenta es requerido" });

    var account = service.GetAccount(accountId);
    if (account is null)
        return Results.NotFound(new { error = $"La cuenta {accountId} no existe" });

    return Results.Ok(new { accountId = account.AccountId, balance = account.Balance });
});

app.MapPost("/api/transfers", (TransferRequest? request, BankingService service) =>
{
    if (request is null)
        return Results.BadRequest(new { error = "Solicitud inválida" });

    if (string.IsNullOrWhiteSpace(request.Source))
        return Results.BadRequest(new { error = "La cuenta origen es requerida" });

    if (string.IsNullOrWhiteSpace(request.Target))
        return Results.BadRequest(new { error = "La cuenta destino es requerida" });

    var (success, message, source, target) = service.Transfer(request);

    if (!success)
        return Results.BadRequest(new { error = message });

    return Results.Ok(new
    {
        message,
        source = new { accountId = source!.AccountId, balance = source.Balance },
        target = new { accountId = target!.AccountId, balance = target.Balance }
    });
});

app.Run();
