using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Discount.Infrastructure.Extensions
{
    public static class DiscountDbExtension
    {
        public static IServiceProvider MigrationDatabase<IContext>(this IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IContext>>();
            try
            {
                logger.LogInformation("Migrating database associated Started");
                ApplyMigration(config);
                logger.LogInformation("Migrated database associated Completed");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);    
            }
            return serviceProvider;
        }

        private static void ApplyMigration(IConfiguration config)
        {
            using var connection = new NpgsqlConnection(config["DatabaseSettings:ConnectionString"]);
            connection.Open();
            using var command = new NpgsqlCommand
            {
                Connection = connection
            };

            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();
            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
                                                            ProductName VARCHAR(255) NOT NULL,
                                                            Description TEXT,
                                                            Amount INT)";
            command.ExecuteNonQuery();

            command.CommandText = @"INSERT INTO Coupon(ProductName, Description, Amount) 
                                        VALUES('Adidas Quick Force Indoor Badminton Shoes', 'Shoes Discount', 150),
                                              ('Adidas FIFA World Cup 2018 OMB Football (White/Red/Black)', 'Football Discount', 500)";
            command.ExecuteNonQuery();
        }
    }
}
