using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Domain.Specs;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace Catalog.Infrastructure.Tests
{
    [TestFixture]
    public class ProductRepositoryTests
    {
        private Mock<ICatalogContext> _mockContext;
        private Mock<IMongoCollection<Product>> _mockCollection;
        private ProductRepository _productRepository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<ICatalogContext>();
            _mockCollection = new Mock<IMongoCollection<Product>>();
            _mockContext.Setup(c => c.Products).Returns(_mockCollection.Object);
            _productRepository = new ProductRepository(_mockContext.Object);
        }

        [TestCase(10, "")]
        [TestCase(20, "Name")]
        [TestCase(70, "priceAsc")]
        [TestCase(100, "priceDesc")]
        [Test]
        public async Task GetProducts_Should_Return_Pagination_Of_Products(int pageSize, string sort)
        {
            // Arrange
            var catalogSpecParams = new CatalogSpecParams
            {
                PageIndex = 1,
                PageSize = pageSize,
                Sort = sort,
                BrandId = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Search = "Product",
            };

            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Test Product 1" },
                new Product { Id = Guid.NewGuid(), Name = "Test Product 2" }
            };
            var mockCursor = new Mock<IAsyncCursor<Product>>();
            mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            mockCursor.Setup(x => x.Current).Returns(products);
            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                                   It.IsAny<FindOptions<Product, Product>>(),
                                                   It.IsAny<CancellationToken>()))
                           .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _productRepository.GetProducts(catalogSpecParams);

            // Assert
            Assert.That(result.Data, Is.EqualTo(products));
            // Additional assertions for PageSize behavior
            Assert.That(catalogSpecParams.PageSize, Is.EqualTo(Math.Min(pageSize, CatalogSpecParams.MaxPageSize)), "PageSize should be capped correctly");
        }

        [Test]
        public async Task Create_Should_Insert_Product()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product" };

            // Act
            var result = await _productRepository.Create(product);

            // Assert
            _mockCollection.Verify(c => c.InsertOneAsync(product, null, default), Times.Once);
            Assert.That(result, Is.EqualTo(product));
        }

        [Test]
        public async Task DeleteById_Should_Return_True_When_Deleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var deleteResult = new DeleteResult.Acknowledged(1);
            _mockCollection.Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Product>>(), default))
                .ReturnsAsync(deleteResult);

            // Act
            var result = await _productRepository.DeleteById(id);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteById_Should_Return_False_When_Not_Deleted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var deleteResult = new DeleteResult.Acknowledged(0);
            _mockCollection.Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Product>>(), default))
                .ReturnsAsync(deleteResult);

            // Act
            var result = await _productRepository.DeleteById(id);

            // Assert
            Assert.IsFalse(result);
        }


        [Test]
        public async Task GetById_Should_Return_Product()
        {
            // Arrange
            var id = Guid.NewGuid();
            var product = new Product { Id = id, Name = "Test Product" };
            var mockCursor = new Mock<IAsyncCursor<Product>>();
            mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            mockCursor.Setup(x => x.Current).Returns(new List<Product> { product });
            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                                   It.IsAny<FindOptions<Product, Product>>(),
                                                   It.IsAny<CancellationToken>()))
                           .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _productRepository.GetById(id);

            // Assert
            Assert.That(result, Is.EqualTo(product));
        }


        [Test]
        public async Task Update_Should_Return_True_When_Updated()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Updated Product" };
            var updateResult = new ReplaceOneResult.Acknowledged(1, 1, null);
            _mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                product,
                It.IsAny<ReplaceOptions>(),
                default
            )).ReturnsAsync(updateResult);

            // Act
            var result = await _productRepository.Update(product);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Update_Should_Return_False_When_Acknowledged_But_ModifiedCount_Is_Zero()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Updated Product" };
            var updateResult = new ReplaceOneResult.Acknowledged(1, 0, null); // Acknowledged but ModifiedCount == 0
            _mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                product,
                It.IsAny<ReplaceOptions>(),
                default
            )).ReturnsAsync(updateResult);

            // Act
            var result = await _productRepository.Update(product);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Update_Should_Return_False_When_Not_Updated()
        {
            // Arrange
            var product = new Product { Id = Guid.NewGuid(), Name = "Updated Product" };
            var updateResult = new ReplaceOneResult.Acknowledged(0, 0, null); // Acknowledged without ObjectId
            _mockCollection.Setup(c => c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<Product>>(),
                product,
                It.IsAny<ReplaceOptions>(),
                default
            )).ReturnsAsync(updateResult);

            // Act
            var result = await _productRepository.Update(product);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetByName_Should_Return_Products()
        {
            // Arrange
            var name = "Test";
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Test Product 1" },
                new Product { Id = Guid.NewGuid(), Name = "Test Product 2" }
            };
            var mockCursor = new Mock<IAsyncCursor<Product>>();
            mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
            mockCursor.Setup(x => x.Current).Returns(products);
            _mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Product>>(),
                                                   It.IsAny<FindOptions<Product, Product>>(),
                                                   It.IsAny<CancellationToken>()))
                           .ReturnsAsync(mockCursor.Object);

            // Act
            var result = await _productRepository.GetByName(name);

            // Assert
            Assert.That(result, Is.EqualTo(products));
        }
    }
}
