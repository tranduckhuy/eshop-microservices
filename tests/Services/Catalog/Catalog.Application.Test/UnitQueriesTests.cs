using Catalog.Application.Commands.Handlers;
using Catalog.Application.Exceptions;
using Catalog.Application.Queries;
using Catalog.Application.Queries.Handlers;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Domain.Specs;
using MongoDB.Driver;
using Moq;
using System.Reflection.Metadata;

namespace Application
{
    [TestFixture]
    public class UnitQueriesTests
    {
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private Mock<IBrandRepository> _brandRepositoryMock;
        private GetAllBrandsHandler _getAllBrandsHandler;
        private GetAllCategoriesHandler _getAllCategoriesHandler;
        private GetProductByIdHandler _getProductByIdHandler;
        private GetProductByNameHandler _getProductByNameHandler;
        private GetProductsHandler _getProductsHandler;

        [SetUp]
        public void Setup()
        {
            // Repositories
            _productRepositoryMock = new Mock<IProductRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _brandRepositoryMock = new Mock<IBrandRepository>();

            // Handlers
            _getAllBrandsHandler = new GetAllBrandsHandler(_brandRepositoryMock.Object);
            _getAllCategoriesHandler = new GetAllCategoriesHandler(_categoryRepositoryMock.Object);
            _getProductByIdHandler = new GetProductByIdHandler(_productRepositoryMock.Object);
            _getProductByNameHandler = new GetProductByNameHandler(_productRepositoryMock.Object);
            _getProductsHandler = new GetProductsHandler(_productRepositoryMock.Object);
        }

        #region GetAllBrandsQueryHandler

        [Test]
        public async Task Handle_BrandsExist_ReturnsBrandResponses()
        {
            // Arrange
            var query = new GetAllBrandsQuery();
            var brands = new List<Brand>
            {
                new Brand { Id = Guid.NewGuid(), Name = "Brand1" },
                new Brand { Id = Guid.NewGuid(), Name = "Brand2" }
            };

            _brandRepositoryMock.Setup(repo => repo.GetAll())
                                .ReturnsAsync(brands);

            // Act
            var result = await _getAllBrandsHandler.Handle(query, CancellationToken.None);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(brands, Has.Count.EqualTo(((List<BrandResponse>)result).Count));
            });

