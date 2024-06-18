using Catalog.Application.Commands;
using Catalog.Application.Commands.Handlers;
using Catalog.Application.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Moq;

namespace Catalog.Application.Tests.Commands.Handlers
{
    [TestFixture]
    public class CreateProductHandlerTests
    {
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<IBrandRepository> _brandRepositoryMock;
        private CreateProductHandler _createProductHandler;
        private UpdateProductHandler _updateProductHandler;
        private DeleteProductByIdHandler _deleteProductHandler;

        [SetUp]
        public void Setup()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _brandRepositoryMock = new Mock<IBrandRepository>();

            _createProductHandler = new CreateProductHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object, _brandRepositoryMock.Object);
            _updateProductHandler = new UpdateProductHandler(_productRepositoryMock.Object, _categoryRepositoryMock.Object, _brandRepositoryMock.Object);
            _deleteProductHandler = new DeleteProductByIdHandler(_productRepositoryMock.Object);
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
        public async Task Handle_ShouldCreateProduct_WhenAllValidationsPass()
        {
            // Arrange
            var command = CreateValidCreateProductCommand();

            SetupBrandRepositoryMock(command.BrandId, new Brand());
            SetupCategoryRepositoryMock(command.CategoryId, new Category());
            var productEntity = new Product { Id = Guid.NewGuid(), Name = command.Name };
            _productRepositoryMock.Setup(repo => repo.Create(It.IsAny<Product>())).ReturnsAsync(productEntity);

            // Act
            var result = await _createProductHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(productEntity.Id, Is.EqualTo(result.Id));
            });
            _productRepositoryMock.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public void CreateProduct_Throws_BrandNotFoundException_When_Brand_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidCreateProductCommand();

            // Simulate brand not found
            SetupBrandRepositoryMock(command.BrandId, null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BrandNotFoundException>(() => _createProductHandler.Handle(command, CancellationToken.None));
            Assert.That(command.BrandId, Is.EqualTo(ex.BrandId));
        }

        [Test]
        public void CreateProduct_Throws_CategoryNotFoundException_When_Category_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidCreateProductCommand();

            SetupBrandRepositoryMock(command.BrandId, new Brand());
            SetupCategoryRepositoryMock(command.CategoryId, null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<CategoryNotFoundException>(() => _createProductHandler.Handle(command, CancellationToken.None));
            Assert.That(command.CategoryId, Is.EqualTo(ex.CategoryId));
        }

        #endregion

        #region UpdateProductCommandTests

        [Test]
        public async Task Handle_ProductExists_UpdateSuccessful()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product exists
            SetupProductRepositoryMock(command.Id, new Product());
            SetupBrandRepositoryMock(command.BrandId, new Brand());
            SetupCategoryRepositoryMock(command.CategoryId, new Category());
            _productRepositoryMock.Setup(repo => repo.Update(It.IsAny<Product>()))
                      .ReturnsAsync(true);

            // Act
            var result = await _updateProductHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);

            // Verify interactions
            _productRepositoryMock.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
            _productRepositoryMock.Verify(repo => repo.GetById(command.Id), Times.Once);
            _brandRepositoryMock.Verify(repo => repo.GetById(command.BrandId), Times.Once);
            _categoryRepositoryMock.Verify(repo => repo.GetById(command.CategoryId), Times.Once);
        }

        [Test]
        public void UpdateProduct_Throws_ProductNotFoundException_When_Product_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product not found
            SetupProductRepositoryMock(command.Id, null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _updateProductHandler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo($"Product with Id '{command.Id}' was not found."));
        }

        [Test]
        public void UpdateProduct_Throws_BrandNotFoundException_When_Brand_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product exists
            SetupProductRepositoryMock(command.Id, new Product());
            SetupBrandRepositoryMock(command.BrandId, null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<BrandNotFoundException>(() => _updateProductHandler.Handle(command, CancellationToken.None));
            Assert.That(command.BrandId, Is.EqualTo(ex.BrandId));
        }

        [Test]
        public void UpdateProduct_Throws_CategoryNotFoundException_When_Category_Does_Not_Exist()
        {
            // Arrange
            var command = CreateValidUpdateProductCommand();

            // Simulate product exists
            SetupProductRepositoryMock(command.Id, new Product());
            SetupBrandRepositoryMock(command.BrandId, new Brand());
            SetupCategoryRepositoryMock(command.CategoryId, null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<CategoryNotFoundException>(() => _updateProductHandler.Handle(command, CancellationToken.None));
            Assert.That(command.CategoryId, Is.EqualTo(ex.CategoryId));
        }

        #endregion

        #region DeleteProductCommandTests

        [Test]
        public async Task Handle_ProductExists_DeleteSuccessful()
        {
            // Arrange
            var command = new DeleteProductByIdCommand(Guid.NewGuid());

            // Simulate product found
            SetupProductRepositoryMock(command.Id, new Product { Id = command.Id });
            _productRepositoryMock.Setup(repo => repo.DeleteById(command.Id))
                      .ReturnsAsync(true);

            // Act
            var result = await _deleteProductHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);

            // Verify interactions
            _productRepositoryMock.Verify(repo => repo.GetById(command.Id), Times.Once);
            _productRepositoryMock.Verify(repo => repo.DeleteById(command.Id), Times.Once);
        }

        [Test]
        public void DeleteProduct_Throws_ProductNotFoundException_When_Product_Does_Not_Exist()
        {
            // Arrange
            var command = new DeleteProductByIdCommand(Guid.NewGuid());

            // Simulate product not found
            SetupProductRepositoryMock(command.Id, null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _deleteProductHandler.Handle(command, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo($"Product with Id '{command.Id}' was not found."));
        }

        #endregion  
    }
}
