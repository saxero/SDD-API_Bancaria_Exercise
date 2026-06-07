using BankingApi;
using FluentAssertions;
using Xunit;

namespace BankingApi.Tests;

public class BankingServiceTests
{
    private readonly BankingService _service = new();

    [Fact]
    public void GetBalance_ExistingAccount_ReturnsCorrectBalance()
    {
        var account = _service.GetAccount("ACC-001");
        account.Should().NotBeNull();
        account!.Balance.Should().Be(1000m);
    }

    [Fact]
    public void GetBalance_NonExistingAccount_ReturnsNull()
    {
        var account = _service.GetAccount("ACC-999");
        account.Should().BeNull();
    }

    [Fact]
    public void Transfer_SufficientBalance_UpdatesBothAccounts()
    {
        var (success, message, source, target) = _service.Transfer(new TransferRequest("ACC-001", "ACC-002", 300m));

        success.Should().BeTrue();
        message.Should().Be("Transferencia exitosa");
        source!.Balance.Should().Be(700m);
        target!.Balance.Should().Be(800m);
    }

    [Fact]
    public void Transfer_InsufficientBalance_ReturnsError()
    {
        var (success, message, _, _) = _service.Transfer(new TransferRequest("ACC-001", "ACC-002", 2000m));

        success.Should().BeFalse();
        message.Should().Be("Saldo insuficiente");
    }

    [Fact]
    public void Transfer_NegativeAmount_ReturnsError()
    {
        var (success, message, _, _) = _service.Transfer(new TransferRequest("ACC-001", "ACC-002", -50m));

        success.Should().BeFalse();
        message.Should().Be("El monto debe ser mayor a cero");
    }

    [Fact]
    public void Transfer_ZeroAmount_ReturnsError()
    {
        var (success, message, _, _) = _service.Transfer(new TransferRequest("ACC-001", "ACC-002", 0m));

        success.Should().BeFalse();
        message.Should().Be("El monto debe ser mayor a cero");
    }

    [Fact]
    public void Transfer_SameAccount_ReturnsError()
    {
        var (success, message, _, _) = _service.Transfer(new TransferRequest("ACC-001", "ACC-001", 100m));

        success.Should().BeFalse();
        message.Should().Be("La cuenta origen y destino deben ser diferentes");
    }

    [Fact]
    public void Transfer_ExactBalance_LeavesSourceAtZero()
    {
        var (success, message, source, target) = _service.Transfer(new TransferRequest("ACC-001", "ACC-002", 1000m));

        success.Should().BeTrue();
        message.Should().Be("Transferencia exitosa");
        source!.Balance.Should().Be(0m);
        target!.Balance.Should().Be(1500m);
    }

    [Fact]
    public void Transfer_NonExistingSource_ReturnsError()
    {
        var (success, message, _, _) = _service.Transfer(new TransferRequest("ACC-999", "ACC-002", 100m));

        success.Should().BeFalse();
        message.Should().Be("La cuenta origen no existe");
    }

    [Fact]
    public void Transfer_NonExistingTarget_ReturnsError()
    {
        var (success, message, _, _) = _service.Transfer(new TransferRequest("ACC-001", "ACC-999", 100m));

        success.Should().BeFalse();
        message.Should().Be("La cuenta destino no existe");
    }
}
