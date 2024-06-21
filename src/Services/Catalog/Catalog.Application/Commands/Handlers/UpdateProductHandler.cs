using Catalog.Application.Exceptions;
using Catalog.Application.Mappers;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Commands.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;

        public UpdateProductHandler(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IBrandRepository brandRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var brandTask = _brandRepository.GetById(request.BrandId);
            var categoryTask = _categoryRepository.GetById(request.CategoryId);
            var productTask = _productRepository.GetById(request.Id);

            await Task.WhenAll(brandTask, categoryTask, productTask);

            _ = await productTask ?? throw new ProductNotFoundException($"Id '{request.Id}'");
            var brand = await brandTask ?? throw new BrandNotFoundException(request.BrandId);
            var category = await categoryTask ?? throw new CategoryNotFoundException(request.CategoryId);

            var productEntity = CatalogMapper.Mapper.Map<Product>(request);
            productEntity.Brand = brand;
            productEntity.Category = category;

            return await _productRepository.Update(productEntity);
        }
    }
}
