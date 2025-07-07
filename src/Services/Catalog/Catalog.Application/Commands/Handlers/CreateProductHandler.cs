using Catalog.Application.Exceptions;
using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Commands.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;

        public CreateProductHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
        }

        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.GetById(request.BrandId) ?? throw new BrandNotFoundException(request.BrandId);
            var category = await _categoryRepository.GetById(request.CategoryId) ?? throw new CategoryNotFoundException(request.CategoryId);

            var productEntity = CatalogMapper.Mapper.Map<Product>(request);
            productEntity.Brand = brand;
            productEntity.Category = category;

            var newProduct = await _productRepository.Create(productEntity);
            var productResponse = CatalogMapper.Mapper.Map<ProductResponse>(newProduct);
            return productResponse;
        }
    }
}
