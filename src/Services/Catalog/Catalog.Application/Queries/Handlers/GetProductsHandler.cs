using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Domain.Specs;
using MediatR;

namespace Catalog.Application.Queries.Handlers
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, Pagination<ProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Pagination<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetProducts(request.CatalogSpecParams);
            return CatalogMapper.Mapper.Map<Pagination<Product>, Pagination<ProductResponse>>(products);
        }
    }
}
