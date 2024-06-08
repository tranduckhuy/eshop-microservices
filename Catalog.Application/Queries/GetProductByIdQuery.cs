using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetProductByIdQuery(Guid id) : IRequest<ProductResponse>
    {
        public Guid Id { get; set; } = id;
    }
}
