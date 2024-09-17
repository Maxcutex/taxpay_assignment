using FluentAssertions;
using NUnit.Framework;
using TaxPayApi.Application.Accounts.Queries.GetAccounts;
using TaxPayApi.Domain.Entities;
using TaxPayApi.Domain.Enums;

namespace TaxPayApi.Application.IntegrationTests.Accounts.Queries;

using static Testing;

public class GetAccountsTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnAllAccounts()
    {
        await RunAsDefaultUserAsync();

        await AddAsync(Account.CreateTaxAccount("0000000001", "Tax A"));

        var query = new GetAccountsQuery();

        var result = await SendAsync(query);

        result.Should().HaveCount(1);
        result.First().Type.Should().Be(AccountType.Tax.ToString());
    }

    [Test]
    public async Task ShouldDenyAnonymousUser()
    {
        var query = new GetAccountsQuery();

        var action = () => SendAsync(query);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}