namespace TaxPayApi.Domain.Entities;

public class Entry : BaseAuditableEntity
{
    private Entry(int sourceAccountId, int destinationAccountId, decimal amount, string? description)
    {
        SourceAccountId = sourceAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount;
        Description = description;
    }

    public int SourceAccountId { get; private set; }
    public Account? SourceAccount { get; }
    public int DestinationAccountId { get; private set; }
    public Account? DestinationAccount { get; }
    public decimal Amount { get; private set; }
    public string? Description { get; private set; }

    public static Entry CreateEntry(int sourceAccountId, int destinationAccountId, decimal amount, string? description)
    {
        return new Entry(sourceAccountId, destinationAccountId, amount, description);
    }
}