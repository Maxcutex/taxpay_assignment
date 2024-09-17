using MediatR;
using TaxPayApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaxPayApi.Application.Accounts.Commands.CreateAccountList;
using TaxPayApi.Domain.Enums;

namespace TaxPayApi.Infrastructure.Persistence;

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ISender mediator)
{
    
    private static string? _currentUserId;

    
    public async Task InitialiseAsync()
    {
        try
        {
            if (context.Database.IsSqlServer())
            {
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAdminAsync()
    {
        try
        {
            await TrySeedAdminAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding admin to the database");
            throw;
        }
    }
    
    public async Task SeedAccountsAsync()
    {
        try
        {
            await TrySeedAccountsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding accounts to the database");
            throw;
        }
    }
    
    public string? GetCurrentUserId()
    {
        return _currentUserId;
    }
    
    private async Task TrySeedAdminAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole("Administrator");

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser
            { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            var seedAdmin =await userManager.CreateAsync(administrator, "Administrator1!");

            if (seedAdmin.Succeeded)
            {
                _currentUserId = administrator.Id;
            }
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }
    }
    
      private async Task TrySeedAccountsAsync()
    {
        string[] taxAccountIds =
         [
            "1000000001",
            "1000000002",
            "1000000003",
            "1000000004",
            "1000000005",
            "1000000006",
            "1000000007",
            "1000000008",
            "1000000009",
            "1000000010",
            "1000000011",
            "1000000012",
            "1000000013",
            "1000000014",
            "1000000015",
        ];
        
        string[] customerAccountIds =
        [
            "2000000001",
            "2000000002",
            "2000000003",
            "2000000004",
            "2000000005",
            "2000000006",
            "2000000007",
            "2000000008",
            "2000000009",
            "2000000010",
            "2000000011",
            "2000000012",
            "2000000013",
            "2000000014",
            "2000000015",
        ];

        for (int i = 0; i < taxAccountIds.Length; i++)
        {
            Console.WriteLine($"Tax {taxAccountIds[i]}");
            var taxAccountCommand = new CreateAccountCommand
            {
                AccountId = taxAccountIds[i],
                Title = $"Tax {taxAccountIds[i]}",
                Balance = 0,
                Type = AccountType.Tax
            };

            var taxId = await mediator.Send(taxAccountCommand);
            
            Random random = new Random();
        
            // Generate a random decimal between 500 and 5000
            decimal minValue = 500m;
            decimal maxValue = 5000m;
            
            var accountAccountCommand = new CreateAccountCommand
            {
                AccountId = customerAccountIds[i],
                Title = $"Customer {customerAccountIds[i]}",
                Balance = Math.Round((decimal)random.NextDouble() * (maxValue - minValue) + minValue, 2),
                Type = AccountType.Customer
            };
            await mediator.Send(accountAccountCommand);
        }
    }
    
}