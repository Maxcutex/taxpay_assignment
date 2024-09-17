using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaxPayApi.Application.Common.Interfaces;

namespace TaxPayApi.Application.Accounts.Commands.CreateAccountList;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateAccountCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
        
        RuleFor(v => v.AccountId)
            .NotEmpty().WithMessage("Account Id is required.")
            .MaximumLength(10).WithMessage("AccountId must not exceed 10 characters.")
            .MinimumLength(10).WithMessage("AccountId must not be less than 10 characters.");
            //.MustAsync(BeUniqueAccountId).WithMessage("The specified accountId already exists.");
    }

    private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AllAsync(l => l.Title != title, cancellationToken);
    }
    
    private async Task<bool> BeUniqueAccountId(string accountId, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AllAsync(l => l.AccountId != accountId, cancellationToken);
    }
}