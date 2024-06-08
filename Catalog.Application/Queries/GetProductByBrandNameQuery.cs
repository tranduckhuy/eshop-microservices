using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetProductByBrandNameQuery(string brandName) : IRequest<IEnumerable<ProductResponse>>
    {
        public string BrandName { get; set; } = brandName;
    }
}
