using Catalog.Application.Exceptions;
using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Queries.Handlers
{
    public class GetProductByBrandNameHandler : IRequestHandler<GetProductByBrandNameQuery, IEnumerable<ProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByBrandNameHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductResponse>> Handle(GetProductByBrandNameQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByBrand(request.BrandName);

            if (products is not null && products.Any())
            {
                return CatalogMapper.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponse>>(products);
            }

            throw new ProductNotFoundException($"brand name {request.BrandName}");
        }
    }
}
