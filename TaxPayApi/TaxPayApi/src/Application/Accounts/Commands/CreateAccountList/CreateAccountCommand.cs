using MediatR;
using TaxPayApi.Application.Common.Interfaces;
using TaxPayApi.Domain.Entities;
using TaxPayApi.Domain.Enums;

namespace TaxPayApi.Application.Accounts.Commands.CreateAccountList;

public record CreateAccountCommand : IRequest<int>
{
    public string AccountId { get; init; }
    public string Title { get; init; }
    public decimal? Balance { get; init; }
    public AccountType Type { get; init; }
}

public class CreateAccountCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateAccountCommand, int>
{
    public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Type == AccountType.Customer 
            ? Account.CreateCustomerAccount(request.AccountId, request.Title, request.Balance ?? 0)
                : Account.CreateTaxAccount(request.AccountId, request.Title);

        context.Accounts.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}