            // Verify interactions
            _brandRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Test]
        public async Task Handle_NoBrandsExist_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAllBrandsQuery();
            var brands = new List<Brand>();

            _brandRepositoryMock.Setup(repo => repo.GetAll())
                                .ReturnsAsync(brands);

            // Act
            var result = await _getAllBrandsHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            // Verify interactions
            _brandRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }

        #endregion

        #region GetAllCategoriesQueryHandler
        [Test]
        public async Task Handle_CategoriesExist_ReturnsCategoryResponses()
        {
            // Arrange
            var query = new GetAllCategoriesQuery();
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category1" },
                new Category { Id = Guid.NewGuid(), Name = "Category2" }
            };

            _categoryRepositoryMock.Setup(repo => repo.GetAll())
                                   .ReturnsAsync(categories);

            // Act
            var result = await _getAllCategoriesHandler.Handle(query, CancellationToken.None);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(categories, Has.Count.EqualTo(((List<CategoryResponse>)result).Count));
            });

            // Verify interactions
            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Test]
        public async Task Handle_NoCategoriesExist_ReturnsEmptyList()
        {
            // Arrange
            var query = new GetAllCategoriesQuery();
            var categories = new List<Category>();

            _categoryRepositoryMock.Setup(repo => repo.GetAll())
                                   .ReturnsAsync(categories);

            // Act
            var result = await _getAllCategoriesHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            // Verify interactions
            _categoryRepositoryMock.Verify(repo => repo.GetAll(), Times.Once);
        }

        #endregion

        #region GetProductByIdHandler

        [Test]
        public async Task Handle_GetProductById_ReturnsExistingProduct()
        {
            // Arrange
            var query = new GetProductByIdQuery(id: Guid.NewGuid());
            var product = new Product { Id = query.Id, Name = "Product1" };

            _productRepositoryMock.Setup(repo => repo.GetById(query.Id))
                                  .ReturnsAsync(product);

            // Act
            var result = await _getProductByIdHandler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.Not.Null, "The result should not be null");
                Assert.That(result.Id, Is.EqualTo(product.Id), $"Expected Id {product.Id}, but got {result.Id}");
                Assert.That(result.Name, Is.EqualTo(product.Name), $"Expected Name '{product.Name}', but got '{result.Name}'");
            });

            // Verify interactions
            _productRepositoryMock.Verify(repo => repo.GetById(query.Id), Times.Once);
        }

        [Test]
        public void Handle_Throws_ProductNotFoundException_When_Product_With_Id_Not_Exist()
        {
            // Arrange
            var query = new GetProductByIdQuery(id: Guid.NewGuid());

            _productRepositoryMock.Setup(repo => repo.GetById(query.Id))
                                  .ReturnsAsync((Product?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _getProductByIdHandler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo($"Product with Id '{query.Id}' was not found."));

            // Verify interactions
            _productRepositoryMock.Verify(repo => repo.GetById(query.Id), Times.Once);
        }

        #endregion

        #region GetProductByNameHandler
        [Test]
        public async Task Handle_GetProductByName_ReturnsExistingProducts()
        {
            // Arrange
            var query = new GetProductByNameQuery(name: "Product1");
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = query.Name },
                new Product { Id = Guid.NewGuid(), Name = query.Name }
            };

            _productRepositoryMock.Setup(repo => repo.GetByName(query.Name))
                                  .ReturnsAsync(products);

            // Act
            var result = await _getProductByNameHandler.Handle(query, CancellationToken.None);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(products, Has.Count.EqualTo(((List<ProductResponse>)result).Count));
            });

            // Verify interactions
            _productRepositoryMock.Verify(repo => repo.GetByName(query.Name), Times.Once);
        }

        [Test]
        public void Handle_Throws_ProductNotFoundException_When_Product_With_Name_Not_Exist()
        {
            // Arrange
            var query = new GetProductByNameQuery(name: "Product1");

            _productRepositoryMock.Setup(repo => repo.GetByName(query.Name))
                                  .ReturnsAsync(new List<Product>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _getProductByNameHandler.Handle(query, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo($"Product with name '{query.Name}' was not found."));

            // Verify interactions
            _productRepositoryMock.Verify(repo => repo.GetByName(query.Name), Times.Once);
        }

        #endregion

        #region GetProductsHandler

        [TestCase(10)]
        [TestCase(70)]
        [TestCase(100)]
        [Test]
        public async Task Handle_GetProducts_ReturnsProducts(int pageSize)
        {
            // Arrange
            var catalogSpecParams = new CatalogSpecParams
            {
                PageIndex = 1,
                PageSize = pageSize,
                Sort = "Name",
                BrandId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Search = "Product",
            };

            var query = new GetProductsQuery(catalogSpecParams);
            var products = new Pagination<Product>
            {
                Data = new List<Product>
                {
                    new Product { Id = Guid.NewGuid(), Name = "Product1" },
                    new Product { Id = Guid.NewGuid(), Name = "Product2" }
                }
            };

            _productRepositoryMock.Setup(repo => repo.GetProducts(query.CatalogSpecParams))
                                  .ReturnsAsync(products);

            // Act
            var result = await _getProductsHandler.Handle(query, CancellationToken.None);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(products.Data, Has.Count.EqualTo(result.Data.Count));
            });

            // Verify interactions
            _productRepositoryMock.Verify(repo => repo.GetProducts(query.CatalogSpecParams), Times.Once);

            // Additional assertions for PageSize behavior
            Assert.That(catalogSpecParams.PageSize, Is.EqualTo(Math.Min(pageSize, CatalogSpecParams.MaxPageSize)), "PageSize should be capped correctly");
        }

        #endregion
    }
}
