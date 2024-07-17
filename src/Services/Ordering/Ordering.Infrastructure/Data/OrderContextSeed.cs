using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using Polly;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            // Create policy to handle exception when seeding data
            var policy = Policy.Handle<DbUpdateException>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogError("[{retry}] Exception {Message} occurred in {timeSpan}", retry, exception.Message, timeSpan);
                    });

            await policy.ExecuteAsync(async () =>
            {
                if (!await orderContext.Orders.AnyAsync())
                {
                    orderContext.Orders.AddRange(GetPreconfiguredOrders());
                    await orderContext.SaveChangesAsync();
                    logger.LogInformation($"Ordering Database: {nameof(OrderContext)} seeded.");
                }
            });
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return
            [
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
            ];
        }
    }
}
