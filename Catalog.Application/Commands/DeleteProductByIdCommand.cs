using MediatR;

namespace Catalog.Application.Commands
{
    public record DeleteProductByIdCommand(Guid Id) : IRequest<bool>;
}
