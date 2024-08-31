using EShop.IdentityServer.Data;
using EShop.IdentityServer.Factories;
using EShop.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System.Reflection;

namespace EShop.IdentityServer
{
    public static class Extensions
    {
        public static IServiceCollection AddEShopIdentityServer(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>( (serviceProvider, dbContextOptionsBuilder) =>
            {
                dbContextOptionsBuilder.UseNpgsql(
                    connectionString: serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Identity"),
                    npgsqlOptionsAction: NpgSqlOptionsAction);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(configurationStoreOptions =>
                {
                    configurationStoreOptions.ResolveDbContextOptions = ResolveDbContextOptions;
                })
                .AddOperationalStore(operationalStoreOptions =>
                {
                    operationalStoreOptions.ResolveDbContextOptions = ResolveDbContextOptions;
                });

            //services.AddTransient<IProfileService, ProfileService>();

            return services;
        }

        private static void ResolveDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("IdentityServer");
            optionsBuilder.UseNpgsql(connectionString, NpgSqlOptionsAction);
        }

        private static void NpgSqlOptionsAction(NpgsqlDbContextOptionsBuilder sqlOptions)
        {
            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
        }
    }
}
