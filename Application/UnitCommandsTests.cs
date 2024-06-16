using Catalog.Application.Commands;
using Catalog.Application.Commands.Handlers;
using Catalog.Application.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Application
{
    public class UnitCommandsTests
    {
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<IBrandRepository> _brandRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _brandRepositoryMock = new Mock<IBrandRepository>();
        }

        // Helper method to setup mocks
        private void SetupBrandRepositoryMock(Guid brandId, Brand? brand)
        {
            _brandRepositoryMock.Setup(repo => repo.GetById(brandId))
                .ReturnsAsync(brand);
        }

        private void SetupCategoryRepositoryMock(Guid categoryId, Category? category)
        {
            _categoryRepositoryMock.Setup(repo => repo.GetById(categoryId))
                .ReturnsAsync(category);
        }

        private void SetupProductRepositoryMock(Guid productId, Product? product)
        {
            _productRepositoryMock.Setup(repo => repo.GetById(productId))
                .ReturnsAsync(product);
        }

        protected static CreateProductCommand CreateValidCreateProductCommand()
        {
            return new CreateProductCommand(
                Name: "Product Name",
                Summary: "Product Summary",
                Description: "Product Description",
                ImageFile: "Product Picture",
                Price: 100m,
                Quantity: 10,
                BrandId: Guid.NewGuid(),
                CategoryId: Guid.NewGuid()
            );
        }

        protected static UpdateProductCommand CreateValidUpdateProductCommand()
        {
            return new UpdateProductCommand(
                Id: Guid.NewGuid(),
                Name: "Product Name",
                Summary: "Product Summary",
                Description: "Product Description",
                ImageFile: "Product Picture",
                Price: 100m,
                Quantity: 10,
                BrandId: Guid.NewGuid(),
                CategoryId: Guid.NewGuid()
            );
        }

        #region CreateProductCommandTests
        [Test]
        public async Task CreateProduct_Throws_BrandNotFoundException_When_Brand_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidCreateProductCommand();

            // Simulate brand not found
            SetupBrandRepositoryMock(command.BrandId, null);

            var handler = new CreateProductHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object, _brandRepositoryMock.Object);

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<BrandNotFoundException>()
                .WithMessage($"Brand with id {command.BrandId} not found.");
        }

        [Test]
        public async Task CreateProduct_Throws_BrandNotFoundException_When_Brand_Exists_But_Category_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidCreateProductCommand();

            // Simulate brand found
            SetupBrandRepositoryMock(command.BrandId, new Brand());
            // Simulate category not found
            SetupCategoryRepositoryMock(command.CategoryId, null);

            var handler = new CreateProductHandler(
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _brandRepositoryMock.Object
            );

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<CategoryNotFoundException>()
                .WithMessage($"Category with id {command.CategoryId} not found.");
        }

        [Test]
        public async Task CreateProductSuccess()
        {
            // Arrange
            var command = CreateValidCreateProductCommand();

            // Simulate brand found
            SetupBrandRepositoryMock(command.BrandId, new Brand());
            // Simulate category found
            SetupCategoryRepositoryMock(command.CategoryId, new Category());

            var handler = new CreateProductHandler(
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _brandRepositoryMock.Object
            );

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().NotThrowAsync();
        }
        #endregion

        #region UpdateProductCommandTests
        [Test]
        public async Task UpdateProduct_Throws_ProductNotFoundException_When_Product_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product not found
            SetupProductRepositoryMock(command.Id, null);

            var handler = new UpdateProductHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object, _brandRepositoryMock.Object);

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ProductNotFoundException>()
                .WithMessage($"Product with id '{command.Id}' was not found.");
        }

        [Test]
        public async Task UpdateProduct_Throws_BrandNotFoundException_When_Brand_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product found
            SetupProductRepositoryMock(command.Id, new Product());
            // Simulate brand not found
            SetupBrandRepositoryMock(command.BrandId, null);

            var handler = new UpdateProductHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object, _brandRepositoryMock.Object);

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<BrandNotFoundException>()
                .WithMessage($"Brand with id {command.BrandId} not found.");
        }

        [Test]
        public async Task UpdateProduct_Throws_BrandNotFoundException_When_Brand_Exists_But_Category_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product found
            SetupProductRepositoryMock(command.Id, new Product());
            // Simulate brand found
            SetupBrandRepositoryMock(command.BrandId, new Brand());
            // Simulate category not found
            SetupCategoryRepositoryMock(command.CategoryId, null);

            var handler = new UpdateProductHandler(
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _brandRepositoryMock.Object
            );

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<CategoryNotFoundException>()
                .WithMessage($"Category with id {command.CategoryId} not found.");
        }

        [Test]
        public async Task UpdateProductSuccess()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product found
            SetupProductRepositoryMock(command.Id, new Product());
            // Simulate brand found
            SetupBrandRepositoryMock(command.BrandId, new Brand());
            // Simulate category found
            SetupCategoryRepositoryMock(command.CategoryId, new Category());

            var handler = new UpdateProductHandler(
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _brandRepositoryMock.Object
            );

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().NotThrowAsync();
        }
        #endregion

        #region DeleteProductCommandTests
        [Test]
        public async Task DeleteProduct_Throws_ProductNotFoundException_When_Product_Does_Not_Exist()
        {
            // Arrange
            var command = new DeleteProductByIdCommand(Guid.NewGuid());

            // Simulate product not found
            SetupProductRepositoryMock(command.Id, null);

            var handler = new DeleteProductByIdHandler(_productRepositoryMock.Object);

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ProductNotFoundException>()
                .WithMessage($"Product with id '{command.Id}' was not found.");
        }
        [Test]
        public async Task DeleteProductSuccess()
        {
            // Arrange
            var command = new DeleteProductByIdCommand(Guid.NewGuid());

            // Simulate product found
            SetupProductRepositoryMock(command.Id, new Product());

            var handler = new DeleteProductByIdHandler(_productRepositoryMock.Object);

            // Act & Assert
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);
            await act.Should().NotThrowAsync();
        }
        #endregion
    }
}
