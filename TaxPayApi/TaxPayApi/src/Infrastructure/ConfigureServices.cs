using System.Net;
using System.Net.Security;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using TaxPayApi.Application.Common.Interfaces;
using TaxPayApi.Infrastructure.Identity;
using TaxPayApi.Infrastructure.Persistence;
using TaxPayApi.Infrastructure.Persistence.Interceptors;
using TaxPayApi.Infrastructure.Services;

namespace TaxPayApi.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("TaxPayApiDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitializer>();

        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(
                options =>
                {
                    options.Clients.Add(new Client
                    {
                        ClientId = "SecureSpa",
                        AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedScopes = { "SecureSpaAPI", "openid", "profile" }
                    });
                });

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
            
        services.AddAuthorization();
        services.AddLocalApiAuthentication();
        
        IdentityModelEventSource.ShowPII = true;

        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
            (sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) !=
            SslPolicyErrors.RemoteCertificateNotAvailable;
        
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                options.Backchannel = new HttpClient(handler);
                options.RequireHttpsMetadata = false;
                options.Authority = "http://localhost:5001"; // IdentityServer URL
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            });


        return services;
    }

    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };
    }
    private static IEnumerable<Client> GetClients()
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "ro.client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets =
                {
                    new Secret("resource-owner-secret".Sha256())
                },
                AllowedScopes = { "api1" }
            }
        };
    }

    private static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("api1", "My API")
        };
    }
}