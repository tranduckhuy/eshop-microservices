using Catalog.Application.Exceptions;
using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Queries.Handlers
{
    public class GetProductByCategoryHandler : IRequestHandler<GetProductByCategoryQuery, IEnumerable<ProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByCategoryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductResponse>> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByCategory(request.CategoryName);
            if (products is not null && products.Any())
            {
                return CatalogMapper.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponse>>(products);    
            }
            throw new ProductNotFoundException($"category name '{request.CategoryName}'");
        }
    }
}
