using System.Reflection;
using TaxPayApi.Application.Common.Interfaces;
using TaxPayApi.Domain.Entities;
using TaxPayApi.Infrastructure.Identity;
using TaxPayApi.Infrastructure.Persistence.Interceptors;
using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TaxPayApi.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IOptions<OperationalStoreOptions> operationalStoreOptions,
    IMediator mediator,
    AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
    : ApiAuthorizationDbContext<ApplicationUser>(options, operationalStoreOptions), IApplicationDbContext
{
    private readonly IMediator _mediator = mediator;

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Entry> Entries  => Set<Entry>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}