using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using Swashbuckle.AspNetCore.SwaggerUI;
using TaxPayApi.Application;
using TaxPayApi.Application.Accounts.Queries.GetAccounts;
using TaxPayApi.Application.Accounts.Queries.GetAccountsWithPagination;
using TaxPayApi.Application.Common.Models;
using TaxPayApi.Application.Transaction.Commands.CreateTransaction;
using TaxPayApi.Infrastructure;
using TaxPayApi.Infrastructure.Persistence;
using WebApi;
using WebApi.Middleware;
using OpenApiSecurityScheme = Microsoft.OpenApi.Models.OpenApiSecurityScheme;
using OpenApiServer = Microsoft.OpenApi.Models.OpenApiServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebApiServices();

builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "TaxPayApi API";
    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.OAuth2,
        Name = "Bearer",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}.",
    });

    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", cors =>
    {
        cors.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseMigrationsEndPoint();

// Initialise and seed database
using var scope = app.Services.CreateScope(); // Create the scope

var services = scope.ServiceProvider; // Correct way to access the scoped service provider

// Now you can use the service provider to resolve your dependencies
var initializer = services.GetRequiredService<ApplicationDbContextInitializer>();
await initializer.InitialiseAsync();
await initializer.SeedAdminAsync();

await initializer.SeedAccountsAsync();


app.UseHealthChecks("/health");

app.UseSwagger(c =>
{
    c.RouteTemplate = "docs/{documentName}/openapi.json";
    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Servers = new List<OpenApiServer>
    {
        new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.PathBase.Value}" }
    });
});
app.UseSwaggerUI(config =>
{
    config.SwaggerEndpoint("v1/openapi.json", "Version 1");
    config.RoutePrefix = "docs";
    config.DocExpansion(DocExpansion.List);
    config.DisplayRequestDuration();
    config.DefaultModelsExpandDepth(-1);
});

app.UseRouting();

app.UseCors("AllowOrigin");
app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapGet("/account", async ([AsParameters] GetAccountsWithPaginationQuery query, [FromServices] ISender mediator) =>
    {
        var record = await mediator.Send(query);
        return Results.Ok(record);
    }).WithName("GetAccounts")
    .WithTags("Accounts")
    .Produces<PaginatedList<AccountDto>>(StatusCodes.Status200OK)
    .RequireAuthorization();


app.MapPost("/transaction", async ([FromBody] CreateTransactionCommand command, [FromServices] ISender mediator) =>
    {
        var result = await mediator.Send(command);
        return Results.Ok(result);
    }).WithName("Transact")
    .WithTags("Transaction")
    .Produces<int>(StatusCodes.Status200OK)
    .RequireAuthorization();

app.Run();