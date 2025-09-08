using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("OrderingConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The 'OrderingConnectionString' configuration setting is missing or empty. Please check your configuration.");
            }

            services.AddDbContext<OrderContext>(options =>
            {
                options.UseNpgsql(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                });
            });

            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            return services;
        }

        public static async Task SeedDataAsync(IServiceProvider services, OrderContext context)
        {
            if (!await context.Orders.AnyAsync())
            {
                context.Orders.Add(
                    new()
                    {
                        UserName = "huydz",
                        FirstName = "Huy",
                        LastName = "Tran",
                        Email = "duchuy@eshop.com",
                        AddressLine = "Quy Nhon",
                        Country = "Vietnam",
                        TotalPrice = 1610,
                        State = "Quy Nhon",
                        ZipCode = "123456",

                        CardName = "Mastercard",
                        CardNumber = "1610200316102003",
                        CreatedBy = "Huy",
                        Expiration = "12/25",
                        CVV = "123",
                        PaymentMethod = PaymentMethod.CreditCard,
                        LastModifiedBy = "Huy",
                        LastModifiedDate = DateTime.MinValue,
                    }
                );
            }
        }

        public static async Task MigrateDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var logger = scopedServices.GetRequiredService<ILogger<OrderContext>>();
            try
            {
                var context = scopedServices.GetRequiredService<OrderContext>();
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrated database associated with context {DbContextName}", nameof(OrderContext));

                await SeedDataAsync(scopedServices, context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred migrating the database used on context {DbContextName}", nameof(OrderContext));
            }
        }
    }
}
