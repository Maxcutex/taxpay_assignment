using TaxPayApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TaxPayApi.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Entry> Entries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}