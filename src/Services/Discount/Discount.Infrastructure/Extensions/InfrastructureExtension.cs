using Discount.Domain.Repositories;
using Discount.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Discount.Infrastructure.Extensions
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.MigrationDatabase<object>();
            return services;
        }
    }
}
