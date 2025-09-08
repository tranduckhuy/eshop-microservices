using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using EShop.IdentityServer;
using EShop.IdentityServer.Data;
using EShop.IdentityServer.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEShopIdentityServer();
builder.Services.AddRazorPages();

// Cors
builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

var forwardHeaders = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

forwardHeaders.KnownNetworks.Clear();
forwardHeaders.KnownProxies.Clear();
app.UseForwardedHeaders(forwardHeaders);

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseRouting();

app.UseCors("CorsPolicy");

app.UseIdentityServer();

app.UseAuthorization();

app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (await userManager.FindByNameAsync("huydz") is null)
    {
        await userManager.CreateAsync(new ApplicationUser
        {
            UserName = "huydz",
            Email = "huytde.dev@gmail.com",
            EmailConfirmed = true,
            GivenName = "Huy",
            FamilyName = "Tran",
            PhoneNumber = "0123456789",
            PhoneNumberConfirmed = true,
        }, "j2HaO-iHui");
    }

    var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

    if (!await configurationDbContext.ApiScopes.AnyAsync())
    {
        await configurationDbContext.ApiScopes.AddRangeAsync(
            new ApiScope
            {
                Name = "catalogapi",
                DisplayName = "Catalog.API"
            }.ToEntity(),

            new ApiScope
            {
                Name = "basketapi",
                DisplayName = "Basket.API"
            }.ToEntity(),
            new ApiScope
            {
                Name = "eshopapigateway",
                DisplayName = "EshopAPIGateway"
            }.ToEntity());

        await configurationDbContext.SaveChangesAsync();
    }

    if (!await configurationDbContext.ApiResources.AnyAsync())
    {
        await configurationDbContext.ApiResources.AddRangeAsync(
            new ApiResource
            {
                Name = "9fc33c2e-dbc1-4d0a-b212-68b9e07b3ba0",
                DisplayName = "Catalog.API",
                Scopes = { "catalogapi" }
            }.ToEntity(),
            new ApiResource
            {
                Name = "89d1690f-7d79-4a3e-a9b4-8d680ce0efc1",
                DisplayName = "Basket.API",
                Scopes = { "basketapi" }
            }.ToEntity(),
            new ApiResource
            {
                Name = "e7b9c8c1-4c36-4d7b-8d43-4b9c6d8a68b9",
                DisplayName = "EshopAPIGateway",
                Scopes = { "eshopapigateway", "basketapi" }
            }.ToEntity());

        await configurationDbContext.SaveChangesAsync();
    }

    if (!await configurationDbContext.Clients.AnyAsync())
    {
        await configurationDbContext.Clients.AddRangeAsync(
            new Client
            {
                ClientId = "b4e758d2-f13d-4a1e-bf38-cc88f4e290e1",
                ClientSecrets = new List<Secret> { new("secret".Sha512()) },
                ClientName = "Catalog API Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "catalogapi" },
                AllowedCorsOrigins = new List<string> { "https://localhost:9000" }
            }.ToEntity(),
            new Client
            {
                ClientId = "c5f864d2-f13d-4a1e-bf38-cc88f4e290e1",
                ClientSecrets = { new("secret".Sha512()) },
                ClientName = "Basket API Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "basketapi" },
                AllowedCorsOrigins = new List<string> { "https://localhost:9001" }
            }.ToEntity(),
            new Client
            {
                ClientId = "b21f4e6b-0e44-4d97-8f0a-2b8d8a4f29c1",
                ClientSecrets = { new("secret".Sha512()) },
                ClientName = "Eshop Gateway Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "eshopapigateway", "basketapi" },
                AllowedCorsOrigins = new List<string> { "https://localhost:9010" }
            }.ToEntity(),


            new Client
            {
                ClientId = "4ecc4153-daf9-4eca-8b60-818a63637a81",
                ClientSecrets = new List<Secret> { new("secret".Sha512()) },
                ClientName = "Web Application",
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = new List<string> { "openid", "profile", "email", "https://www.example.com/api" },
                RedirectUris = new List<string> { "https://webapplication:7002/signin-oidc" },
                PostLogoutRedirectUris = new List<string> { "https://webapplication:7002/signout-callback-oidc" }
            }.ToEntity(),
            new Client
            {
                ClientId = "7e98ad57-540a-4191-b477-03d88b8187e1",
                RequireClientSecret = false,
                ClientName = "Single Page Application",
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = new List<string> { "openid", "profile", "email", "https://www.example.com/api" },
                AllowedCorsOrigins = new List<string> { "http://singlepageapplication:7003" },
                RedirectUris =
                    new List<string> { "http://singlepageapplication:7003/authentication/login-callback" },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://singlepageapplication:7003/authentication/logout-callback"
                }
            }.ToEntity());

        await configurationDbContext.SaveChangesAsync();
    }

    if (!await configurationDbContext.IdentityResources.AnyAsync())
    {
        await configurationDbContext.IdentityResources.AddRangeAsync(
            new IdentityResources.OpenId().ToEntity(),
            new IdentityResources.Profile().ToEntity(),
            new IdentityResources.Email().ToEntity());

        await configurationDbContext.SaveChangesAsync();
    }

    app.UseDeveloperExceptionPage();
}

await app.RunAsync();
