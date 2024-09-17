using FluentAssertions;
using NUnit.Framework;
using TaxPayApi.Application.Accounts.Commands.CreateAccountList;
using TaxPayApi.Application.Common.Exceptions;
using TaxPayApi.Domain.Entities;
using TaxPayApi.Domain.Enums;

namespace TaxPayApi.Application.IntegrationTests.Accounts.Commands;

using static Testing;

public class CreateAccountTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateAccountCommand();
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        await SendAsync(new CreateAccountCommand
        {
            Title = "Tax A",
            Type = AccountType.Tax,
            AccountId = "0000000001",
        });

        var command = new CreateAccountCommand
        {
            Title = "Tax A",
            Type = AccountType.Tax,
            AccountId = "0000000002",
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }
    
    [Test]
    public async Task ShouldRequireUniqueAccountId()
    {
        await SendAsync(new CreateAccountCommand
        {
            Title = "Tax A",
            Type = AccountType.Tax,
            AccountId = "0000000001",
        });

        var command = new CreateAccountCommand
        {
            Title = "Tax B",
            Type = AccountType.Tax,
            AccountId = "0000000001",
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateAccount()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateAccountCommand
        {
            Title = "Tax A",
            Type = AccountType.Tax,
            AccountId = "0000000001",
        };

        var id = await SendAsync(command);

        var account = await FindAsync<Account>(id);

        account.Should().NotBeNull();
        account!.Title.Should().Be(command.Title);
        account.CreatedBy.Should().Be(userId);
        account.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}