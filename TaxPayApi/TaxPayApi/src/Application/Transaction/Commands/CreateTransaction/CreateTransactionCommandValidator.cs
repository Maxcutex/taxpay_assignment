using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaxPayApi.Application.Common.Interfaces;
using TaxPayApi.Domain.Enums;

namespace TaxPayApi.Application.Transaction.Commands.CreateTransaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateTransactionCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(v => v.SourceAccountId)
            .Must(c=> c > 0).WithMessage("Source Account Id must be greater than zero")
            .NotEqual(c=> c.DestinationAccountId)
            .WithMessage("Source Account Id must be different from Destination Account")
            .MustAsync((c, token)=> AccountExistAsType(c, AccountType.Customer, token))
            .WithMessage("The source account does not exist.");
        
        RuleFor(v => v.DestinationAccountId)
            .Must(c=> c > 0)
            .WithMessage("Destination Account Id must be greater than zero")
            .MustAsync((c, token)=> AccountExistAsType(c, AccountType.Tax, token))
            .WithMessage("The destination account does not exist.");
        
        RuleFor(v => v.Amount)
            .Must(c=> c > 0)
            .WithMessage("Amount must be greater than zero");
        
        RuleFor(v => v.Amount)
            .MustAsync((v, amount, token) => CustomerAccountHasSufficientBalance(v.SourceAccountId, amount, token))
            .WithMessage("The source account does not have sufficient balance");
        
        
        
    }

    private async Task<bool> AccountExistAsType(int accountId, AccountType type, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.Type == type, cancellationToken) != null;
    }
    
    private async Task<bool> CustomerAccountHasSufficientBalance(int accountId, Decimal amount, CancellationToken cancellationToken)
    {
        return ((await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.Type == AccountType.Customer, cancellationToken))!).Balance >= amount;
    }
}