using Ordering.Domain.Entities;

namespace Ordering.Application.Responses
{
    public record OrderResponse(
        long Id,
        string UserName,
        decimal TotalPrice,
        string FirstName,
        string LastName,
        string Email,
        string AddressLine,
        string Country,
        string State,
        string ZipCode,
        string CardName,
        string CardNumber,
        string Expiration,
        string CVV,
        PaymentMethod PaymentMethod
    );
}
