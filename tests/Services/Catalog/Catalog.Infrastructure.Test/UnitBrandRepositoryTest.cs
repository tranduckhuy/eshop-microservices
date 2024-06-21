using Catalog.Domain.Entities;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using MongoDB.Driver;
using Moq;

namespace Catalog.Infrastructure.Test
{
    public class UnitBrandRepositoryTest
    {
        private Mock<ICatalogContext> _mockContext;
        private Mock<IMongoCollection<Brand>> _mockCollection;
        private Mock<IAsyncCursor<Brand>> _mockCursor;
        private BrandRepository _repository;
        private List<Brand> _brands;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<ICatalogContext>();
            _mockCollection = new Mock<IMongoCollection<Brand>>();
            _mockCursor = new Mock<IAsyncCursor<Brand>>();
            _brands = new List<Brand>
            {
                new Brand { Id = Guid.NewGuid(), Name = "Brand1" },
                new Brand { Id = Guid.NewGuid(), Name = "Brand2" }
            };

            _mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
            _mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.Setup(x => x.Current).Returns(_brands);

            _mockCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Brand>>(), It.IsAny<FindOptions<Brand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            _mockContext.Setup(c => c.Brands).Returns(_mockCollection.Object);

            _repository = new BrandRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllBrands()
        {
            var result = await _repository.GetAll();
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Brand1"));
        }

        [Test]
        public async Task GetById_ShouldReturnCorrectBrand()
        {
            var brandId = _brands.First().Id;
            var filter = Builders<Brand>.Filter.Eq(b => b.Id, brandId);

            _mockCollection.Setup(x => x.FindAsync(It.Is<FilterDefinition<Brand>>(f => f == filter), It.IsAny<FindOptions<Brand>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            var result = await _repository.GetById(brandId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Brand1"));
        }
    }
}