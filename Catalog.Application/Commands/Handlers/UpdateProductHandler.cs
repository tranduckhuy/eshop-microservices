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
            if (request == null)
            {
                throw new UpdateProductException($"The product to be updated cannot have a null value!");
            }

            var brandTask = _brandRepository.GetById(request.BrandId);
            var categoryTask = _categoryRepository.GetById(request.CategoryId);
            var productTask = _productRepository.GetById(request.Id);

            await Task.WhenAll(brandTask, categoryTask, productTask);

            var brand = await brandTask ?? throw new BrandNotFoundException(request.BrandId);
            var category = await categoryTask ?? throw new CategoryNotFoundException(request.CategoryId);
            _ = await productTask ?? throw new ProductNotFoundException($"Id '{request.Id}'");

            try
            {
                var productEntity = CatalogMapper.Mapper.Map<Product>(request);
                productEntity.Brand = brand;
                productEntity.Category = category;

                return await _productRepository.Update(productEntity);
            }
            catch (Exception ex)
            {
                throw new UpdateProductException(ex.Message);
            }
        }
    }
}
