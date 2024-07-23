using MediatR;
using Ordering.Application.Responses;

namespace Ordering.Application.Queries
{
    public record GetOrdersByUserNameQuery(string UserName) : IRequest<IEnumerable<OrderResponse>>;
}
