using MediatR;

namespace Catalog.Application.Commands
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string Summary,
        string Description,
        string ImageFile,
        decimal Price,
        int Quantity,
        Guid BrandId,
        Guid CategoryId
    ) : IRequest<bool>;
}
