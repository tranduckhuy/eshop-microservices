using MediatR;

namespace Ordering.Application.Commands
{
    public record DeleteOrderCommand(long Id) : IRequest<Unit>;
}
