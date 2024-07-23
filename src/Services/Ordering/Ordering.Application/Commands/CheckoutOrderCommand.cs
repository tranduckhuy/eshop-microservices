using MediatR;
using Ordering.Domain.Entities;

namespace Ordering.Application.Commands
{
    public record CheckoutOrderCommand(
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
    ) : IRequest<long>;
}
