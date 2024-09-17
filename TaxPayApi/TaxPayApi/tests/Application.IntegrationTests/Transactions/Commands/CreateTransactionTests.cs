using FluentAssertions;
using NUnit.Framework;
using TaxPayApi.Application.Common.Exceptions;
using TaxPayApi.Application.Transaction.Commands.CreateTransaction;
using TaxPayApi.Domain.Entities;
using TaxPayApi.Domain.Enums;
using CreateAccountCommand = TaxPayApi.Application.Accounts.Commands.CreateAccountList.CreateAccountCommand;

namespace TaxPayApi.Application.IntegrationTests.Transactions.Commands;

using static Testing;

public class CreateTransactionTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTransactionCommand();

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTransaction()
    {
        var userId = await RunAsDefaultUserAsync();

        var taxAccountId =await SendAsync(new CreateAccountCommand
        {
            Title = "Tax B",
            Type = AccountType.Tax,
            AccountId = "0000000003",
        });

        var customerAccountId =await SendAsync(new CreateAccountCommand
        {
            Title = "Customer B",
            Type = AccountType.Customer,
            AccountId = "0000000004",
            Balance = 500
        });

        var command = new CreateTransactionCommand()
        {
            SourceAccountId = customerAccountId,
            DestinationAccountId = taxAccountId,
            Amount = 50
        };
        
        var entryId = await SendAsync(command);

        var entry = await FindAsync<Entry>(entryId);
        var tax = await FindAsync<Account>(taxAccountId);
        var customer = await FindAsync<Account>(customerAccountId);

        entry.Should().NotBeNull();
        tax.Should().NotBeNull();
        tax!.Balance.Should().Be(50);
        customer.Should().NotBeNull();
        customer!.Balance.Should().Be(450);
        entry!.SourceAccountId.Should().Be(customerAccountId);
        entry!.DestinationAccountId.Should().Be(taxAccountId);
        entry!.Amount.Should().Be(50);
        entry.CreatedBy.Should().Be(userId);
        entry.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        entry.LastModifiedBy.Should().Be(userId);
        entry.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}