namespace TaxPayApi.Domain.Entities;

public class Account : BaseAuditableEntity
{
    private Account(string accountId, string title, decimal balance, AccountType type)
    {
        AccountId = accountId;
        Title = title;
        Balance = balance;
        Type = type;
    }

    public string AccountId { get; private set; }

    public string Title { get; private set; }

    public decimal Balance { get; private set; }
    public AccountType Type { get; private set; }

    public static Account CreateCustomerAccount(string accountId, string title, decimal balance)
    {
        return new Account(accountId, title, balance, AccountType.Customer);
    }
    
    public static Account CreateTaxAccount(string accountId, string title)
    {
        return new Account(accountId, title, 0, AccountType.Tax);
    }

    public void ModifyBalance(decimal amount, bool add)
    {
        Balance = add ? Balance + amount : Balance - amount;
    }
}