using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetProductByCategoryQuery(string categoryName) : IRequest<IEnumerable<ProductResponse>>
    {
        public string CategoryName { get; set; } = categoryName;
    }
}
