using MediatR;
using TaxPayApi.Application.Common.Interfaces;
using TaxPayApi.Domain.Entities;

namespace TaxPayApi.Application.Transaction.Commands.CreateTransaction;

public record CreateTransactionCommand : IRequest<int>
{
    public int SourceAccountId { get; init; }
    public int DestinationAccountId { get; init; }
    public decimal Amount { get; init; }
    public string? Description { get; init; }
}

public class CreateTransactionCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateTransactionCommand, int>
{
    public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {

        var sourceAccount = context.Accounts.First(c => c.Id == request.SourceAccountId);
        var destinationAccount = context.Accounts.First(c => c.Id == request.DestinationAccountId);

        sourceAccount.ModifyBalance(request.Amount, false);
        destinationAccount.ModifyBalance(request.Amount, true);
        var entry = Entry.CreateEntry(request.SourceAccountId, request.DestinationAccountId, request.Amount, request.Description);

        context.Entries.Add(entry);
        await context.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}